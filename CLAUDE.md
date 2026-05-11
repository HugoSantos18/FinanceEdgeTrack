# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

FinanceEdgeTrack é uma Web API de finanças pessoais em **ASP.NET Core 8.0** com **PostgreSQL** (Npgsql + EF Core 8). Domínio: usuário tem uma `Carteira` única, que agrega `Receitas`, `Despesas` e `Metas`. Cada `Meta` acumula `AporteMetas` (depósitos parciais até atingir `ValorAlvo`). Autenticação via JWT Bearer + ASP.NET Identity.

## Common commands

Todos os comandos rodam a partir da raiz do repositório.

```powershell
# Build (a sln tem o projeto Web + xUnit)
dotnet build FinanceEdgeTrack.sln

# Quando o app está rodando em VS, o build falha com MSB3027 (file lock).
# Pare o app (Shift+F5) ou rode sem gerar o apphost:
dotnet build FinanceEdgeTrack.sln "-p:UseAppHost=false"

# Run API (Swagger em http://localhost:5230/swagger ou https://localhost:7149/swagger)
dotnet run --project FinanceEdgeTrack

# Tests (xUnit + Moq)
dotnet test
dotnet test --filter "FullyQualifiedName~PostDespesa"   # filtrar por nome

# EF Core migrations — TODAS rodam de dentro de FinanceEdgeTrack/
cd FinanceEdgeTrack
dotnet ef migrations add <NomeDescritivo>
dotnet ef migrations remove
dotnet ef database update
dotnet ef database update <NomeDaMigration>             # ir até uma migration específica
```

Connection string e JWT key vêm via env vars (`env.example` mostra o formato). User secrets também são suportados (`AddUserSecrets<Program>()` em `App.cs`).

## Arquitetura — visão geral

Camadas dentro do projeto `FinanceEdgeTrack/`:

```
Controllers/          → endpoints HTTP, [ApiController] + versionado em api/v{version:apiVersion}/
Application/
  Common/             → ApiResponse<T> (wrapper de retorno), PagedList/PaginationParams (paginação), ResultMessages
  Dtos/Read|Write/    → DTOs separados por intenção: Read = responses, Write = inputs (Create*/Update*)
  Mappings/           → MapsterMappingConfig (centraliza Adapt rules)
  Services/           → MetaService, ReceitaService, DespesaService, CarteiraService, AuthService, CacheService, …
Domain/
  Models/             → entidades ricas (Carteira, Meta, AporteMetas, Receita, Despesa, ApplicationUser)
  Interfaces/         → IRepository<T>, IUnitOfWork, I*Service, I*Metrics, ICurrentUser, etc.
  Enum/               → Status (EmAberto/Concluido/…)
Infrastructure/
  Data/               → AppDbContext, AppDbContextFactory (design-time), DataBaseConfiguration, UnitOfWork
  Data/Migrations/    → migrations EF Core
  Repositories/       → Repository<T> base + repositórios específicos
  Extensions/         → middleware (ApiExceptionFilter, CORS, RateLimiting, Versioning, Roles)
  Config/             → POCOs ligados ao appsettings (JwtSettings, CorsOptions, RateLimitingOptions, …)
  Seed/               → UserSeed (cria admin no dev/seed)
Logging/              → CustomerLogger, provider customizado
Properties/           → launchSettings.json
```

**Test project**: `FinanceEdgeTrackxUnitTests/` (xUnit + Moq), referencia `FinanceEdgeTrack.csproj`. Estruturado por feature: `UnitTests/Despesas/`, `UnitTests/Metas/`, etc., com pasta `Configs/` para controllers de teste compartilhados.

### Padrões adotados (e o porquê deles)

- **Repository + Unit of Work**: `IRepository<T>` expõe `Query() → IQueryable<T>` (deixa o filtro/projeção para o service, evita acoplar repositório a casos de uso). `IUnitOfWork` agrega os repositórios + `CommitAsync()` + `BeginTransactionAsync()`. Útil aqui porque o DbContext é único por request e operações cross-aggregate (Carteira+Meta+Aporte) precisam de uma fronteira transacional clara.
- **Service layer retorna `ApiResponse<T>`** (`Ok(data, msg)` / `Fail(msg)`) — controllers só fazem `if (!response.Success) return BadRequest(response)`. Padroniza o shape de erro e evita exceptions como controle de fluxo.
- **DTOs separados Read/Write**: input (CreateXxxDTO/UpdateXxxDTO) ≠ output (XxxDTO). Evita expor campos `private set` indevidamente e mantém validação por intenção.
- **Mapster `ProjectToType<T>()` em queries** (não `_mapper.Map` depois de carregar): roda o mapping no SQL, economiza memória e ciclos.
- **Cache com chave por usuário**: `_cacheService.SetCacheKey(_currentUser.UserId)` — listagens filtradas (`MetasFiltradas*Async`) cacheiam por user. **Atenção**: hoje não há invalidação após writes (cache pode ficar stale por 1-2 min). Limitação conhecida.

## Convenções específicas do projeto (gotchas)

Essas regras vieram à custa de bugs reais. Violar quebra coisas.

### 1. `Repository.UpdateAsync` NÃO salva — só marca

```csharp
public Task UpdateAsync(T entity)
{
    _context.Entry(entity).State = EntityState.Modified;  // apenas marca
    return Task.CompletedTask;                            // nada vai pro banco aqui
}
```

Para persistir, sempre chamar `_uof.CommitAsync()` (= `SaveChangesAsync`). Se você abriu transação, a ordem correta é:

```csharp
using var tx = await _uof.BeginTransactionAsync();
// … modificações em entidades tracked …
await _uof.CommitAsync();      // grava no banco DENTRO da transação
await tx.CommitAsync();        // confirma a transação
```

Chamar só `tx.CommitAsync()` sem `_uof.CommitAsync()` antes resulta em transação vazia — bug recorrente.

### 2. Para débito de Saldo da Carteira, use `ExecuteUpdateAsync` com guarda atômica

A Carteira tem token de concorrência via `xmin` (coluna de sistema do PG). Para operações de saldo, **prefira UPDATE direto com guarda** em vez de change tracking:

```csharp
var rows = await _uof.CarteiraRepository.Query()
    .Where(c => c.CarteiraId == carteira.CarteiraId && c.Saldo >= valor)
    .ExecuteUpdateAsync(s => s.SetProperty(c => c.Saldo, c => c.Saldo - valor));

if (rows == 0)
    return ApiResponse<...>.Fail("Saldo insuficiente");
```

**Por quê**: a guarda `Saldo >= valor` no WHERE é avaliada pelo PG e impede overdraft mesmo sob concorrência (race entre check-em-memória + save). `ExecuteUpdateAsync` também é parametrizado (SQL injection-safe).

### 3. Concorrência otimista da Carteira: `xmin` (não `RowVersion`)

`AppDbContext` configura:

```csharp
#pragma warning disable CS0618
entity.UseXminAsConcurrencyToken();    // método obsoleto, mas o único que funciona
#pragma warning restore CS0618
```

`xmin` é coluna de sistema do PostgreSQL — auto-incrementa em todo UPDATE. **Não usar `IsRowVersion()`** porque a migration scaffolder gera `AddColumn<uint>("xmin")`, que falha no PG. Se o gerador de migration insistir em criar a coluna `xmin`, edite a migration à mão removendo o `AddColumn`/`DropColumn` do `xmin`.

### 4. Meta: estado é recalculado por `AtualizarProgresso()` (idempotente)

Depois de qualquer Add/Remove em `Meta.Aportes`, chame:

```csharp
meta.AdicionarAporte(novoAporte);    // ou meta.RemoverAporte(aporte)
meta.AtualizarProgresso();           // recalcula ValorAtual / Porcentagem / Restante / Último depósito / Status
```

**Por quê idempotente**: o método recomputa tudo do zero a partir de `Aportes.Sum(...)`. Pode ser chamado N vezes sem efeito colateral; nunca deixa Meta em estado inconsistente.

### 5. `AporteMetas` exige `MetaId` no factory

```csharp
var aporte = AporteMetas.Criar(meta.MetaId, dto.Valor);   // FK obrigatória
```

FK explícita + NOT NULL desde a migration `AddXminAndAporteMetaIdRequired`. Não criar aporte sem MetaId (lança `ArgumentException`).

### 6. Autorização: tudo escopado pelo usuário corrente

Listagens e mutations sempre filtram por `_currentUser.UserId` (direto) ou via `carteira.CarteiraId` (carteira foi carregada pelo `_carteiraService.GetCarteiraAsync()`, que usa `_currentUser.UserId`). Antes de adicionar endpoint novo, garanta esse escopo — não confiar só em `[Authorize]`.

### 7. Identity + JWT

`UserSeed.SeedAdminAsync` roda em dev OU quando `SEED_ADMIN=true`. Admin padrão: `admin@financeedgetrack.com` / `Admin@2026#Secure!`. Senha hard-coded só para dev.

## Quando criar uma migration

Mudança em entidade do `Domain/Models/` que reflete em schema → `dotnet ef migrations add <Nome>`, revisar o arquivo gerado **antes** de `database update`. Atenção especial:

- Se a migration tem `AddColumn<uint>("xmin")` → remover (sistema do PG).
- Se a migration torna uma coluna existente NOT NULL → adicione `migrationBuilder.Sql("DELETE FROM ... WHERE col IS NULL")` antes do `AlterColumn`, senão o `defaultValue` injetado pode violar FK.
- Para reverter sem aplicar: `dotnet ef migrations remove` (só funciona se a última não foi aplicada no banco).

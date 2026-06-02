<div align="center">

# FinanceEdgeTrack API 🚀

**API REST para controle financeiro pessoal — despesas, receitas, metas com aportes progressivos e dashboards analíticos.**

![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet?logo=dotnet)
![C#](https://img.shields.io/badge/Language-C%23-239120?logo=csharp)
![PostgreSQL](https://img.shields.io/badge/Database-PostgreSQL-4169E1?logo=postgresql)
![JWT](https://img.shields.io/badge/Auth-JWT_Bearer-orange?logo=jsonwebtokens)
![Swagger](https://img.shields.io/badge/Docs-Swagger-85EA2D?logo=swagger)
![Architecture](https://img.shields.io/badge/Architecture-Clean_Architecture-lightblue)

</div>

---

## Índice

1. [Sobre o Projeto](#sobre-o-projeto)
2. [Funcionalidades](#funcionalidades)
3. [Arquitetura](#arquitetura)
4. [Stack Tecnológica](#stack-tecnológica)
5. [Pré-requisitos](#pré-requisitos)
6. [Como Rodar o Projeto](#como-rodar-o-projeto)
7. [Autenticação e Autorização](#autenticação-e-autorização)
8. [Rate Limiting](#rate-limiting)
9. [Formato de Resposta](#formato-de-resposta)
10. [Códigos de Status](#códigos-de-status)
11. [Endpoints](#endpoints)
    - [Auth](#-auth)
    - [Dashboard — Admin](#-dashboard--admin)
    - [Despesas](#-despesas)
    - [Receitas](#-receitas)
    - [Metas](#-metas)
    - [Aportes em Metas](#-aportes-em-metas)
12. [Swagger UI](#swagger-ui)
13. [Autor](#autor)

---

## Sobre o Projeto

O **FinanceEdgeTrack** é uma API REST desenvolvida em **ASP.NET Core 8** voltada para o gerenciamento financeiro pessoal. Permite ao usuário registrar e acompanhar despesas (incluindo as fixas), receitas, metas financeiras de médio/longo prazo com sistema de aportes progressivos, e visualizar dashboards analíticos consolidados por período.

A API foi construída seguindo os princípios de **Clean Architecture**, com separação clara entre as camadas de domínio, aplicação, infraestrutura e apresentação. Autenticação via **JWT Bearer** com controle de acesso por roles, **rate limiting** nativo do ASP.NET Core e documentação interativa via **Swagger/OpenAPI**.

---

## Funcionalidades

- **Autenticação completa** — registro, login, refresh de token e revogação via JWT Bearer
- **Gestão de roles** — controle de acesso por políticas (`Admin`, `User`)
- **Despesas** — CRUD completo com suporte a despesas fixas e filtros de ordenação
- **Receitas** — CRUD completo com filtros de ordenação por valor
- **Metas financeiras** — criação e acompanhamento de metas com valor-alvo, data limite e status
- **Aportes em metas** — registro de aportes financeiros parciais com cálculo automático de progresso
- **Dashboards analíticos** — consolidados mensal, geral e por período personalizado
- **Paginação** — em todos os endpoints de listagem (`PageNumber`, `PageSize` até 40)
- **Rate limiting** — proteção contra abuso por IP (anônimos) e por usuário (autenticados)
- **Cache em memória** — para otimização de consultas repetitivas

---

## Arquitetura

O projeto segue o padrão **Clean Architecture** dividido em 4 projetos distintos, com a regra de dependência respeitada estritamente (camadas externas dependem de internas, nunca o contrário):

```
┌─────────────────────────────────────────────────────────────────┐
│                      FinanceEdgeTrackAPI                        │
│         Controllers · Filters · Middlewares · Program           │
├─────────────────────────────────────────────────────────────────┤
│                  FinanceEdgeTrack.Application                   │
│           Services · DTOs · Interfaces · Mapster Config         │
├─────────────────────────────────────────────────────────────────┤
│                 FinanceEdgeTrack.Infrastructure                  │
│     EF Core · Identity · JWT · Repositories · Cache · Logging   │
├─────────────────────────────────────────────────────────────────┤
│                    FinanceEdgeTrack.Domain                      │
│              Entities · Enums · Interfaces de Domínio           │
└─────────────────────────────────────────────────────────────────┘
```

| Camada | Responsabilidade |
|---|---|
| **API** | Receber requisições HTTP, autenticar, aplicar filtros globais e delegar para a Application |
| **Application** | Orquestrar a lógica de negócio, transformar dados entre camadas (DTOs) e definir contratos (interfaces) |
| **Infrastructure** | Implementar os contratos: acesso a banco, autenticação, cache, repositórios concretos |
| **Domain** | Entidades e regras de negócio puras, sem dependências externas |

---

## Stack Tecnológica

| Tecnologia | Versão | Função |
|---|---|---|
| ASP.NET Core | 8.0 | Framework principal da API |
| Entity Framework Core | 8.0.23 | ORM para acesso a dados |
| PostgreSQL + Npgsql | 8.0.11 | Banco de dados relacional |
| ASP.NET Core Identity | 8.0.23 | Gestão de usuários, roles e senhas |
| JWT Bearer | 8.0.23 | Autenticação stateless |
| Swashbuckle (Swagger) | 6.6.2 | Documentação interativa OpenAPI |
| Mapster | 7.4.0 | Mapeamento de objetos (Entity → DTO) |
| Asp.Versioning | 8.1.1 | Versionamento de endpoints da API |
| ASP.NET Core Rate Limiting | 8.0 (nativo) | Proteção contra abuso de requisições |

---

## Pré-requisitos

Antes de começar, certifique-se de ter instalado:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/) (qualquer versão recente)
- Ferramenta `dotnet-ef` para rodar as migrations:

```bash
dotnet tool install --global dotnet-ef
```

---

## Como Rodar o Projeto

### 1. Clone o repositório

```bash
git clone https://github.com/HugoSantos18/FinanceEdgeTrack.git
cd FinanceEdgeTrack
```

### 2. Configure os User Secrets

As configurações sensíveis (string de conexão, chave JWT, etc.) são gerenciadas via **User Secrets** do .NET e **não devem ser commitadas**. Para configurar:

```bash
dotnet user-secrets init --project FinanceEdgeTrack/FinanceEdgeTrackAPI.csproj
```

Adicione os seguintes valores (adaptando para o seu ambiente):

```bash
# String de conexão com o PostgreSQL
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=FinanceEdgeTrack;Username=postgres;Password=suasenha" --project FinanceEdgeTrack

# Configurações do JWT
dotnet user-secrets set "JWT:SecretKey" "sua-chave-secreta-com-pelo-menos-32-caracteres" --project FinanceEdgeTrack
dotnet user-secrets set "JWT:ValidIssuer" "FinanceEdgeTrackAPI" --project FinanceEdgeTrack
dotnet user-secrets set "JWT:ValidAudience" "FinanceEdgeTrackClient" --project FinanceEdgeTrack

# Rate Limiting (requisições por janela de tempo)
dotnet user-secrets set "RateLimiting:AuthenticatedUserLimit" "60" --project FinanceEdgeTrack
dotnet user-secrets set "RateLimiting:LoginLimit" "10" --project FinanceEdgeTrack
dotnet user-secrets set "RateLimiting:WindowSeconds" "60" --project FinanceEdgeTrack

# CORS (origens permitidas)
dotnet user-secrets set "Cors:AllowedOrigins:0" "http://localhost:3000" --project FinanceEdgeTrack
```

> A estrutura completa esperada em `appsettings.json` / secrets:
>
> ```json
> {
>   "ConnectionStrings": {
>     "DefaultConnection": "Host=...;Database=...;Username=...;Password=..."
>   },
>   "JWT": {
>     "SecretKey": "...",
>     "ValidIssuer": "...",
>     "ValidAudience": "..."
>   },
>   "RateLimiting": {
>     "AuthenticatedUserLimit": 60,
>     "LoginLimit": 10,
>     "WindowSeconds": 60
>   },
>   "Cors": {
>     "AllowedOrigins": ["http://localhost:3000"]
>   }
> }
> ```

### 3. Aplique as Migrations

```bash
dotnet ef database update \
  --project FinanceEdgeTrack.Infrastructure \
  --startup-project FinanceEdgeTrack
```

### 4. Rode a aplicação

```bash
dotnet run --project FinanceEdgeTrack
```

A API estará disponível em `https://localhost:{porta}`.

### 5. Acesse o Swagger UI

Em ambiente de desenvolvimento, a documentação interativa está disponível em:

```
https://localhost:{porta}/swagger
```

---

## Autenticação e Autorização

### Fluxo de autenticação

```
1. POST /api/Auth/Register  →  Cria a conta do usuário
2. POST /api/Auth/Login     →  Retorna { accessToken, refreshToken }
3. Todas as requisições     →  Header: Authorization: Bearer {accessToken}
```

O `accessToken` tem validade curta. Quando expirar, use o `refreshToken` no endpoint `/api/Auth/refresh` (requer role **Admin**) para obter um novo par de tokens sem precisar fazer login novamente.

### Políticas de acesso

| Role | Endpoints liberados |
|---|---|
| **Anônimo** | `POST /api/Auth/Login` e `POST /api/Auth/Register` |
| **User** (autenticado) | Despesas, Receitas, Metas e Aportes em Metas |
| **Admin** | Tudo acima + Dashboard + gerenciamento de usuários e roles |

> A política padrão da API exige autenticação em **todos** os endpoints, exceto Login e Register, que possuem `[AllowAnonymous]` explícito.

### Usando o token no Swagger

1. Clique no botão **Authorize** (cadeado) no topo da página do Swagger
2. No campo `Value`, informe: `Bearer {seu_access_token}`
3. Clique em **Authorize** — todas as requisições seguintes incluirão o header automaticamente

---

## Rate Limiting

A API implementa **rate limiting nativo** do ASP.NET Core com janela deslizante:

| Tipo de requisição | Partição | Limite padrão |
|---|---|---|
| Usuário autenticado | Por `user_id` (claim JWT) | Configurável via `AuthenticatedUserLimit` |
| Anônimo / Login | Por endereço IP | Configurável via `LoginLimit` |

Quando o limite é excedido, a API retorna `429 Too Many Requests`. Os limites e a janela de tempo são configurados via `appsettings.json` / User Secrets na seção `RateLimiting`.

---

## Formato de Resposta

Todos os endpoints retornam um envelope padrão `ApiResponse<T>`:

**Sucesso:**
```json
{
  "success": true,
  "message": "Operação realizada com sucesso",
  "data": { }
}
```

**Erro:**
```json
{
  "success": false,
  "message": "Descrição do erro",
  "data": null
}
```

O campo `data` contém o objeto ou lista retornada em caso de sucesso, ou `null` em caso de erro.

---

## Códigos de Status

| Código | Nome | Quando ocorre |
|---|---|---|
| `200` | OK | Requisição processada com sucesso |
| `400` | Bad Request | Dados inválidos, validação falhou ou regra de negócio violada |
| `401` | Unauthorized | Token ausente, expirado ou inválido |
| `403` | Forbidden | Usuário autenticado sem a role/policy exigida pelo endpoint |
| `404` | Not Found | Recurso não encontrado no banco de dados |
| `429` | Too Many Requests | Limite de requisições (rate limit) excedido |

---

## Endpoints

### Auth

Base: `api/Auth` — **sem versionamento**

| Método | Rota | Autenticação | Descrição |
|---|---|---|---|
| `POST` | `/api/Auth/Login` | Público | Autentica e retorna access token + refresh token |
| `POST` | `/api/Auth/Register` | Público | Registra um novo usuário na plataforma |
| `POST` | `/api/Auth/refresh` | Admin | Renova o access token via refresh token |
| `POST` | `/api/Auth/revoke/{username}` | Admin | Revoga o refresh token de um usuário (força novo login) |
| `POST` | `/api/Auth/make-admin` | Admin | Promove um usuário ao role Admin |
| `POST` | `/api/Auth/AddUserToRole` | Admin | Adiciona um usuário a um role existente |
| `POST` | `/api/Auth/CreateRole` | Admin | Cria um novo role no sistema |

**Status codes possíveis:**

| Endpoint | 200 | 400 | 401 | 403 | 404 | 429 |
|---|:---:|:---:|:---:|:---:|:---:|:---:|
| Login | ✓ | | ✓ | | | ✓ |
| Register | ✓ | ✓ | | | | ✓ |
| refresh | ✓ | ✓ | ✓ | ✓ | | ✓ |
| revoke | ✓ | | ✓ | ✓ | | ✓ |
| make-admin | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| AddUserToRole | ✓ | ✓ | ✓ | ✓ | | ✓ |
| CreateRole | ✓ | ✓ | ✓ | ✓ | | ✓ |

---

### Dashboard — Admin

Base: `api/v1/Dashboard` — **requer role Admin em todos os endpoints**

| Método | Rota | Autenticação | Descrição |
|---|---|---|---|
| `GET` | `/api/v1/Dashboard/monthly` | Admin | Dashboard consolidado de um mês específico (gastos, receitas, metas e KPIs) |
| `GET` | `/api/v1/Dashboard/general` | Admin | Dashboard consolidado geral com todo o histórico do usuário |
| `GET` | `/api/v1/Dashboard/period` | Admin | Dashboard consolidado para um intervalo de datas personalizado |

**Parâmetros de query:**

| Endpoint | Parâmetros |
|---|---|
| `/monthly` | `year` (int), `month` (int, 1–12) |
| `/general` | — |
| `/period` | `dateStart` (DateTime), `dateEnd` (DateTime) |

**Status codes possíveis:**

| Endpoint | 200 | 400 | 401 | 403 | 429 |
|---|:---:|:---:|:---:|:---:|:---:|
| monthly | ✓ | ✓ | ✓ | ✓ | ✓ |
| general | ✓ | ✓ | ✓ | ✓ | ✓ |
| period | ✓ | ✓ | ✓ | ✓ | ✓ |

---

### Despesas

Base: `api/v1/Despesa` — **requer autenticação**

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/v1/Despesa/{id}` | Retorna uma despesa pelo seu ID |
| `GET` | `/api/v1/Despesa/All` | Lista todas as despesas (paginado) |
| `GET` | `/api/v1/Despesa/fixed` | Lista apenas as despesas fixas (paginado) |
| `GET` | `/api/v1/Despesa/biggest-expense` | Lista as despesas ordenadas do maior para o menor valor (paginado) |
| `GET` | `/api/v1/Despesa/lower-expense` | Lista as despesas ordenadas do menor para o maior valor (paginado) |
| `POST` | `/api/v1/Despesa` | Registra uma nova despesa |
| `PUT` | `/api/v1/Despesa/{id}` | Atualiza uma despesa existente |
| `DELETE` | `/api/v1/Despesa/{id}` | Remove uma despesa |

**Parâmetros de paginação** (query, endpoints de listagem):
`pageNumber` (int, default 1) · `pageSize` (int, default 10, máx. 40)

**Status codes possíveis:**

| Endpoint | 200 | 400 | 401 | 404 | 429 |
|---|:---:|:---:|:---:|:---:|:---:|
| GET `/{id}` | ✓ | | ✓ | ✓ | ✓ |
| GET `/All` | ✓ | ✓ | ✓ | | ✓ |
| GET `/fixed` | ✓ | ✓ | ✓ | | ✓ |
| GET `/biggest-expense` | ✓ | ✓ | ✓ | | ✓ |
| GET `/lower-expense` | ✓ | ✓ | ✓ | | ✓ |
| POST `/` | ✓ | ✓ | ✓ | | ✓ |
| PUT `/{id}` | ✓ | ✓ | ✓ | ✓ | ✓ |
| DELETE `/{id}` | ✓ | ✓ | ✓ | | ✓ |

---

### Receitas

Base: `api/v1/Receita` — **requer autenticação**

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/v1/Receita/{id}` | Retorna uma receita pelo seu ID |
| `GET` | `/api/v1/Receita/All` | Lista todas as receitas (paginado) |
| `GET` | `/api/v1/Receita/biggest-expense` | Lista as receitas ordenadas do maior para o menor valor (paginado) |
| `GET` | `/api/v1/Receita/lower-expense` | Lista as receitas ordenadas do menor para o maior valor (paginado) |
| `POST` | `/api/v1/Receita` | Registra uma nova receita |
| `PUT` | `/api/v1/Receita/{id}` | Atualiza uma receita existente |
| `DELETE` | `/api/v1/Receita/{id}` | Remove uma receita |

**Status codes possíveis:**

| Endpoint | 200 | 400 | 401 | 404 | 429 |
|---|:---:|:---:|:---:|:---:|:---:|
| GET `/{id}` | ✓ | | ✓ | ✓ | ✓ |
| GET `/All` | ✓ | ✓ | ✓ | | ✓ |
| GET `/biggest-expense` | ✓ | ✓ | ✓ | | ✓ |
| GET `/lower-expense` | ✓ | ✓ | ✓ | | ✓ |
| POST `/` | ✓ | ✓ | ✓ | | ✓ |
| PUT `/{id}` | ✓ | ✓ | ✓ | ✓ | ✓ |
| DELETE `/{id}` | ✓ | ✓ | ✓ | | ✓ |

---

### Metas

Base: `api/v1/Meta` — **requer autenticação**

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/v1/Meta/{id}` | Retorna uma meta pelo seu ID |
| `GET` | `/api/v1/Meta/All` | Lista todas as metas (paginado) |
| `GET` | `/api/v1/Meta/biggest-expense` | Lista as metas ordenadas do maior para o menor valor-alvo (paginado) |
| `GET` | `/api/v1/Meta/lower-expense` | Lista as metas ordenadas do menor para o maior valor-alvo (paginado) |
| `GET` | `/api/v1/Meta/almost-done` | Lista as metas com maior porcentagem de conclusão primeiro (paginado) |
| `GET` | `/api/v1/Meta/oldest` | Lista as metas das mais antigas para as mais recentes (paginado) |
| `GET` | `/api/v1/Meta/recently` | Lista as metas das mais recentes para as mais antigas (paginado) |
| `GET` | `/api/v1/Meta/status` | Lista as metas filtradas por status (paginado) |
| `POST` | `/api/v1/Meta` | Cria uma nova meta financeira |
| `PUT` | `/api/v1/Meta/{id}` | Atualiza uma meta existente |
| `DELETE` | `/api/v1/Meta/{id}` | Remove uma meta |

**Parâmetro adicional do `/status`:** `status` (query, enum: `Pendente` / `Concluida` / `Cancelada`)

**Status codes possíveis:**

| Endpoint | 200 | 400 | 401 | 404 | 429 |
|---|:---:|:---:|:---:|:---:|:---:|
| GET `/{id}` | ✓ | | ✓ | ✓ | ✓ |
| GET `/All` | ✓ | ✓ | ✓ | | ✓ |
| GET `/biggest-expense` | ✓ | ✓ | ✓ | | ✓ |
| GET `/lower-expense` | ✓ | ✓ | ✓ | | ✓ |
| GET `/almost-done` | ✓ | ✓ | ✓ | | ✓ |
| GET `/oldest` | ✓ | ✓ | ✓ | | ✓ |
| GET `/recently` | ✓ | ✓ | ✓ | | ✓ |
| GET `/status` | ✓ | ✓ | ✓ | | ✓ |
| POST `/` | ✓ | ✓ | ✓ | | ✓ |
| PUT `/{id}` | ✓ | ✓ | ✓ | ✓ | ✓ |
| DELETE `/{id}` | ✓ | ✓ | ✓ | | ✓ |

---

### Aportes em Metas

Base: `api/v1/AporteMetas` — **requer autenticação**

Aportes são contribuições financeiras parciais registradas em uma meta, permitindo o acompanhamento progressivo do valor acumulado até o valor-alvo.

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/v1/AporteMetas/{aporteId}` | Retorna um aporte específico pelo seu ID |
| `GET` | `/api/v1/AporteMetas/meta/{metaId}` | Lista todos os aportes de uma meta (paginado) |
| `GET` | `/api/v1/AporteMetas/meta/{metaId}/total` | Retorna o valor total aportado em uma meta |
| `POST` | `/api/v1/AporteMetas/meta/{metaId}` | Registra um novo aporte em uma meta |
| `DELETE` | `/api/v1/AporteMetas/{aporteId}` | Remove um aporte |

**Restrição de valor:** Um aporte deve ter valor entre `R$ 1,00` e `R$ 99.999.999,00`.

**Status codes possíveis:**

| Endpoint | 200 | 400 | 401 | 404 | 429 |
|---|:---:|:---:|:---:|:---:|:---:|
| GET `/{aporteId}` | ✓ | | ✓ | ✓ | ✓ |
| GET `/meta/{metaId}` | ✓ | ✓ | ✓ | | ✓ |
| GET `/meta/{metaId}/total` | ✓ | | ✓ | ✓ | ✓ |
| POST `/meta/{metaId}` | ✓ | ✓ | ✓ | | ✓ |
| DELETE `/{aporteId}` | ✓ | ✓ | ✓ | | ✓ |

---

## Swagger UI

Em ambiente de desenvolvimento, o Swagger UI está disponível em `/swagger` após iniciar a aplicação.

A documentação inclui:
- Descrição de cada endpoint com seus parâmetros
- Schemas de request e response tipados com `ApiResponse<T>`
- Todos os status codes possíveis por endpoint (incluindo `429`)
- Autenticação JWT integrada — clique em **Authorize** e informe `Bearer {seu_token}`

> O Swagger está habilitado **apenas em ambiente de desenvolvimento** (`ASPNETCORE_ENVIRONMENT=Development`).

---

## Autor

Desenvolvido por **Hugo Santos**

[![GitHub](https://img.shields.io/badge/GitHub-HugoSantos18-181717?logo=github)](https://github.com/HugoSantos18)
[![Email](https://img.shields.io/badge/Email-hugossilva.dev%40gmail.com-D14836?logo=gmail)](mailto:hugossilva.dev@gmail.com)

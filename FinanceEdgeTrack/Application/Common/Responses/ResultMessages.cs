namespace FinanceEdgeTrack.Application.Common.Responses;

public static class ResultMessages
{
    public const string NotFoundReceive = "Receita não encontrada";
    public const string NotFoundDespesa = "Despesa não encontrada";
    public const string NotFoundLancamento = "Lançamento não encontrado.";
    public const string NotFoundMeta = "Meta não encontrada.";
    public const string NotFoundUser = "Usuário não encontrado";
    public const string NotFoundAporte = "Aporte não encontrado.";
    public const string InvalidPrice = "Deve ser informado um valor válido.";
    public const string InvalidCredentials = "Credenciais inválidas.";
    public const string EmptyMetaCollection = "Não foi encontrado nenhuma meta.";
    public const string EmptyAporteCollection = "Não foi encontrado nenhum aporte";
    public const string MoreThanZero = "O valor deve ser maior que zero.";
    public const string WalletNotFound = "Ops, parece que você perdeu sua carteira. :´( ";
    public const string ValidAporte = "Deve ser feito um aporte válido, credenciais incorretas.";
    public const string ValidMeta = "Para finalizar a criação de uma meta, deve ser preenchido as credenciais corretamente.";
    public const string ValidCredentialsSuccess = "Concluído com sucesso!";
    public const string ErrorCreation = "Não foi possível realizar a operação.";
    public const string InvalidLoginCredentials = "Credenciais inválidas, não foi possível fazer o Login.";
    public const string UserAlreadyExists = "Usuário já existe.";
    public const string ConfirmPasswordError = "O campo de confirmação de senha deve ser o mesmo da senha.";
    public const string InvalidAccessToken = "Token de acesso inválido.";
    public const string InvalidRefreshToken = "RefreshToken inválido.";
    public const string RevokeSuccessfull = "Token revogado com sucesso.";
    public const string ConfirmPasswordInavlid = "A confirmação de senha deve ser igual a senha original.";
    public const string MetaCompleted = $"Parabéns você acaba de finalizar sua meta, continue progredindo :D";
    public const string ErrorUpdate = "Não foi possível realizar a atualização";
    public const string InvalidIndentityRoleCreation = $"Não foi possível criar a role";
    public const string ErrorToAddUserToRole = $"Não foi possível adicionar o user a role";
    public const string ErrorToGetWalletAmmountUser = $"Não foi possível resgatar o saldo da carteira.";
}

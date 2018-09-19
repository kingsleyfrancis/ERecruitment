namespace ERecruitment.Services.Accounts
{
    public interface IAccountService
    {
        int GetClientId(string email);

        string GetFullname(int clientId);
    }
}
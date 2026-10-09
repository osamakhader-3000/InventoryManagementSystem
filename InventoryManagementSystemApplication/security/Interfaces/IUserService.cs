using SecurityApplicationCore.Models;

namespace SecurityApplicationLogic.Interfaces
{
    public interface IUserService
    {
        public Task<Guid> CreateNewLocalUser(User user);

        public Task<List<User>> ListUsers();

        public Task<User?> GetUser(Guid userId);

        public Task<User> CreateNewLocalAccount(string username, string email, string password);
        Task<User> GetOrCreateExternalUser(string username,string email);
        Task SendAccountVerificationEmail(User user);

        Task<bool> VerifyEmailToken(string token);
        Task<User> GetValidatedUser(string username, string password);
        Task ChangePassword(Guid userId, string oldPassword, string newPassword);
        Task SuspendUser(Guid userId);
        Task ActivateUser(Guid userId);
        Task<int> ChangeUserRole(Guid userId, Guid roleId);
        Task<List<User>> ListUsersByIds(List<Guid> userIds);
    }
}

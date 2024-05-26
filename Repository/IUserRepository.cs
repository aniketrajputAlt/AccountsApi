using AccountsApi.Model;

namespace AccountsApi.Repository
{
    public interface IUserRepository
    {
        public Task<User> GetUserByUsernameAndPasswordAsync(string username, string password);
       public  Task<bool> UpdateUserPasswordAsync(string username, string newPassword);
       public Task<User> GetUserDetailsByIdAsync(int userId);
    }
}

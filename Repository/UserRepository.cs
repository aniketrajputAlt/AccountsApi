using AccountsApi.Model;
using Microsoft.EntityFrameworkCore;

namespace AccountsApi.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly BankingAppDbContext _context;

        public UserRepository(BankingAppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByUsernameAndPasswordAsync(string username, string password)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password && u.IsActive);
        }

        public async Task<bool> UpdateUserPasswordAsync(string username, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user != null)
            {
                user.Password = newPassword;
                user.LastPasswordChange = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<User> GetUserDetailsByIdAsync(int userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);
        }
    }
}

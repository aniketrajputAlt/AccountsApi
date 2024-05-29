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
        public async Task<bool> UpdatePasswordAsync(string username, string newPassword)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

            if (user == null)
                return false;

            user.Password = newPassword; // Assuming Password is stored in plain text for demonstration
            user.LastPasswordChange = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}

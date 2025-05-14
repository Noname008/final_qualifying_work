using final_qualifying_work.Data;
using final_qualifying_work.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace final_qualifying_work.Services
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int userId);
        Task<UpdateProfileResult> UpdateUserProfileAsync(int userId, UserProfileModel model);
        Task<UpdateProfileResult> RegistryUserAsync(User user);
        Task<UpdateProfileResult> VerifyUserAsync(User user);
    }

    public class UpdateProfileResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public User User { get; set; }
    }

    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(
            AppDbContext context,
            IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<UpdateProfileResult> UpdateUserProfileAsync(int userId, UserProfileModel model)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return new UpdateProfileResult { Success = false, Message = "User not found" };

            // Обновление username
            if (!string.IsNullOrEmpty(model.Username) && model.Username != user.Username)
            {
                if (await _context.Users.AnyAsync(u => u.Username == model.Username))
                    return new UpdateProfileResult { Success = false, Message = "Username already taken" };

                user.Username = model.Username;
            }

            // Обновление email
            if (!string.IsNullOrEmpty(model.Email) && model.Email != user.Email)
            {
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                    return new UpdateProfileResult { Success = false, Message = "Email already in use" };

                user.Email = model.Email;
            }

            // Смена пароля
            if (!string.IsNullOrEmpty(model.CurrentPassword) &&
                !string.IsNullOrEmpty(model.NewPassword))
            {
                var passwordVerification = _passwordHasher.VerifyHashedPassword(
                    user, user.Password, model.CurrentPassword);

                if (passwordVerification != PasswordVerificationResult.Success)
                    return new UpdateProfileResult { Success = false, Message = "Current password is incorrect" };

                user.Password = _passwordHasher.HashPassword(user, model.NewPassword);
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new UpdateProfileResult
            {
                Success = true,
                Message = "Profile updated successfully",
                User = user
            };
        }

        public async Task<UpdateProfileResult> RegistryUserAsync(User user)
        {
            if(_context.Users.SingleOrDefault(u => u.Username == user.Username) is not null)
            {
                return new UpdateProfileResult
                {
                    Success = false,
                    Message = "Имя пользователя занято",
                    User = user
                };
            }
            if (_context.Users.SingleOrDefault(u => u.Email == user.Email) is not null)
            {
                return new UpdateProfileResult
                {
                    Success = false,
                    Message = "Email уже зарегистрирован",
                    User = user
                };
            }

            user.Password = _passwordHasher.HashPassword(user, user.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UpdateProfileResult
            {
                Success = true,
                User = user
            };
        }
        public async Task<UpdateProfileResult> VerifyUserAsync(User user)
        {
            var userVer = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username || u.Email == user.Username);
            var passwordVerification = _passwordHasher.VerifyHashedPassword(
                    userVer, userVer.Password, user.Password);

            if (passwordVerification != PasswordVerificationResult.Success)
                return new UpdateProfileResult { Success = false, Message = "Current password is incorrect" };
            return new UpdateProfileResult { Success = true, User = userVer };
        }
    }
}

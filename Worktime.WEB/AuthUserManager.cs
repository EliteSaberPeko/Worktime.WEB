using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using Worktime.WEB.Controllers;
using Worktime.WEB.Models;

namespace Worktime.WEB
{
    public class AuthUserManager<TUser> : UserManager<User>
    {
        public AuthUserManager(IUserStore<User> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<User>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            PasswordHasher = new AuthPasswordHasher<User>();
        }
    }

    public class AuthPasswordHasher<TUser> : PasswordHasher<User>
    {
        public override string HashPassword(User user, string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }
            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(password);
            byte[] passwordBytes = Encoding.UTF8.GetBytes("JldfUrmthG");

            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = AES.Encrypt(bytesToBeEncrypted, passwordBytes);
            return Convert.ToBase64String(bytesEncrypted);
        }
        public override PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
        {
            if (providedPassword == null)
            {
                throw new ArgumentNullException(nameof(providedPassword));
            }
            byte[] bytesToBeDecrypted = Convert.FromBase64String(hashedPassword);
            byte[] passwordBytesdecrypt = Encoding.UTF8.GetBytes("JldfUrmthG");

            passwordBytesdecrypt = SHA256.Create().ComputeHash(passwordBytesdecrypt);

            byte[] bytesDecrypted = AES.Decrypt(bytesToBeDecrypted, passwordBytesdecrypt);
            string decryptedResult = Encoding.UTF8.GetString(bytesDecrypted);
            return providedPassword.Equals(decryptedResult) ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }
    }
    public class AuthIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DefaultError()
        {
            return new IdentityError { Code = nameof(DefaultError), Description = $"Неизвестная ошибка!" };
        }
        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError { Code = nameof(DuplicateUserName), Description = $"Такой пользователь уже существует!" };
        }
        public override IdentityError InvalidEmail(string email)
        {
            return new IdentityError { Code = nameof(InvalidEmail), Description = $"Неправильный e-mail '{email}'" };
        }
    }
}

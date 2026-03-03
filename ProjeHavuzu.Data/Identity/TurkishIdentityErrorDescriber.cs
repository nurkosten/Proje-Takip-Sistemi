using Microsoft.AspNetCore.Identity;

namespace ProjeHavuzu.Data.Identity
{
    /// <summary>
    /// ASP.NET Identity hata mesajlarını Türkçe olarak döndürür.
    /// </summary>
    public class TurkishIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateEmail(string email)
            => new() { Code = nameof(DuplicateEmail), Description = $"'{email}' e-posta adresi zaten kullanılmaktadır." };

        public override IdentityError DuplicateUserName(string userName)
            => new() { Code = nameof(DuplicateUserName), Description = $"'{userName}' kullanıcı adı zaten kullanılmaktadır." };

        public override IdentityError InvalidEmail(string? email)
            => new() { Code = nameof(InvalidEmail), Description = $"'{email}' geçerli bir e-posta adresi değildir." };

        public override IdentityError DuplicateRoleName(string role)
            => new() { Code = nameof(DuplicateRoleName), Description = $"'{role}' rol adı zaten kullanılmaktadır." };

        public override IdentityError InvalidRoleName(string? role)
            => new() { Code = nameof(InvalidRoleName), Description = $"'{role}' geçerli bir rol adı değildir." };

        public override IdentityError InvalidUserName(string? userName)
            => new() { Code = nameof(InvalidUserName), Description = $"'{userName}' geçerli bir kullanıcı adı değildir. Kullanıcı adı sadece harf ve rakam içerebilir." };

        public override IdentityError LoginAlreadyAssociated()
            => new() { Code = nameof(LoginAlreadyAssociated), Description = "Bu kullanıcı zaten bir dış giriş sağlayıcısına bağlıdır." };

        public override IdentityError InvalidToken()
            => new() { Code = nameof(InvalidToken), Description = "Geçersiz doğrulama kodu. Lütfen tekrar deneyiniz." };

        public override IdentityError UserAlreadyHasPassword()
            => new() { Code = nameof(UserAlreadyHasPassword), Description = "Kullanıcının zaten bir şifresi bulunmaktadır." };

        public override IdentityError UserAlreadyInRole(string role)
            => new() { Code = nameof(UserAlreadyInRole), Description = $"Kullanıcı zaten '{role}' rolüne atanmıştır." };

        public override IdentityError UserNotInRole(string role)
            => new() { Code = nameof(UserNotInRole), Description = $"Kullanıcı '{role}' rolünde bulunmamaktadır." };

        public override IdentityError UserLockoutNotEnabled()
            => new() { Code = nameof(UserLockoutNotEnabled), Description = "Bu kullanıcı için hesap kilitleme etkin değildir." };

        public override IdentityError RecoveryCodeRedemptionFailed()
            => new() { Code = nameof(RecoveryCodeRedemptionFailed), Description = "Kurtarma kodu doğrulaması başarısız oldu." };

        public override IdentityError ConcurrencyFailure()
            => new() { Code = nameof(ConcurrencyFailure), Description = "Eş zamanlı işlem hatası oluştu. Lütfen tekrar deneyiniz." };

        public override IdentityError DefaultError()
            => new() { Code = nameof(DefaultError), Description = "Beklenmeyen bir hata oluştu. Lütfen tekrar deneyiniz." };

        // ── Şifre Kuralları ──

        public override IdentityError PasswordMismatch()
            => new() { Code = nameof(PasswordMismatch), Description = "Girdiğiniz şifre hatalıdır." };

        public override IdentityError PasswordTooShort(int length)
            => new() { Code = nameof(PasswordTooShort), Description = $"Şifre en az {length} karakter uzunluğunda olmalıdır." };

        public override IdentityError PasswordRequiresDigit()
            => new() { Code = nameof(PasswordRequiresDigit), Description = "Şifre en az bir rakam (0-9) içermelidir." };

        public override IdentityError PasswordRequiresLower()
            => new() { Code = nameof(PasswordRequiresLower), Description = "Şifre en az bir küçük harf (a-z) içermelidir." };

        public override IdentityError PasswordRequiresUpper()
            => new() { Code = nameof(PasswordRequiresUpper), Description = "Şifre en az bir büyük harf (A-Z) içermelidir." };

        public override IdentityError PasswordRequiresNonAlphanumeric()
            => new() { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "Şifre en az bir özel karakter (!, @, #, $ vb.) içermelidir." };

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
            => new() { Code = nameof(PasswordRequiresUniqueChars), Description = $"Şifre en az {uniqueChars} farklı karakter içermelidir." };
    }
}

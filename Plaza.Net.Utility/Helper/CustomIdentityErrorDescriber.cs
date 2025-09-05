

using Microsoft.AspNetCore.Identity;

namespace Plaza.Net.Utility.Helper
{
    public class CustomIdentityErrorDescriber:IdentityErrorDescriber
    {
        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = nameof(PasswordTooShort),
                Description = $"密码至少需要{length}位"
            };
        }
        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateEmail),
                Description = $"电子邮箱{email}已存在"
            };
        }
        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = $"用户名{userName}已存在"
            };
        }
    }
}

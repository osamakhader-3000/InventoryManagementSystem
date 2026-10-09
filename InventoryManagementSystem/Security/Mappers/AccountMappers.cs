namespace SecurityApplicationPresentation.Mappers
{
    public static class AccountMappers
    {
        public static SecurityApplicationPresentation.DataTransferObjects.Account MapToAccountResource(this SecurityApplicationCore.Models.User user)
        {
            return new SecurityApplicationPresentation.DataTransferObjects.Account
            {
                Email = user.Email,
                Password = user.Password,
                Username = user.Username
            };
        }

        public static IEnumerable<SecurityApplicationPresentation.DataTransferObjects.Account> MapToAccountResource(this IEnumerable<SecurityApplicationCore.Models.User> users)
        {
            return users.Select(u => u.MapToAccountResource());
        }
        /*
        public static SecurityApplicationCore.Models.User MapToUserModel(this SecurityApplicationPresentation.DataTransferObjects.Account account)
        {
            return SecurityApplicationCore.Models.User.Create(account.Email, account.Username,  account.Password,null, SecurityApplicationCore.Models.UserAccountStatus.Temporary, SecurityApplicationCore.Models.UserEmailVerificationStatus.Unverified, Enumerable.Empty<SecurityApplicationCore.Models.Role>(), true);
        }

        public static IEnumerable<SecurityApplicationCore.Models.User> MapToUserModel(this IEnumerable<SecurityApplicationPresentation.DataTransferObjects.Account> accounts)
        {
            return accounts.Select(a => a.MapToUserModel());
        }*/
    }
}

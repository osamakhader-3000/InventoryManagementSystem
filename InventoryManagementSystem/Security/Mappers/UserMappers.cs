namespace SecurityApplicationPresentation.Mappers
{
    public static class UserMappers
    {
        public static SecurityApplicationPresentation.DataTransferObjects.User MapToResource(this SecurityApplicationCore.Models.User user)
        {
            return new SecurityApplicationPresentation.DataTransferObjects.User
            {
                ID = user.Id,
                Username = user.Username,
                Email = user.Email,
                AccountStatus = (byte)user.AccountStatus,
                EmailVerificationStatus = (byte)user.EmailVerificationStatus,
                UserRole = user.Roles.FirstOrDefault()?.Id,
                LocalAccount = user.LocalAccount
            };
        }

        public static IEnumerable<SecurityApplicationPresentation.DataTransferObjects.User> MapToResource(this IEnumerable<SecurityApplicationCore.Models.User> users)
        {
            return users.Select(MapToResource);
        }
        /*
        public static SecurityApplicationCore.Models.User MapToModel(this SecurityApplicationPresentation.DataTransferObjects.User user)
        {
            SecurityApplicationCore.Models.UserEmailVerificationStatus emailVerificationStatus = (SecurityApplicationCore.Models.UserEmailVerificationStatus)user.EmailVerificationStatus;
            SecurityApplicationCore.Models.UserAccountStatus accountStatus = (SecurityApplicationCore.Models.UserAccountStatus)user.AccountStatus;
            byte userRoleByte = user.UserRole?? (byte)(SecurityApplicationCore.Models.UserRole.Employee);
            SecurityApplicationCore.Models.Role userRole = new SecurityApplicationCore.Models.Role(Guid.Empty, "employee");
            IEnumerable<SecurityApplicationCore.Models.Role> roles = new List<SecurityApplicationCore.Models.Role>
            {
                userRole
            };

            return  SecurityApplicationCore.Models.User.Create(user.ID??Guid.Empty, user.Username, user.Email, null, null, accountStatus, emailVerificationStatus, roles, user.LocalAccount);
        }

        public static IEnumerable<SecurityApplicationCore.Models.User> MapToModel(this IEnumerable<SecurityApplicationPresentation.DataTransferObjects.User> users)
        {
            return users.Select(MapToModel);
        }*/
    }
}

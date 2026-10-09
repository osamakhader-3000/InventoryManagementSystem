using SecurityApplicationCore.Models;
using SecurityApplicationLogic.Interfaces;
using SecurityApplicationLogic.IRepositories;
using SecurityDomain.Enums.Errors;
using SecurityDomain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SecurityApplicationLogic.Features.Users.RegisterUser
{
    public class RegisterLocalUserUseCase
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IUserValidationService _userValidationService;
        private readonly IRoleService _roleService;

        public RegisterLocalUserUseCase(IUsersRepository usersRepository, IUserValidationService userValidationService, IRoleService roleService) {
            _usersRepository = usersRepository;
            _userValidationService = userValidationService;
            _roleService = roleService;
        }

        public async Task<Result<Boolean, UserError>> Validate(RegisterLocalUserRequest request)
        {
            List<User> users = await _usersRepository.ListUsersByUsernameOrEmail(request.UserName, request.Email);

            if (!_userValidationService.IsUserNameValid(request.UserName))
            {
                return Result<Boolean, UserError>.Failure(UserError.InvalidUsernameFormat, request.UserName);
            }
           

            if (!_userValidationService.IsPasswordValid(request.Password))
            {
                return Result<Boolean, UserError>.Failure(UserError.InvalidPasswordFormat, request.Password);
            }


            if (users.Any(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return Result<Boolean, UserError>.Failure(UserError.EmailAlreadyExists, request.Email);

            }

            if (users.Any(u => u.Username != null && u.Username.Equals(request.UserName, StringComparison.OrdinalIgnoreCase)))
            {
                return Result<Boolean, UserError>.Failure(UserError.UsernameAlreadyExists, request.UserName);
            }

            return Result<Boolean, UserError>.Success(true);

        }
        /*
        public async Task<Result<Guid, UserError>> Execute(RegisterLocalUserRequest request)
        {
            Result<Boolean, UserError> validationResult = await Validate(request);

            if (!validationResult.IsSuccess)             
                return Result<Guid, UserError>.Failure(validationResult.Error ?? UserError.UnexpectedError, validationResult.Message);
            
            User user = User.Create(request.Email,request.UserName, request.Password, true, UserEmailVerificationStatus.Unverified, UserAccountStatus.Temporary);

            if (user.IsUserMissingRole())
            {
                Role? role = await _roleService.GetDefaultRole();
                if (role != null)
                    user.AddRole(role);
            }

            Guid id = await CreateNewLocalUser(user);

            return user;
        }*/
    }
}

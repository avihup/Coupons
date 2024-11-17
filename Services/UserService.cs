using AutoMapper;
using Microsoft.AspNetCore.Identity;
using TestCase.Exceptions;
using TestCase.Interfaces.Repositories;
using TestCase.Interfaces.Services;
using TestCase.Models.Database;
using TestCase.Models.ViewModels;
using TestCase.Repositories;

namespace TestCase.Services
{
    public class UserService : IUserService
    {
        private readonly IUsersRespository _usersRepository;
        private readonly IMapper _mapper;

        public UserService(IUsersRespository usersRepository, IMapper mapper)
        {
            _usersRepository = usersRepository;
            _mapper = mapper;
        }

        public async Task<UserViewModel> GetByIdAsync(Guid id, Guid? clientId = null)
        {
            var user = await _usersRepository.GetByIdAsync(id, clientId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id {id} not found");
            }
            return _mapper.Map<UserViewModel>(user);
        }

        public async Task<IEnumerable<UserViewModel>> GetAllAsync(Guid? clientId = null)
        {
            var couponsDb = await _usersRepository.GetAllAsync(clientId);
            return _mapper.Map<IEnumerable<UserViewModel>>(couponsDb);
        }

        public async Task<UserViewModel> CreateAsync(CreateUserRequest request)
        {
            // Check if username already exists
            var existingUser = await _usersRepository.GetUserNameAsync(request.UserName);
            if (existingUser != null)
            {
                throw new ValidationException("Username already exists");
            }
            try
            {
                var user = new UserDto
                {
                    UserName = request.UserName,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    ClientId = request.ClientId
                };

                var userDb = await _usersRepository.CreateAsync(user);
                return _mapper.Map<UserViewModel>(userDb);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
            {
                throw new ValidationException($"A user with name '{request.UserName}' already exists.");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create user. Please try again.", ex);
            }
        }
    }
}

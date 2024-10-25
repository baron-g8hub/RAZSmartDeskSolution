using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using RAZSmartDesk.BusinessLogic.Services.IServices;
using RAZSmartDesk.DataAccess.Repositories;
using RAZSmartDesk.DataAccess.Repositories.IRepositories;
using RAZSmartDesk.Entities;

namespace RAZSmartDesk.BusinessLogic.Services
{
    public class AppUserService : IAppUserService
    {
        private readonly IAppUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AppUserService> _logger;

        public AppUserService(IAppUserRepository userRepository, IMapper mapper, ILogger<AppUserService> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<AppUser>> GetAppUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            var usersToReturn = await _userRepository.GetListAsync(cancellationToken: cancellationToken);
            _logger.LogInformation("List of {Count} users has been returned", usersToReturn.Count);

            return _mapper.Map<List<AppUser>>(usersToReturn);
        }

        public Task<List<AppUser>> GetAppUsersAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<AppUser> AddAppUserAsync(AppUser model)
        {
            throw new NotImplementedException();
        }

        public Task<AppUser> UpdateAppUserAsync(AppUser model)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAppUserAsync(int userId)
        {
            var userToDelete = await _userRepository.GetAsync(x => x.AppUsersId == userId);

            if (userToDelete is null)
            {
                _logger.LogError("User with userId = {AppUserId} was not found", userId);
                //throw new UserNotFoundException();
            }

            await _userRepository.RemoveAsync(userToDelete);
        }
    }
}

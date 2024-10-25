using RAZSmartDesk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAZSmartDesk.BusinessLogic.Services.IServices
{
    public interface IAppUserService
    {
        Task<List<AppUser>> GetAppUsersAsync(CancellationToken cancellationToken = default);
        Task<AppUser> GetAppUserAsync(int userId, CancellationToken cancellationToken = default);
        Task<AppUser> AddAppUserAsync(AppUser model);
        Task<AppUser> UpdateAppUserAsync(AppUser model);
        Task DeleteAppUserAsync(int userId);
    }
}

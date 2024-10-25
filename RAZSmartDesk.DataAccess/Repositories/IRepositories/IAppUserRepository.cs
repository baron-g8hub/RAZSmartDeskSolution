using RAZSmartDesk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAZSmartDesk.DataAccess.Repositories.IRepositories
{
    public interface IAppUserRepository 
    {
        Task<IEnumerable<AppUser>> GetAppUsersByCompanyIdAsync(int companyId);

        Task<AppUser?> FindByAppUserIdAsync(int id);

        Task<AppUser> AddAsync(AppUser model);

        Task<AppUser> UpdateAsync(AppUser model);

        Task<AppUser> RemoveAsync(AppUser model);
    }
}

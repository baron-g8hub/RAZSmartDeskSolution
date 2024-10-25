using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using RAZSmartDesk.DataAccess.DapperDBContext;
using RAZSmartDesk.DataAccess.Repositories.IRepositories;
using RAZSmartDesk.Entities;

namespace RAZSmartDesk.DataAccess.Repositories
{
    public class AppUserRepository : IAppUserRepository
    {

        public Task<AppUser> FindAsync(int uid)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AppUser>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AppUser> AddAsync(AppUser model)
        {
            throw new NotImplementedException();
        }

        public Task<AppUser> RemoveAsync(AppUser model)
        {
            throw new NotImplementedException();
        }

        public Task<AppUser> UpdateAsync(AppUser model)
        {
            throw new NotImplementedException();
        }
    }
}

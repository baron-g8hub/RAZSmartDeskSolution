using System;
using System.Collections;
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
        private readonly DapperDbContext _context;

        public AppUserRepository(DapperDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AppUser>> GetAppUsersByCompanyIdAsync(int companyId)
        {
            const string query = "SELECT * FROM AppUsers WHERE CompanyEmployeeId = @companyId ";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<AppUser>(query, new { companyId });
        }

        public async Task<AppUser?> FindByAppUserIdAsync(int id)
        {
            const string query = "SELECT * FROM [AppUsers] WHERE AppUserId=@id";

            using var connection = _context.CreateConnection();
            var result = await connection.QuerySingleOrDefaultAsync<AppUser>(query, new { id });
            return result;
        }


        // TODO: Apply Stored Procedure when Adding multiple users
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

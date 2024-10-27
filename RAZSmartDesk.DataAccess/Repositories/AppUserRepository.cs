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

        public async Task<IEnumerable<User>> GetAppUsersByCompanyIdAsync(int companyId)
        {
            const string query = "SELECT * FROM AppUsers WHERE CompanyEmployeeId = @companyId ";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<User>(query, new { companyId });
        }

        public async Task<User?> FindByAppUserIdAsync(int id)
        {
            const string query = "SELECT * FROM [AppUsers] WHERE AppUserId=@id";

            using var connection = _context.CreateConnection();
            var result = await connection.QuerySingleOrDefaultAsync<User>(query, new { id });
            return result;
        }

        public async Task<User?> FindByUsernamePasswordAsync(string username, string password)
        {
            const string query = "SELECT * FROM [AppUsers] WHERE Username=@username AND Password=@password";

            using var connection = _context.CreateConnection();
            var result = await connection.QuerySingleOrDefaultAsync<User>(query, new { username, password });
            return result;
        }

        // TODO: Apply Stored Procedure when Adding multiple users
        public Task<User> AddAsync(User model)
        {
            throw new NotImplementedException();
        }

        public Task<User> RemoveAsync(User model)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateAsync(User model)
        {
            throw new NotImplementedException();
        }

      
    }
}

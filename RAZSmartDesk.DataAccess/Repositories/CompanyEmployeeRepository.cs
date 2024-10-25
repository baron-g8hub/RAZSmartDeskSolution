using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using RAZSmartDesk.DataAccess.DapperDBContext;
using RAZSmartDesk.DataAccess.Repositories.IRepositories;
using RAZSmartDesk.Entities;

namespace RAZSmartDesk.DataAccess.Repositories
{
    public class CompanyEmployeeRepository : ICompanyEmployees
    {
        private readonly DapperDbContext context;

        public CompanyEmployeeRepository(DapperDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<CompanyEmployee>> GetAsync()
        {
            var sql = $@"SELECT * FROM 
                               [CompanyEmployees]";

            using var connection = context.CreateConnection();
            return await connection.QueryAsync<CompanyEmployee>(sql);
        }



        public Task<CompanyEmployee> FindAsync(Guid uid)
        {
            throw new NotImplementedException();
        }

     
        public Task<CompanyEmployee> AddAsync(CompanyEmployee model)
        {
            throw new NotImplementedException();
        }

        public Task<CompanyEmployee> RemoveAsync(CompanyEmployee model)
        {
            throw new NotImplementedException();
        }

        public Task<CompanyEmployee> UpdateAsync(CompanyEmployee model)
        {
            throw new NotImplementedException();
        }
    }
}

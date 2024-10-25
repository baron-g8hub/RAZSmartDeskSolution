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
    public class CompanyRepository : ICompanyRepository
    {
        private readonly DapperDbContext context;

        public CompanyRepository(DapperDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Company>> GetAsync()
        {
            var sql = $@"SELECT *
                            FROM 
                               [Companies]";

            using var connection = context.CreateConnection();
            return await connection.QueryAsync<Company>(sql);
        }

        public Task<Company> FindAsync(Guid uid)
        {
            throw new NotImplementedException();
        }

    

        public Task<Company> AddAsync(Company model)
        {
            throw new NotImplementedException();
        }

        public Task<Company> RemoveAsync(Company model)
        {
            throw new NotImplementedException();
        }

        public Task<Company> UpdateAsync(Company model)
        {
            throw new NotImplementedException();
        }
    }
}

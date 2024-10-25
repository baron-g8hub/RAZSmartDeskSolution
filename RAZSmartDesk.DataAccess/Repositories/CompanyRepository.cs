using Dapper;
using RAZSmartDesk.DataAccess.DapperDBContext;
using RAZSmartDesk.DataAccess.Repositories.IRepositories;
using RAZSmartDesk.Entities;

namespace RAZSmartDesk.DataAccess.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly DapperDbContext _context;

        public CompanyRepository(DapperDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Company>> GetAsync()
        {
            const string query = "SELECT * FROM [Companies]";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Company>(query);
        }

        public async Task<Company?> FindAsync(int? id)
        {
            const string query = "SELECT  FROM [Companies] WHERE Companies [CompanyId]=@id";

            using var connection = _context.CreateConnection();
            var result = await connection.QuerySingleOrDefaultAsync<Company>(query, new { id });
            return result;
        }

        public async Task<Company> AddAsync(Company model)
        {
            model.CreatedDate = DateTime.Now;
            model.UpdatedDate = model.CreatedDate;
            const string query = @"INSERT INTO [dbo].[Companies]
                                ([CompanyName],
                                 [CompanyCode],
                                 [CompanyDescription],
                                 [IsActive],
                                 [CreatedBy],
                                 [CreatedDate],
                                 [UpdatedBy],
                                 [UpdatedDate])
                                VALUES
                                (@CompanyName,
                                 @CompanyCode,
                                 @CompanyDescription,
                                 @IsActive,
                                 @CreatedBy,
                                 @CreatedDate,
                                 @UpdatedBy,
                                 @UpdatedDate)";

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(query, model);
            return model;
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

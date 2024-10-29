using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
    public class UserRepository : IAppUserRepository
    {
        private readonly DapperDbContext _context;

        public UserRepository(DapperDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User?>> GetAppUsersByCompanyIdAsync(int companyId, int userTypeId)
        {
            using var connection = _context.CreateConnection();
            if (userTypeId == 1)
            {
                const string query = @"SELECT a.[UserId]
                                          ,a.Username
	                                      ,a.Password
                                          ,a.UserTypeId
	                                      ,b.CompanyName
	                                      ,c.UserTypeName
                                          ,a.[IsActive]
                                          ,a.[CreatedBy]
                                          ,a.[CreatedDate]
                                          ,a.[UpdatedBy]
                                          ,a.[UpdatedDate]
                                      FROM [Users] as a
                                      INNER JOIN Companies as b
                                      ON a.UserCompanyId = b.CompanyId 
                                      INNER JOIN UserTypes as c
                                      ON  a.UserTypeId = c.UserTypeId 
                                      WHERE a.UserCompanyId = @companyId";
                return await connection.QueryAsync<User>(query, new { companyId });
            }
            else
            {
                const string query = @"SELECT a.[UserId]
                                          ,a.Username
	                                      ,a.Password
                                          ,a.UserTypeId
	                                      ,b.CompanyName
	                                      ,c.UserTypeName
                                          ,a.[IsActive]
                                          ,a.[CreatedBy]
                                          ,a.[CreatedDate]
                                          ,a.[UpdatedBy]
                                          ,a.[UpdatedDate]
                                      FROM [Users] as a
                                      INNER JOIN Companies as b
                                      ON a.UserCompanyId = b.CompanyId 
                                      INNER JOIN UserTypes as c
                                      ON  a.UserTypeId = c.UserTypeId 
                                      WHERE a.UserCompanyId = @companyId AND a.UserTypeId = @userTypeId";
                return await connection.QueryAsync<User>(query, new { companyId, userTypeId });
            }
        }

        public async Task<User?> FindByUserIdCompanyIdAsync(int id, int companyId)
        {
            const string query = @"SELECT a.[UserId]
                                          ,a.UserTypeId
                                          ,a.Username
	                                      ,a.Password
	                                      ,b.CompanyName
                                          ,a.UserCompanyId
	                                      ,c.UserTypeName
                                          ,a.[IsActive]
                                          ,a.[CreatedBy]
                                          ,a.[CreatedDate]
                                          ,a.[UpdatedBy]
                                          ,a.[UpdatedDate]
                                      FROM [Users] as a
                                      INNER JOIN Companies as b
                                      ON a.UserCompanyId = b.CompanyId 
                                      INNER JOIN UserTypes as c
                                      ON  a.UserTypeId = c.UserTypeId
                                      WHERE a.UserId=@id AND a.UserCompanyId = @companyId";

            using var connection = _context.CreateConnection();
            var result = await connection.QuerySingleOrDefaultAsync<User>(query, new { id, companyId });
            return result;
        }

        public async Task<User?> FindByUserIdAsync(int id)
        {
            const string query = @"SELECT a.[UserId]
                                          ,a.UserTypeId
                                          ,a.Username
	                                      ,a.Password
	                                      ,b.CompanyName
                                          ,a.UserCompanyId
	                                      ,c.UserTypeName
                                          ,a.[IsActive]
                                          ,a.[CreatedBy]
                                          ,a.[CreatedDate]
                                          ,a.[UpdatedBy]
                                          ,a.[UpdatedDate]
                                      FROM [Users] as a
                                      INNER JOIN Companies as b
                                      ON a.UserCompanyId = b.CompanyId 
                                      INNER JOIN UserTypes as c
                                      ON  a.UserTypeId = c.UserTypeId
                                      WHERE a.UserId=@id";

            using var connection = _context.CreateConnection();
            var result = await connection.QuerySingleOrDefaultAsync<User>(query, new { id });
            return result;
        }

        public async Task<User?> FindByUsernamePasswordAsync(string username, string password)
        {
            const string query = @"SELECT Users.UserId, 
		                                  Users.Password, 
		                                  Users.UserTypeId,
		                                  Users.UserCompanyId,
		                                  Users.IsActive,
		                                  Users.CreatedBy,
		                                  Users.CreatedDate,
		                                  Users.UpdatedBy,
		                                  Users.UpdatedDate,
		                                  UserTypes.UserTypeName,
		                                  UserTypes.UserTypeLevel
		                                    FROM [Users] 
		                                    INNER JOIN UserTypes ON Users.UserTypeId = UserTypes.UserTypeId 
                                            WHERE Users.Username=@username AND Users.Password=@password";

            using var connection = _context.CreateConnection();
            var result = await connection.QuerySingleOrDefaultAsync<User>(query, new { username, password });
            return result;
        }

        // TODO: Apply Stored Procedure when Adding multiple users
        public async Task<User> AddAsync(User model)
        {
            const string query = @"INSERT INTO [dbo].[Users]  ([UserCompanyId]
                                                   ,[UserTypeId]
                                                   ,[Username]
                                                   ,[Password]
                                                   ,[IsActive]
                                                   ,[CreatedBy]
                                                   ,[CreatedDate]
                                                   ,[UpdatedBy]
                                                   ,[UpdatedDate])
                                             VALUES
                                                   (@UserCompanyId
                                                   ,@UserTypeId
                                                   ,@Username
                                                   ,@Password
                                                   ,@IsActive
                                                   ,@CreatedBy
                                                   ,@CreatedDate
                                                   ,@UpdatedBy
                                                   ,@UpdatedDate)";

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(query, model);
            return model;
        }



        public async Task<User> UpdateAsync(User model)
        {
            model.UpdatedDate = DateTime.UtcNow;
            const string query = @"UPDATE [dbo].[Users] SET UserTypeId = @UserTypeId
                                                  ,Username = @Username
                                                  ,Password = @Password
                                                  ,IsActive = @IsActive
                                                  ,UpdatedBy = @UpdatedBy
                                                  ,UpdatedDate = GetUTCDate()
                                             WHERE UserId = @UserId";

            var parameters = new DynamicParameters();
            parameters.Add("UserId", model.UserId, DbType.Int32);
            parameters.Add("UserTypeId", model.UserTypeId, DbType.Int32);
            parameters.Add("Username", model.Username, DbType.String);
            parameters.Add("Password", model.Password, DbType.String);
            parameters.Add("UpdatedBy", model.UpdatedBy, DbType.String);
            parameters.Add("IsActive", model.IsActive, DbType.Boolean);

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(query, parameters);
            return model;
        }

        public async Task<User> RemoveAsync(User model)
        {
            var id = model.UserId;

            const string query = @"DELETE FROM [dbo].[Users] WHERE UserId=@id";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { id });
            }
            return model;
        }

    }
}

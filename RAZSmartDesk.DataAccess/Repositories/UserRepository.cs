﻿using System;
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
    public class UserRepository : IAppUserRepository
    {
        private readonly DapperDbContext _context;

        public UserRepository(DapperDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAppUsersByCompanyIdAsync(int companyId, int userTypeId)
        {
            using var connection = _context.CreateConnection();
            if (userTypeId == 1)
            {
                const string query = @"SELECT a.[UserId]
                                          ,a.Username
	                                      ,a.Password
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

        public async Task<User?> FindByUserIdAsync(int id)
        {
            const string query = "SELECT * FROM [Users] WHERE UserId=@id";

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

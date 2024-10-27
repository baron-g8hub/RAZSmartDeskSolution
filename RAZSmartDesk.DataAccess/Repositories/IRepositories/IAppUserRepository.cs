﻿using RAZSmartDesk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAZSmartDesk.DataAccess.Repositories.IRepositories
{
    public interface IAppUserRepository 
    {
        Task<IEnumerable<User>> GetAppUsersByCompanyIdAsync(int companyId);

        Task<User?> FindByAppUserIdAsync(int id);

        Task<User?> FindByUsernamePasswordAsync(string username, string password);

        Task<User> AddAsync(User model);

        Task<User> UpdateAsync(User model);

        Task<User> RemoveAsync(User model);
    }
}

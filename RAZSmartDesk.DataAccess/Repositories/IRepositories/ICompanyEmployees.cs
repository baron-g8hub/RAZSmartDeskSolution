using RAZSmartDesk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAZSmartDesk.DataAccess.Repositories.IRepositories
{
    public interface ICompanyEmployees
    {
        Task<IEnumerable<CompanyEmployee>> GetAsync();

        Task<CompanyEmployee> FindAsync(Guid uid);

        Task<CompanyEmployee> AddAsync(CompanyEmployee model);

        Task<CompanyEmployee> UpdateAsync(CompanyEmployee model);

        Task<CompanyEmployee> RemoveAsync(CompanyEmployee model);
    }
}

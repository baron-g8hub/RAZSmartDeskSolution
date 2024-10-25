using RAZSmartDesk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAZSmartDesk.DataAccess.Repositories.IRepositories
{
    public interface ICompanyRepository
    {
        Task<IEnumerable<Company>> GetAsync();

        Task<Company?> FindAsync(int? id);

        Task<Company> AddAsync(Company model);

        Task<Company> UpdateAsync(Company model);

        Task<Company> RemoveAsync(Company model);
    }
}

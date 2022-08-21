using System.Threading.Tasks;
using Veterinary.Domain.Models;
using Veterinary.Domain.Types;

namespace Veterinary.Services.EmployeeServices
{
    public interface IEmployeeService
    {
         Task<HttpListResponse<EmployeeProfile>> GetAllAsync(int offset, int limit);

         Task<EmployeeProfile> GetAsync();

         Task<EmployeeProfile> GetByIdAsync(string id);

         Task<EmployeeProfile> UpdateByIdAsync(string id, UpdateEmployeeProfileDto employeeProfile);
    }
}

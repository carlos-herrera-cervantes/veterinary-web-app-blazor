using System.Collections.Generic;
using System.Threading.Tasks;
using Veterinary.Domain.Models;

namespace Veterinary.Services.EmployeeServices
{
    public interface IEmployeeService
    {
         Task<IEnumerable<Employee>> GetAllAsync();

         Task<Employee> GetByIdAsync(string id);

         Task<Employee> CreateAsync(Employee employee);

         Task<Employee> UpdateByIdAsync(string id, Employee employee);

         Task DeleteByIdAsync(string id);
    }
}
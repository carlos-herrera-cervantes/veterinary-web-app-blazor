using System.Threading.Tasks;
using Veterinary.Domain.Models;

namespace Veterinary.Services.EmployeeServices
{
    public interface IEmployeeService
    {
         Task<Employee> CreateAsync(Employee employee);
    }
}
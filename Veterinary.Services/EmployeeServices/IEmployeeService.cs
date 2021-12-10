using System.Threading.Tasks;
using Veterinary.Domain.Models;

namespace Veterinary.Services.EmployeeServices
{
    public interface IEmployeeService
    {
         Task<ListEmployeeResponseDto> GetAllAsync();

         Task<SingleEmployeeResponseDto> GetByIdAsync(string id);

         Task<SingleEmployeeResponseDto> CreateAsync(Employee employee);

         Task<SingleEmployeeResponseDto> UpdateByIdAsync(string id, Employee employee);

         Task DeleteByIdAsync(string id);
    }
}
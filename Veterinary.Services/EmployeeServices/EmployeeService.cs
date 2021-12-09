using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Veterinary.Domain.Models;

namespace Veterinary.Services.EmployeeServices
{
    public class EmployeeService : IEmployeeService
    {
        private IEnumerable<Employee> _employees = new List<Employee>
        {
            new Employee
            {
                Name = "Carlos",
                LastName = "Herrera",
                Gender = "Male",
                PhoneNumber = "5215513909810",
                Birthdate = "1994-11-08",
                Municipality = "Acapulco",
                PostalCode = "39550",
                Street = "2",
                Colony = "Bella Vista",
                Number = "25"
            },
            new Employee
            {
                Name = "Isela",
                LastName = "Ortíz",
                Gender = "Female",
                PhoneNumber = "5217441467274",
                Birthdate = "2000-11-19",
                Municipality = "Acapulco",
                PostalCode = "39550",
                Street = "5",
                Colony = "Bella Vista",
                Number = "78"
            },
            new Employee
            {
                Name = "Teresa",
                LastName = "Herrera",
                Gender = "Female",
                PhoneNumber = "5217444767174",
                Birthdate = "1995-04-24",
                Municipality = "Acapulco",
                PostalCode = "39550",
                Street = "150",
                Colony = "Pedregoso",
                Number = "108"
            }
        };

        public async Task<IEnumerable<Employee>> GetAllAsync()
            => await Task.FromResult(_employees);

        public async Task<Employee> GetByIdAsync(string id)
            => await Task.FromResult(_employees.Where(e => e.Name == id).FirstOrDefault());

        public async Task<Employee> CreateAsync(Employee employee)
        {
            await Task.FromResult(_employees.Append(employee));
            return employee;
        }

        public async Task<Employee> UpdateByIdAsync(string id, Employee employee)
        {
            await Task.FromResult(_employees.Append(employee));
            return employee;
        }

        public async Task DeleteByIdAsync(string id)
        {
            _employees = await Task.FromResult(_employees.Where(e => e.Name != id));

            foreach (var employee in _employees)
            {
                Console.WriteLine(employee.Name);
            }
        }
    }
}
using System.Threading.Tasks;
using Veterinary.Domain.Models;
using Veterinary.Domain.Types;

namespace Veterinary.Services.CustomerServices;

public interface ICustomerProfileService
{
    Task<HttpListResponse<CustomerProfile>> GetAllAsync(int offset, int limit);

    Task<CustomerProfile> GetByIdAsync(string id);
}

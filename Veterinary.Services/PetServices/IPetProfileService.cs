using System.Threading.Tasks;
using Veterinary.Domain.Models;
using Veterinary.Domain.Types;

namespace Veterinary.Services.PetServices;

public interface IPetProfileService
{
    Task<HttpListResponse<PetProfile>> GetByCustomerIdAsync(string customerId);

    Task<HttpListResponse<PetProfile>> GetAllAsync();

    Task<PetProfile> GetByIdAsync(string id);
}

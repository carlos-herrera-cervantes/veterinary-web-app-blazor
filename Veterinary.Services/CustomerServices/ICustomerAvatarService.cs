using System.Threading.Tasks;
using Veterinary.Domain.Models;

namespace Veterinary.Services.CustomerServices;

public interface ICustomerAvatarService
{
    Task<CustomerAvatar> GetByIdAsync(string id);
}

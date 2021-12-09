using System.Threading.Tasks;
using Veterinary.Domain.Models;

namespace Veterinary.Services.AuthServices
{
    public interface IAuthService
    {
        Task<AuthResponseDto> SignInAsync(Credentials credentials);

        Task SignOutAsync();
    }
}

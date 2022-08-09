using System.Threading.Tasks;
using Veterinary.Domain.Models;
using Veterinary.Domain.Types;

namespace Veterinary.Services.AuthServices
{
    public interface IAuthService
    {
        Task<HttpMessageResponse> SignInAsync(Credentials credentials);

        Task SignOutAsync();

        Task<HttpMessageResponse> SignupEmployeeAsync(Credentials credentials);
    }
}

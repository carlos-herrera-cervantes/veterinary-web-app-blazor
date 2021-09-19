using System.Threading.Tasks;
using Veterinary.Domain.Models;

namespace Veterinary.Services.AuthServices
{
    public interface IAuthService
    {
        Task<string> SignInAsync(Credentials credentials);
    }
}

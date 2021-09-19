using System.Threading.Tasks;
using Veterinary.Domain.Models;

namespace Veterinary.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        /// <summary>
        /// Authenticates an employee
        /// </summary>
        /// <param name="credentials">Employee number and password</param>
        /// <returns>String</returns>
        public async Task<string> SignInAsync(Credentials credentials)
        {
            return await Task.FromResult<string>("fake-token");
        }
    }
}

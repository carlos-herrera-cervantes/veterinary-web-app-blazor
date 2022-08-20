using System.Net.Http;
using System.Threading.Tasks;
using Veterinary.Domain.Models;

namespace Veterinary.Services.AuthServices;

public interface IAvatarService
{
    Task<Avatar> GetAsync();

    Task<Avatar> Upload(MultipartFormDataContent content);
}

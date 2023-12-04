using Mango.Services.EmailAPI.Models.Dto;

namespace Mango.Services.EmailAPI.Services
{
    public interface IEmailService
    {
        Task emailCartAndLog(CartDto cartDto);
        Task RegisteredUserEmailAndLog(string email);
    }
}

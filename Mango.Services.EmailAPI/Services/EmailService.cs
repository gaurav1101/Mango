using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Models.Dto;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Text;

namespace Mango.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly DbContextOptions<ApplicationDBContext> _applicationDBContext;
        public EmailService(DbContextOptions<ApplicationDBContext> applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }
        public async Task emailCartAndLog(CartDto cartDto)
        {
            StringBuilder message=new StringBuilder();
            message.AppendLine("<br/> Email Cart Requested");
            message.AppendLine("<br/> Total" + cartDto.CartHeaderDto.CartTotal);
            message.AppendLine("<br/>");
            message.AppendLine("<ul>");
            foreach(var item in cartDto.CartDetailsDtos)
            {
                message.AppendLine("<li>");
                message.AppendLine(item.Product.Name + " - " + item.Count);
                message.AppendLine("</li>");
            }
            message.AppendLine("</ul>");
            await LogAndEmail(message.ToString(),cartDto.CartHeaderDto.Email);
        }

        public async Task RegisteredUserEmailAndLog(string email)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("<br/> New User Created " + email);
          
            await LogAndEmail(message.ToString(),"admin@gmail.com");
        }
        private async Task<bool> LogAndEmail(string message, string email)
        {
            try
            {
                EmailLogger emailLogger = new EmailLogger
                {
                    Email = email,
                    Message = message,
                    EmailSent = DateTime.Now,
                };

                using (var dbContext = new ApplicationDBContext(_applicationDBContext))
                {
                    // Assuming DataModels is a DbSet in your ApplicationDBContext
                    await dbContext.EmailLoggers.AddAsync(emailLogger);
                    await dbContext.SaveChangesAsync();
                }
                return true;
            }
            catch 
            {     
                return false;
            }
        } 
    }
}

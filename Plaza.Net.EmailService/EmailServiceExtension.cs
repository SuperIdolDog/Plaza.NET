using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.EmailService
{
    public static class EmailServiceExtension
    {
        public static void AddEmailService(this IServiceCollection services,IConfiguration configuration)
        {
            services.Configure<FromEmailConfig>(configuration);
            services.AddScoped<IEmailSender,EmailSender>();
        }
    }
}

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Auth
{
    public class EmailConfirmationTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
    {
        //public EmailConfirmationTokenProvider
        //    (
        //    IDataProtectionProvider dataProtectionProvider,
        //    IOptions<EmailConfirmationTokenProviderOptions> options,
        //    ILogger<DataProtectorTokenProvider<TUser>> logger
        //    )
        //    : base(dataProtectionProvider, options, logger)
        //{

        //}
        //public class EmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
        //{

        //}
        //public EmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider, IOptions<DataProtectionTokenProviderOptions> options) : base(dataProtectionProvider, options)
        //{
        //}
        public EmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider, IOptions<DataProtectionTokenProviderOptions> options, ILogger<DataProtectorTokenProvider<TUser>> logger) : base(dataProtectionProvider, options, logger)
        {
        }
    }
}

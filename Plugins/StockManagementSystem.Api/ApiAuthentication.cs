using System.Collections.Generic;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StockManagementSystem.Api.Helpers;
using StockManagementSystem.Services.Authentication;

namespace StockManagementSystem.Api
{
    public class ApiAuthentication : IExternalAuthenticationRegistrar
    {
        public void Configure(AuthenticationBuilder builder)
        {
            RsaSecurityKey signingKey = CryptoHelper.CreateRsaSecurityKey();

            builder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Audience = "sms_api";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateActor = false,
                    ValidateIssuer = false,
                    NameClaimType = JwtClaimTypes.Name,
                    RoleClaimType = JwtClaimTypes.Role,
                    IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                        new List<RsaSecurityKey> {signingKey}
                    // Uncomment this if you are using an certificate to sign your tokens.
                    // IssuerSigningKey = new X509SecurityKey(cert),
                };
            });
        }
    }
}
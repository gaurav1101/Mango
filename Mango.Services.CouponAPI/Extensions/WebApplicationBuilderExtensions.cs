using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mango.Services.CouponAPI.Extensions
{ 
	public static class WebApplicationBuilderExtensions
	{
		public static WebApplicationBuilder AddAppAuthentication(this WebApplicationBuilder builder)
		{
			//changes to be for to implement Authentication of any API
			var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");
			var issuer = builder.Configuration.GetValue<string>("ApiSettings:Issuer");
			var audience = builder.Configuration.GetValue<string>("ApiSettings:Audience");

			builder.Services.AddAuthentication(x =>
			{
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
				.AddJwtBearer(x => {
					x.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
						ValidateIssuer = true,
						ValidIssuer = issuer,
						ValidateAudience = true,
						ValidAudience = audience
					};
				});
			return builder;
		}
	}
}

#if (EnableJsonWebToken)
using Microsoft.AspNetCore.Authentication.JwtBearer;
#endif
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
#if (EnableSwagger)
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
#endif
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Data.SqlClient;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(option =>
            {
                // Add Default Cross Domain
                option.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
                });
            });

            services.AddControllers();

            //Swagger docs
#if (EnableSwagger)
      services.AddSwaggerGen(c =>
        {
          c.SwaggerDoc("v1", new OpenApiInfo
          {
            Title = "Web API",
            Version = "v1",
            Description = "Dotnet Core 3.1",
          });
          var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
          var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
          c.IncludeXmlComments(xmlPath);
        }
      );
#endif
#if (EnableJsonWebToken)
      // Jwt Token
      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
      {
        option.TokenValidationParameters = new TokenValidationParameters
        {
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
          ValidIssuer = Configuration["Jwt:Issue"],
          ValidateIssuer = true,
          ValidateAudience = false,
          ValidateIssuerSigningKey = true, // validate signingkey
          ValidateLifetime = true, // validate datetime 
        };
      });
#endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
#if (EnableSwagger)
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web Api V1");
                c.RoutePrefix = string.Empty;
            });
#endif

            app.UseRouting();
            // Cors Middleware must be between Routing and Endpoints.
            app.UseCors();
            // Authentication before authorization
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
          {
              endpoints.MapControllers();
          });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using payment_gateway.Filters;
using payment_gateway.Model;
using payment_gateway.Services;
using payment_gateway.Services.Engine;
using payment_gateway_repository.Engine.Base;
using payment_gateway_repository.Engine.Model;
using payment_gateway_repository.Engine.Repository;
using payment_gateway_repository.Repository;
using User = payment_gateway_repository.Model.User;
using UserPayment = payment_gateway_repository.Model.UserPayment;

namespace payment_gateway
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
            services.AddControllers().AddNewtonsoftJson();

            //gets the configuration from appsettings file
            //if we have the app in the cloud, the secrets can be stored in KeyVaults
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();
            var dataBase = new MongoDataBase { ClientBase = new ClientModel { ConnectionString = appSettings.ConnectionString } };
            var repoBase = new MongoRepositoryBase(dataBase);
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddMvc(opt => { opt.Filters.Add(typeof(JsonExceptionFilter)); });
            services.AddRouting(opt => opt.LowercaseUrls = true);

            //we add the authentication bearer to allow users to process actions
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddSingleton<IRepository<User>>(u => new UserRepository(repoBase));
            services.AddSingleton<IRepository<UserPayment>>(p => new UserPaymentsRepository(repoBase));

            services.AddScoped<IUserService, UserService>();

            //swagger documentation
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Payment Gateway", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment Gateway V1");
                //c.RoutePrefix = string.Empty;
            });

            app.UseRouting();
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

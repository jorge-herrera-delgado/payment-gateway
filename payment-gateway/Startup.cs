using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using payment_gateway.AssemblyRegister;
using payment_gateway.Filters;
using payment_gateway.Model;
using payment_gateway.Services;
using payment_gateway.Services.Engine;
using payment_gateway_core.Model;

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
            var appSettingsSection = Configuration.GetSection(nameof(AppSettings));
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();

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
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSettings.Secret)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            //Settings Registration
            services.RegisterSettingOptions<PaypalSettings>(Configuration);

            //Repository Registration
            services.RegisterRepositories(appSettings);

            //Actions Registration
            services.RegisterActions();

            //Validation Registration
            services.RegisterValidation();

            //User Service Registration for authentication
            services.AddSingleton<IUserService, UserService>();

            //swagger documentation.
            //It is a basic concept for the swagger documentation.
            //We can follow some steps here to setup with further details
            //https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-3.1&tabs=visual-studio

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Payment Gateway",
                    Description = "Payment Gateway test for checkout.com",
                    Contact = new OpenApiContact
                    {
                        Name = "Jorge Herrera Delgado",
                        Email = "jorge.herrera.d@outlook.com",
                        Url = new Uri("https://www.linkedin.com/in/jorge-herrera-delgado-3a1b816a/"),
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
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

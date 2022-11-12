
using CliverApi.Services.Mail;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Sheca.Error;
using Sheca.Services;
using Sheca.Services.Mail;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net.Mime;
using System.Text;
using System.Text.Json.Serialization;

namespace Sheca.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services) =>
         services.AddCors(options =>
         {
             options.AddDefaultPolicy(policy =>
             policy.SetIsOriginAllowed(origin => true)
             .AllowAnyMethod()
             .AllowAnyHeader().AllowCredentials());
         });

        public static void ConfigureRepository(this IServiceCollection services) =>
            services
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<ICourseService, CourseService>()
            .AddScoped<IEventService, EventService>()
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<IMailService, MailService>()
            .AddSingleton<IDictionary<string, string>>(_ => new Dictionary<string, string>());
        public static void ConfigureSwaggerOptions(this SwaggerGenOptions options)
        {
            options.AddSecurityDefinition("Bearer",
            new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Place to add JWT with Bearer",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                new string[]{}
                }
            });

        }
        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                var Key = Encoding.UTF8.GetBytes(configuration["JWT:Key"]);
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Key)
                };
            });
        }

        public static void AddHangFireService(this IServiceCollection services, IConfiguration Configuration)
        {
            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

            // Add the processing server as IHostedService
            services.AddHangfireServer();
        }

        public static void AddCustomOptions(this IMvcBuilder builder)
        {
            builder.AddJsonOptions(x =>
             {
                 x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                 x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
             }).ConfigureApiBehaviorOptions(options =>
             {
                 options.InvalidModelStateResponseFactory = context =>
                 {
                     var result = new ValidationFailedResult(context.ModelState);

                     // TODO: add `using System.Net.Mime;` to resolve MediaTypeNames
                     result.ContentTypes.Add(MediaTypeNames.Application.Json);
                     result.ContentTypes.Add(MediaTypeNames.Application.Xml);
                     return result;
                 };
             });
        }
    }
}

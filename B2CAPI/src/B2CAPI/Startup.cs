using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace B2CAPI
{
    public class Startup
    {
        public static string SignUpPolicyId;
        public static string SignInPolicyId;
        public static string ProfilePolicyId;
        public static string ClientId;
        public static string AadInstance;
        public static string Tenant;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            // Add Authentication services.
            services.AddAuthentication(
                sharedOptions => sharedOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Configure the OWIN pipeline to use cookie auth.
            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            // App config settings
            ClientId = Configuration["AzureAD:ClientId"];
            AadInstance = Configuration["AzureAD:AadInstance"];
            Tenant = Configuration["AzureAD:Tenant"];

            // B2C policy identifiers
            SignInPolicyId = Configuration["AzureAD:SignInPolicyId"];


            var options = new JwtBearerOptions
            {
                MetadataAddress = string.Format(AadInstance, Tenant, SignInPolicyId),
                AuthenticationScheme = SignInPolicyId,
                Audience = ClientId,
                AutomaticAuthenticate = true,
                RequireHttpsMetadata = false,
                AutomaticChallenge = false,
                IncludeErrorDetails = true,
                Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        context.HttpContext.Items["jwt-workaround"] = null;

                        return Task.FromResult(0);
                    }
                }
            };
            app.UseJwtBearerAuthentication(options);
            // Configure MVC routes
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
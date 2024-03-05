using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.FraudLabsPro.Factories;
using Nop.Plugin.Misc.FraudLabsPro.Services;

namespace Nop.Plugin.Misc.FraudLabsPro.Infrastructure
{

    /// <summary>
    /// Represents a FraudLabs Pro dependency registrar
    /// </summary>
    /// 

    public class DependencyRegistrar : INopStartup
    {
        public void Configure(IApplicationBuilder application)
        {

        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //register custom services
            services.AddScoped<FraudLabsProManager>();

            //register custom factories
            services.AddScoped<FraudLabsProOrderModelFactory>();
        }

        public int Order => 2;
    }

}

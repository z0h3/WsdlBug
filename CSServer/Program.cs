using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SoapCore;

namespace CSServer
{

    [DataContract(Namespace = "http://localhost:5050/csmodel")]
    public class CsModel
    {
        [DataMember]
        public DateTime? DateExpire { get; set; }
    }
    
    [ServiceContract(Namespace = "http://localhost:5050")]
    public interface ICsSimpleService
    {
        [OperationContract]
        public CsModel Test(string str);
    }

    public class CsSimpleService : ICsSimpleService
    {
        public CsModel Test(string str)
        {
            return new CsModel();
        }
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ICsSimpleService, CsSimpleService>();
            services.AddMvc((x) => x.EnableEndpointRouting = false);
            services.AddSoapCore();
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env, ILoggerFactory log)
        {
            app.UseSoapEndpoint<ICsSimpleService>("/SimpleService.svc", new BasicHttpBinding());
            app.UseMvc();
        }
    }
    
    public class Program
    {
        static void Main(string[] args)
        {
            var host =
                new WebHostBuilder()
                    .UseKestrel((x) => x.AllowSynchronousIO = true)
                    .UseUrls("http://*:5050")
                    .UseStartup<Startup>()
                    .ConfigureLogging((x) =>
                    {
                         x.AddDebug();
                         x.AddConsole();
                    })
                .Build();

            host.Run();
        }
    }
}
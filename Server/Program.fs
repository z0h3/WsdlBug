open System
open System.Runtime.Serialization
open System.ServiceModel
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Logging
open SoapCore
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

[<DataContract(Namespace = "http://test.com/go")>]
type FsModel() =
    [<DataMember>]
    member val DateExpire : Nullable<DateTime> = Nullable() with get, set

[<ServiceContract(Namespace = "http://test.com")>]
type ISimpleService =
    [<OperationContract>]
    abstract Test: string -> FsModel
    
type SimpleService() =
    interface ISimpleService with
        member x.Test(string: string) =
            FsModel()

type Startup() =
    
    member x.ConfigureServices(services: IServiceCollection) =
        services.AddSingleton<ISimpleService, SimpleService>() |> ignore
        services.AddMvc(fun x -> x.EnableEndpointRouting <- false) |> ignore
        services.AddSoapCore() |> ignore
        
    member x.Configure(app: IApplicationBuilder, env: IHostEnvironment, loggerFactory: ILoggerFactory) =
        app.UseSoapEndpoint<ISimpleService>("/SimpleService.svc", BasicHttpBinding(), SoapSerializer.DataContractSerializer) |> ignore
        app.UseMvc() |> ignore

[<EntryPoint>]
let main argv =
    let host =
        WebHostBuilder()
            .UseKestrel(fun x -> x.AllowSynchronousIO <- true)
            .UseUrls("http://*:5050")
            .UseStartup<Startup>()
            .ConfigureLogging(fun x -> x.AddDebug() |> ignore; x.AddConsole() |> ignore; ())
            .Build()
            
    host.Run()        
    0

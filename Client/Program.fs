
open System.ServiceModel
open FSharp.Data
open CSServer
type SimpleService = WsdlProvider<"http://localhost:5050/SimpleService.svc">


[<EntryPoint>]
let main argv =
    // error
    use simpleService = new SimpleService.BasicHttpBindingClient()
    let res = simpleService.Test("")
    printfn "res: %A" res
    
    // correct   
    let binding = BasicHttpBinding()
    let endpoint = EndpointAddress("http://localhost:5050/SimpleService.svc")
    let channelFactory = new ChannelFactory<ICsSimpleService>(binding, endpoint)
    let serviceClient = channelFactory.CreateChannel()
    let result = serviceClient.Test("")
    printfn "result: %A" result
    0

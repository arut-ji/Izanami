// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open Lib
open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging

let routes = choose [ Todo.Routes.handlers ]

let configureApp (app: IApplicationBuilder) = app.UseGiraffe routes
let configureServices (services: IServiceCollection) = services.AddGiraffe() |> ignore

let configureLogging (builder: ILoggingBuilder) =
  let filter (l: LogLevel) = l.Equals LogLevel.Debug

  builder.AddFilter(filter).AddConsole().AddDebug()
  |> ignore

[<EntryPoint>]
let main _ =
  WebHostBuilder()
    .UseKestrel()
    .Configure(Action<IApplicationBuilder> configureApp)
    .ConfigureServices(configureServices)
    .ConfigureLogging(configureLogging)
    .Build()
    .Run()

  0 // return an integer exit code

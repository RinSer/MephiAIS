namespace RPSAdminApi

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Mvc.NewtonsoftJson
open Hangfire
open Hangfire.Mongo
open MongoDB.Driver
open RPSAdmin
open SyncWithTaiga

type Startup private () =
    new (configuration: IConfiguration) as this =
        Startup() then
        this.Configuration <- configuration

    // This method gets called by the runtime. Use this method to add services to the container.
    member this.ConfigureServices(services: IServiceCollection) =
        // create MongoDB client
        let mongo = MongoClient("mongodb://localhost:27017/")
        // get rps database
        let rps = mongo.GetDatabase("rps")
        // add App as singleton with injected db collection
        let app = new App(rps.GetCollection<Project>("projects"))
        services.AddSingleton<App>(fun sp -> app) |> ignore
        // add TaigaSyncService
        services.AddScoped<TaigaSyncService>() |> ignore
        // Add framework services.
        services.AddControllers().AddNewtonsoftJson() |> ignore
        // add Hangfire service
        let migrationOptions = new MongoMigrationOptions(MongoMigrationStrategy.Migrate)
        migrationOptions.BackupStrategy <- MongoBackupStrategy.Collections
        let storageOptions = new MongoStorageOptions()
        storageOptions.MigrationOptions <- migrationOptions
        services.AddHangfire(new Action<IGlobalConfiguration>(fun gc -> 
                    gc.UseMongoStorage("mongodb://localhost:27017/rps", storageOptions) |> ignore)) |> ignore
        

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member this.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        if (env.IsDevelopment()) then
            app.UseDeveloperExceptionPage() |> ignore

        app.UseRouting() |> ignore

        app.UseAuthorization() |> ignore

        app.UseHangfireServer() |> ignore
        app.UseHangfireDashboard() |> ignore

        app.UseEndpoints(fun endpoints ->
            endpoints.MapControllers() |> ignore
            ) |> ignore

        RecurringJob.AddOrUpdate<TaigaSyncService>((fun(s: TaigaSyncService) -> s.UpdateTasksFromTaiga()), Cron.Hourly(0))


    member val Configuration : IConfiguration = null with get, set
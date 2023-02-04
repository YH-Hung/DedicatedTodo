namespace EvaluateTodo.Server
#nowarn "20"    // suppress return not used warn
open System.Reflection
open EvaluateTodo.Server.DAL
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Configuration

module Program =
    let exitCode = 0

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)

        builder.Services.AddControllers()

        // Fetch information from XML doc
        builder.Services.AddSwaggerGen (fun op ->
           let xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"
           op.IncludeXmlComments(xmlFilename))

        let cnStr = builder.Configuration.GetConnectionString "todos"
        builder.Services.AddNpgsqlDataSource(cnStr)
        builder.Services.AddScoped<ITodoRepository, TodoRepositoryNpgSql>()

        // Allow local SPA connection
        builder.Services.AddCors(fun op ->
            op.AddPolicy("localhost", fun p ->
                p.WithOrigins([| "http://localhost:3000" |])
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                |> ignore))

        let app = builder.Build()

        app.UseHttpsRedirection()
        app.UseHttpLogging()

        app.UseSwagger()
        app.UseSwaggerUI()

        // Allow local SPA connection
        app.UseCors("localhost")

        app.MapControllers()

        app.Run()

        exitCode

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
COPY bin/Release/net7.0/publish/ ./
ENTRYPOINT ["dotnet", "DedicatedTodo.Server.dll"]

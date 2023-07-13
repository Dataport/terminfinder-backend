ARG MCR_REGISTRY=mcr.microsoft.com
FROM ${MCR_REGISTRY}/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY src .
RUN dotnet restore "Dataport.Terminfinder.WebAPI/Dataport.Terminfinder.WebAPI.csproj"
RUN dotnet build "Dataport.Terminfinder.WebAPI/Dataport.Terminfinder.WebAPI.csproj" -c Release -o /app/build
RUN dotnet publish "Dataport.Terminfinder.WebAPI/Dataport.Terminfinder.WebAPI.csproj" -c Release -o /app/publish

ARG MCR_REGISTRY=mcr.microsoft.com
FROM ${MCR_REGISTRY}/dotnet/aspnet:6.0
WORKDIR /app

COPY --from=build /app/publish/ .
EXPOSE 80

CMD ["dotnet", "Dataport.Terminfinder.WebAPI.dll"]

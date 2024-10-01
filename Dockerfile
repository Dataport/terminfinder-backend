ARG MCR_REGISTRY=mcr.microsoft.com/

FROM ${MCR_REGISTRY}dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . ./
RUN dotnet publish "Dataport.Terminfinder.WebAPI/Dataport.Terminfinder.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM ${MCR_REGISTRY}dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 80

RUN apt-get update

COPY --from=build /app/publish/ .

CMD ["dotnet", "Dataport.Terminfinder.WebAPI.dll", "--dbmigrate"]

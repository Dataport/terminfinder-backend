ARG MCR_REGISTRY=mcr.microsoft.com/

################################
###        Production       ####
################################

#FROM ${MCR_REGISTRY}dotnet/sdk:8.0 AS build
#COPY src ./
#RUN dotnet publish "Dataport.Terminfinder.WebAPI/Dataport.Terminfinder.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false
#
#FROM ${MCR_REGISTRY}dotnet/aspnet:8.0
#WORKDIR /app
#EXPOSE 80
#
#RUN apt-get update
#
#COPY --from=build /app/publish/ .
#
#CMD ["dotnet", "Dataport.Terminfinder.WebAPI.dll", "--dbmigrate"]

################################
###        Development      ####
################################

FROM ${MCR_REGISTRY}dotnet/sdk:8.0 AS build
WORKDIR /app
COPY src ./
RUN dotnet publish -o publish -c Debug

FROM ${MCR_REGISTRY}dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish/ .

ENTRYPOINT ["dotnet", "Dataport.Terminfinder.WebAPI.dll", "--dbmigrate"]

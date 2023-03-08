ARG MCR_REGISTRY=mcr.microsoft.com

FROM ${MCR_REGISTRY}/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["Dataport.Terminfinder.WebAPI/Dataport.Terminfinder.WebAPI.csproj", "Dataport.Terminfinder.WebAPI/"]
COPY ["Dataport.Terminfinder.BusinessObject/Dataport.Terminfinder.BusinessObject.csproj", "Dataport.Terminfinder.BusinessObject/"]
COPY ["Dataport.Terminfinder.Common/Dataport.Terminfinder.Common.csproj", "Dataport.Terminfinder.Common/"]
COPY ["Dataport.Terminfinder.BusinessLayer/Dataport.Terminfinder.BusinessLayer.csproj", "Dataport.Terminfinder.BusinessLayer/"]
COPY ["Dataport.Terminfinder.Repository/Dataport.Terminfinder.Repository.csproj", "Dataport.Terminfinder.Repository/"]
RUN dotnet restore "Dataport.Terminfinder.WebAPI/Dataport.Terminfinder.WebAPI.csproj"

COPY . .

WORKDIR "/src/Dataport.Terminfinder.WebAPI"
RUN dotnet build "Dataport.Terminfinder.WebAPI.csproj" -c Release -o /app/build
RUN dotnet publish "Dataport.Terminfinder.WebAPI.csproj" -c Release -o /app/publish

FROM ${MCR_REGISTRY}/dotnet/aspnet:6.0
WORKDIR /app

COPY --from=build /app/publish/ .
EXPOSE 80

CMD ["dotnet", "Dataport.Terminfinder.WebAPI.dll"]

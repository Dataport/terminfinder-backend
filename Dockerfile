# DO NOT USE IN PRODUCTION

ARG MCR_REGISTRY=mcr.microsoft.com

FROM ${MCR_REGISTRY}/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY src ./

ARG USERNAME=terminfinder
ARG USER_ID=1000
ARG GROUP_ID=$USER_ID

RUN groupadd --gid "${GROUP_ID}" "${USERNAME}" && \
    useradd --uid ${USER_ID} --gid ${GROUP_ID} -m ${USERNAME}

RUN dotnet publish -o publish -c Debug
#RUN dotnet publish "Dataport.Terminfinder.WebAPI/Dataport.Terminfinder.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM ${MCR_REGISTRY}/dotnet/aspnet:8.0
WORKDIR /app
#EXPOSE 80

COPY --from=build /app/publish/ .

USER $USERNAME
ENTRYPOINT ["dotnet", "Dataport.Terminfinder.WebAPI.dll"]

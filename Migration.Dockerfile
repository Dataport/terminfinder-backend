# DO NOT USE IN PRODUCTION

ARG MCR_REGISTRY=mcr.microsoft.com

FROM ${MCR_REGISTRY}/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /app
COPY . .

ARG USERNAME=terminfinder
ARG USER_ID=1000
ARG GROUP_ID=$USER_ID

RUN groupadd --gid "${GROUP_ID}" "${USERNAME}" && \
    useradd --uid ${USER_ID} --gid ${GROUP_ID} -m ${USERNAME}

ARG CSPROJ_PATH="src/Dataport.Terminfinder.Repository.Migrate/Dataport.Terminfinder.Repository.Migrate.csproj"

RUN dotnet restore $CSPROJ_PATH
WORKDIR "/app/."
COPY . .
RUN dotnet build $CSPROJ_PATH -c $BUILD_CONFIGURATION -o /app/build

FROM ${MCR_REGISTRY}/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/build/ .

USER $USERNAME
ENTRYPOINT ["dotnet", "Dataport.Terminfinder.Repository.Migrate.dll"]

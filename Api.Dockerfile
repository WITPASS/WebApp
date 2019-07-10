FROM mcr.microsoft.com/dotnet/core/sdk:2.2-alpine3.9 AS build
WORKDIR /app
EXPOSE 80

# copy csproj (inc. dependencies) and restore as distinct layers
COPY Api/Api.csproj ./Api/
COPY Data/Data.csproj ./Data/
COPY Api.Shared/Api.Shared.csproj ./Api.Shared/
COPY EventBus/EventBus.csproj ./EventBus/
COPY EventBus.RabbitMQ/EventBus.RabbitMQ.csproj ./EventBus.RabbitMQ/
WORKDIR /app/Api
RUN dotnet restore

# copy and publish app and libraries
WORKDIR /app/
COPY Api/. ./Api/
COPY Data/. ./Data/
COPY Api.Shared/. ./Api.Shared/
COPY EventBus/. ./EventBus/
COPY EventBus.RabbitMQ/. ./EventBus.RabbitMQ/
WORKDIR /app/Api
RUN dotnet publish -c Release -o out

# build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-alpine3.9
WORKDIR /app
COPY --from=build /app/Api/out ./
ENTRYPOINT ["dotnet", "Api.dll"]

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-alpine3.9 AS build
WORKDIR /app
EXPOSE 80

# copy csproj (inc. dependencies) and restore as distinct layers
COPY Api.Gateway/Api.Gateway.csproj ./Api.Gateway/
WORKDIR /app/Api.Gateway
RUN dotnet restore

# copy and publish app and libraries
WORKDIR /app/
COPY Api.Gateway/. ./Api.Gateway/
WORKDIR /app/Api.Gateway
RUN dotnet publish -c Release -o out

# build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-alpine3.9
WORKDIR /app
COPY --from=build /app/Api.Gateway/out ./
ENTRYPOINT ["dotnet", "Api.Gateway.dll"]

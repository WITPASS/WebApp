FROM mcr.microsoft.com/dotnet/core/sdk:2.2-alpine3.9 AS build
WORKDIR /app
EXPOSE 80

# copy csproj (inc. dependencies) and restore as distinct layers
COPY Public/Public.csproj Public/
COPY Data/Data.csproj Data/

RUN dotnet restore Public/Public.csproj

# copy and publish app and libraries
COPY . .
WORKDIR /app/Public
RUN dotnet publish -c Release -o out

# build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-alpine3.9
WORKDIR /app
COPY --from=build /app/Public/out ./
ENTRYPOINT ["dotnet", "Public.dll"]

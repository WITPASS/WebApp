FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /app
EXPOSE 80

# copy csproj and restore as distinct layers
COPY Public/Public.csproj ./Public/
COPY Data/Data.csproj ./Data/
WORKDIR /app/Public
RUN dotnet restore

# copy and publish app and libraries
WORKDIR /app/
COPY Public/. ./Public/
COPY Data/. ./Data/
WORKDIR /app/Public
RUN dotnet publish -c Release -o out

# build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim
WORKDIR /app
COPY --from=build /app/Public/out ./
ENTRYPOINT ["dotnet", "Public.dll"]

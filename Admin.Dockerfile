FROM mcr.microsoft.com/dotnet/core/sdk:3.0-stretch AS build
WORKDIR /app
EXPOSE 80

# copy csproj and restore as distinct layers
COPY Admin/Admin.csproj ./Admin/
COPY Data/Data.csproj ./Data/
WORKDIR /app/Admin
RUN dotnet restore

# copy and publish app and libraries
WORKDIR /app/
COPY Admin/. ./Admin/
COPY Data/. ./Data/
WORKDIR /app/Admin
RUN dotnet publish -c Release -o out

# build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-stretch-slim
WORKDIR /app
COPY --from=build /app/Admin/out ./
ENTRYPOINT ["dotnet", "Admin.dll"]

FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-preview6-alpine3.9 AS build
WORKDIR /app
EXPOSE 80

# copy csproj (inc. dependencies) and restore as distinct layers
COPY Admin/Admin.csproj ./Admin/
COPY Data/Data.csproj ./Data/
COPY Blazor.Fluxor/Blazor.Fluxor.csproj ./Blazor.Fluxor/
WORKDIR /app/Admin
RUN dotnet restore

# copy and publish app and libraries
WORKDIR /app/
COPY Admin/. ./Admin/
COPY Data/. ./Data/
COPY Blazor.Fluxor/. ./Blazor.Fluxor/
WORKDIR /app/Admin
RUN dotnet publish -c Release

# build runtime image
FROM nginx:alpine
COPY --from=build /app/Admin/bin/Release/netstandard2.0/publish/Admin/dist /usr/share/nginx/html/
COPY nginx.conf /etc/nginx/nginx.conf

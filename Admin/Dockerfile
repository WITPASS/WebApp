FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-stretch-slim AS base
COPY ./publish /publish
WORKDIR /publish
EXPOSE 80
ENTRYPOINT ["dotnet", "Admin.dll"]
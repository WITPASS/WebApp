FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
COPY ./publish /publish
WORKDIR /publish
EXPOSE 80
ENTRYPOINT ["dotnet", "Public.dll"]
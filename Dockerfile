FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /App
COPY --from=build-env /App/out .
EXPOSE 8080
EXPOSE 8180
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Application.dll"]

RUN apt-get update \ 
  && apt-get install -y wget
LABEL deunhealth.restart.on.unhealthy "true"

HEALTHCHECK  --interval=30s --timeout=3s --start-period=10s\
  CMD wget --no-verbose --tries=1 -O /dev/null http://localhost:8180/health

FROM mcr.microsoft.com/dotnet/aspnet:6.0
COPY app App/
WORKDIR /App
EXPOSE 8080
EXPOSE 8180
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Application.dll"]

RUN apt-get update \ 
  && apt-get install -y wget
LABEL deunhealth.restart.on.unhealthy "true"

HEALTHCHECK  --interval=30s --timeout=3s --start-period=10s\
  CMD wget --no-verbose --tries=1 -O /dev/null http://localhost:8180/health

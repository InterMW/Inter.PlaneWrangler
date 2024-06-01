FROM mcr.microsoft.com/dotnet/aspnet:8.0
COPY app App/
WORKDIR /App
EXPOSE 80
EXPOSE 8080
ENTRYPOINT ["dotnet", "Application.dll", "--urls http://0.0.0.0:8080"]

RUN apt-get update \ 
  && apt-get install -y wget
LABEL deunhealth.restart.on.unhealthy "true"

HEALTHCHECK  --interval=30s --timeout=3s --start-period=10s\
  CMD wget --no-verbose --tries=1 -O /dev/null http://localhost:8080/health

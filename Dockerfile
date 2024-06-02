FROM mcr.microsoft.com/dotnet/aspnet:8.0
COPY app App/
WORKDIR /App
ENTRYPOINT ["dotnet", "Application.dll"]

RUN apt-get update \ 
  && apt-get install -y wget
LABEL deunhealth.restart.on.unhealthy "true"


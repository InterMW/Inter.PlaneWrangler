FROM mcr.microsoft.com/dotnet/aspnet:6.0
COPY app App/
WORKDIR /App
EXPOSE 80
EXPOSE 8180
ENTRYPOINT ["dotnet", "Application.dll"]
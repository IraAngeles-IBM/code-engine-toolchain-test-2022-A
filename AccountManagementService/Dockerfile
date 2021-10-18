# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

USER 0
RUN chown -R 1001:0 /source && chmod -R og+rwx /source

COPY *.csproj ./aspnetapp/
RUN dotnet restore ./aspnetapp

# copy everything else and build app
COPY . ./aspnetapp/
RUN chown -R 1001:0 /source/aspnetapp && chmod -R og+rwx /source/aspnetapp

# WORKDIR /source/aspnetapp
# RUN dotnet publish -c release -o /app --no-restore /source/aspnetapp  --no-cache
# RUN dotnet build -c release -o /app --no-restore    /restore --no-cache
RUN dotnet build -c release -o /app --no-restore /source/aspnetapp  --no-cache

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
ENV ASPNETCORE_URLS=http://*:5000
EXPOSE 5000
USER 0
COPY --from=build /app ./
RUN chown -R 1001:0 /app && chmod -R 775 /app
USER 1001
ENTRYPOINT ["dotnet", "AccountManagementService.dll"]

# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source
# ENV ASPNETCORE_URLS=http://*:8080
# EXPOSE 8080

USER 0
RUN chown -R 1001:0 /source && chmod -R og+rwx /source
# RUN apt update && rm -rf /var/lib/apt/lists/*
# ENV ASPNETCORE_URLS=http://*:8080
# EXPOSE 8080
# to use 8080 need to update Program.cs with useurl
# expose port and set optional setting in Code-engine to 80
# copy csproj and restore as distinct layers
# COPY *.sln .
# COPY aspnetapp/*.csproj ./aspnetapp/
COPY *.csproj ./aspnetapp/
RUN dotnet restore ./aspnetapp
# RUN dotnet build ./aspnetapp/

# copy everything else and build app
COPY . ./aspnetapp/
RUN chown -R 1001:0 /source/aspnetapp && chmod -R og+rwx /source/aspnetapp

WORKDIR /source/aspnetapp
RUN dotnet publish -c release -o /app --no-restore /source/aspnetapp

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
ENV ASPNETCORE_URLS=http://*:5000
EXPOSE 5000
# ENV ASPNETCORE_URLS=http://*:8080
# EXPOSE 8080

USER 0

# RUN apt update && rm -rf /var/lib/apt/lists/*
COPY --from=build /app ./

# RUN chown -R 1001:0 /app && chmod -R og+rwx /app
RUN chown -R 1001:0 /app && chmod -R 775 /app

USER 1001


# ENTRYPOINT ["tail", "-f", "/dev/null"]
ENTRYPOINT ["dotnet", "PayrollManagementService.dll"]
#add to trigger commit
#add demo trigger

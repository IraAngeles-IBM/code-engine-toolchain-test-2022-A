# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source
EXPOSE 80
RUN apt update && rm -rf /var/lib/apt/lists/*
# ENV ASPNETCORE_URLS=http://*:8080
# EXPOSE 8080
# to use 8080 need to update Program.cs with useurl
# expose port and set optional setting in Code-engine to 80
# copy csproj and restore as distinct layers
# COPY *.sln .
# COPY aspnetapp/*.csproj ./aspnetapp/
COPY *.csproj ./aspnetapp/
RUN dotnet restore ./aspnetapp

# copy everything else and build app
COPY . ./aspnetapp/
WORKDIR /source/aspnetapp
RUN dotnet publish -c release -o /app --no-restore /source/aspnetapp

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
RUN apt update && rm -rf /var/lib/apt/lists/*
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "PayrollManagementService.dll"]
#add to trigger commit
#add demo trigger

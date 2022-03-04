#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AVUP.Fun.Process/AVUP.Fun.Process.csproj", "AVUP.Fun.Process/"]
RUN dotnet restore "AVUP.Fun.Process/AVUP.Fun.Process.csproj" --configfile NuGet.Config
COPY . .
WORKDIR "/src/AVUP.Fun.Process"
RUN dotnet build "AVUP.Fun.Process.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AVUP.Fun.Process.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AVUP.Fun.Process.dll"]
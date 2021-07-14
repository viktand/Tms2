#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["Tms2/Tms2.csproj", "Tms2/"]
RUN dotnet restore "Tms2/Tms2.csproj"
COPY . .
WORKDIR "/src/Tms2"
RUN dotnet build "Tms2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tms2.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tms2.dll"]
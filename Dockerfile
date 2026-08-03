# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["ContractMonthlyClaimSystem.csproj", "./"]
RUN dotnet restore "ContractMonthlyClaimSystem.csproj"

COPY . .
RUN dotnet build "ContractMonthlyClaimSystem.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "ContractMonthlyClaimSystem.csproj" -c Release -o /app/publish

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Render provides the PORT environment variable - listen on it
ENV ASPNETCORE_URLS=http://+:$PORT
EXPOSE $PORT

ENTRYPOINT ["dotnet", "ContractMonthlyClaimSystem.dll"]
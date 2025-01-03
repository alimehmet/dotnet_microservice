FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["DataAPI/DataAPI.csproj", "DataAPI/"]
RUN dotnet clean "DataAPI/DataAPI.csproj"
RUN dotnet restore "DataAPI/DataAPI.csproj"

COPY ["BusinessAPI/BusinessAPI.csproj", "BusinessAPI/"]
RUN dotnet clean "BusinessAPI/BusinessAPI.csproj"
RUN dotnet restore "BusinessAPI/BusinessAPI.csproj"

COPY ["CurrencyWebSite/CurrencyWebSite.csproj", "CurrencyWebSite/"]
RUN dotnet clean "CurrencyWebSite/CurrencyWebSite.csproj"
RUN dotnet restore "CurrencyWebSite/CurrencyWebSite.csproj"

COPY . .

RUN dotnet publish "DataAPI/DataAPI.csproj" -c Release -o /app/publish/dataapi
RUN dotnet publish "BusinessAPI/BusinessAPI.csproj" -c Release -o /app/publish/businessapi
RUN dotnet publish "CurrencyWebSite/CurrencyWebSite.csproj" -c Release -o /app/publish/currencywebsite

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish/currencywebsite .

EXPOSE 80

ENTRYPOINT ["dotnet", "CurrencyWebSite.dll"]

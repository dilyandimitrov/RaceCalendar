FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

EXPOSE 8080
EXPOSE 443

COPY RaceCalendar.Api/RaceCalendar.Api.csproj RaceCalendar.Api/RaceCalendar.Api.csproj
COPY RaceCalendar.Domain/RaceCalendar.Domain.csproj RaceCalendar.Domain/RaceCalendar.Domain.csproj
COPY RaceCalendar.Infrastructure/RaceCalendar.Infrastructure.csproj RaceCalendar.Infrastructure/RaceCalendar.Infrastructure.csproj
COPY RaceCalendar.sln .

RUN dotnet restore

# RUN ls -alR

COPY . .

RUN dotnet publish RaceCalendar.Api/RaceCalendar.Api.csproj -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0
COPY --from=build-env /publish /publish
WORKDIR /publish
ENTRYPOINT [ "dotnet", "RaceCalendar.Api.dll" ]
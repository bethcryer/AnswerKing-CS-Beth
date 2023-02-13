# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:7.0 as build

WORKDIR /app

COPY . .

RUN dotnet restore && dotnet publish src/Answer.King.Api/Answer.King.Api.csproj -c Release -o out

#build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0

WORKDIR /app

COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "Answer.King.Api.dll"]

EXPOSE 80 443
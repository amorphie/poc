#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base

#####################
#PUPPETEER RECIPE 
#####################
RUN apt-get update && apt-get install -y chromium --no-install-recommends --allow-downgrades fonts-ipafont-gothic fonts-wqy-zenhei fonts-thai-tlwg fonts-kacst fonts-freefont-ttf
######################
#END PUPPETEER RECIPE
######################
ENV PUPPETEER_EXECUTABLE_PATH "/usr/bin/chromium"

WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["amorphie.poc.formio.pupper/amorphie.poc.formio.pupper.csproj", "amorphie.poc.formio.pupper/"]
RUN dotnet restore "amorphie.poc.formio.pupper/amorphie.poc.formio.pupper.csproj"
COPY . .
WORKDIR "/src/amorphie.poc.formio.pupper"
RUN dotnet build "amorphie.poc.formio.pupper.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "amorphie.poc.formio.pupper.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "amorphie.poc.formio.pupper.dll"]
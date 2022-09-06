#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/azure-functions/dotnet:4 AS base
RUN apt-get update && \
 apt-get install -y gnupg wget curl unzip --no-install-recommends && \
 wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add - && \
 echo "deb http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list && \
 apt-get update -y && \
 apt-get install -y google-chrome-stable && \
 DRIVERVER=$(curl -s "https://chromedriver.storage.googleapis.com/LATEST_RELEASE_$CHROMEVER") && \
 wget -q --continue -P /chromedriver "https://chromedriver.storage.googleapis.com/105.0.5195.52/chromedriver_linux64.zip" && \
 unzip /chromedriver/chromedriver* -d /usr/bin/
WORKDIR /home/site/wwwroot
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MyDesiredFlight.HtmlFunction/MyDesiredFlight.HtmlFunction.csproj", "MyDesiredFlight.HtmlFunction/"]
RUN dotnet restore "MyDesiredFlight.HtmlFunction/MyDesiredFlight.HtmlFunction.csproj"
COPY . .
WORKDIR "/src/MyDesiredFlight.HtmlFunction"
RUN dotnet build "MyDesiredFlight.HtmlFunction.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyDesiredFlight.HtmlFunction.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true
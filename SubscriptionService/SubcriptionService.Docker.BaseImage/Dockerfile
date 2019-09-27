FROM mcr.microsoft.com/mssql/server

RUN apt-get update && \
    apt-get install -y dos2unix

WORKDIR /src

COPY initialise-database.sql ./
COPY setup-database.sh ./
COPY entrypoint.sh ./

RUN dos2unix *.sh && apt-get --purge remove -y dos2unix && rm -rf /var/lib/apt/lists/*

# Grant permissions for the setup-database and entrypoint shell scripts to be executable
RUN chmod +x ./setup-database.sh

CMD bash ./entrypoint.sh
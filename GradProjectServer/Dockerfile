#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM ubuntu AS base
ARG DEBIAN_FRONTEND=noninteractive
ENV TZ=Asia/Amman
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
RUN apt-get update && \
    apt-get install -y \
    tzdata \
    wget \ 
    apt-transport-https \
    build-essential \
    libgcc1 \
    libgssapi-krb5-2 \
    libicu66 \
    libssl1.1 \
    libstdc++6 \
    zlib1g && \
    rm -rf /var/lib/apt/lists/*
RUN wget https://packages.microsoft.com/config/ubuntu/20.10/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb
RUN apt-get update && \
    apt-get install -y \
    dotnet-sdk-5.0
RUN apt-get update && \
    apt-get install -y \
    lsb-release
RUN sh -c 'echo "deb http://apt.postgresql.org/pub/repos/apt $(lsb_release -cs)-pgdg main" > /etc/apt/sources.list.d/pgdg.list'
RUN wget --quiet -O - https://www.postgresql.org/media/keys/ACCC4CF8.asc | apt-key add -

RUN apt-get update && \
    apt-get install -y \
    postgresql \
    postgresql-client \
    postgresql-contrib

USER postgres
RUN    /etc/init.d/postgresql start &&\
    psql --command "CREATE USER docker WITH SUPERUSER PASSWORD 'docker';"
RUN echo "host all  all    0.0.0.0/0  md5" >> /etc/postgresql/13/main/pg_hba.conf
RUN echo "listen_addresses='*'" >> /etc/postgresql/13/main/postgresql.conf

USER root
FROM base as publish
WORKDIR /src
COPY . .
RUN dotnet restore "/src/GradProjectServer.csproj"
RUN dotnet publish "/src/GradProjectServer.csproj" -c Release -o /app/publish

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish/ .
EXPOSE 80
EXPOSE 443
EXPOSE 5432
COPY init.sh /app
ENTRYPOINT [ "/app/init.sh" ]
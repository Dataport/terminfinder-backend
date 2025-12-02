# DO NOT USE IN PRODUCTION

ARG REGISTRY=docker.io

FROM ${REGISTRY}/postgres:18.1

RUN localedef -i de_DE -c -f UTF-8 -A /usr/share/locale/locale.alias de_DE.UTF-8
ENV LANG=de_DE.utf8

WORKDIR /app
EXPOSE 5432

COPY create_user.sql /docker-entrypoint-initdb.d

USER postgres

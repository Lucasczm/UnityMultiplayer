FROM ubuntu:xenial

ARG DEBIAN_FRONTEND=noninteractive
RUN apt-get update && \
apt-get install -y libglu1 xvfb libxcursor1

COPY build/                  /root/build/
COPY entrypoint.sh           /entrypoint.sh

WORKDIR /root/
EXPOSE 9000
ENTRYPOINT ["/bin/bash", "/entrypoint.sh"]

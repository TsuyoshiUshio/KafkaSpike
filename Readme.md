# Start kafka broker

This docker compose file is taken from [cp-all-in-one](https://github.com/confluentinc/cp-all-in-one).

## Start Kafka server

```
docker-compose up -d
```

For more details, refer to [Confluent Platform Quick Start (Docker)](https://docs.confluent.io/current/quickstart/ce-docker-quickstart.html#ce-docker-quickstart)
In case you encounter the issue of existing container, these command might help.

```
docker rm control-center
docker rm connect
docker rm rest-proxy
docker rm schema-registry
docker rm broker
docker rm zookeeper
```
## Project Description:

In this example, the project aims to simulate workflows using n8n. To run the project, you first need to bring up the components with the command docker-compose up. After ensuring the data is ready, you should restore the **"kafka-to-redis-postgresql.json"** workflow from the "workflow" folder in n8n.

## Workflow Overview:

The file **"kafka-to-redis-postgresql.json"** represents a workflow in n8n that is triggered by Kafka. It triggers when a message **{"Id": "1"}** is sent to the n8n-topic in Kafka.

## Workflow Steps:

1. **Kafka Trigger:** Reads the message from Kafka.
2. **HttpRequest Step:** Queries user information by sending a request to "https://reqres.in/api/users/{UserId}".
3. **Set Step:** Combines the id information from Kafka and the data received from the HttpRequest.
4. **Upsert to Databases:** Saves the combined data to both PostgreSQL and Redis databases.
The Redis database demonstrates a cache example by setting the "ttl" (time-to-live) duration.
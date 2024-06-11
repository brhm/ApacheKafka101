# Project Setup

This project involves setting up Kafka and Kafka UI on Docker containers, starting the Order API and Stock API, and sending/receiving messages between them.

## Steps

### 1. Setup Kafka and Kafka UI with Docker

Run the following command to start Kafka and Kafka UI containers on Docker:

```bash
docker-compose up -d
```
### 2. Configure Kafka UI
Go to http://localhost:8080 in your browser and add a new cluster with the following details:

Bootstrap Servers: "kafka-0":9092, "kafka-1":9092, "kafka-2":9092
### 3. Start Order API and Stock API
Start the Order API and Stock API. The commands or steps to do this may vary depending on your project setup. Refer to the relevant documentation if needed.

### 4. Send an Order Message via Order API
Use Swagger UI to send an order message through the Order API. Access Swagger UI at http://localhost:<order-api-port>/swagger-ui.html and use the interface to send a POST request to the /orders endpoint. An example of the message payload is:

```json
{
  "userId": "123",
  "totalPrice": 100
}
```
### 5. Read Order Message via Stock API
Ensure the Stock API is running and configured to consume messages from Kafka. Check the logs or relevant endpoints of the Stock API to verify that it is receiving the order messages correctly.

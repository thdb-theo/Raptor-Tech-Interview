The solution consists of two parts, a producer and a consumer.

The producer sends an order through a message queue (RabbitMQ) which then sends it to the consumer.

The database I have used is MongoDB

Both Mongo and Rabbit are running as Docker containers.

Commands to run the docker containers:

`docker run -d -p 27017:27017 --name mongo mongo:latest`
`docker run -d -p 15672:15672 -p 5672:5672 -p 5671:5671 --hostname my-rabbitmq --name my-rabbitmq-container rabbitmq:3-management`

To run the application, start the consumer with `dotnet run` in the `raptor-consumer` folder and likewise `dotnet run` in the producer folder to send data.

To my knowledge, all tasks from the task description are solved. Unfortunately, I couldn't get Xunit to work, so unit tests are a bit tedious to write. I did it from scratch and you can run the unit tests with `dotnet run test`

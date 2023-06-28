using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Text.Json;

using System.Text.Json.Serialization;

class Processor {
    readonly IMongoDatabase database;

    public Processor(EventingBasicConsumer consumer, IMongoDatabase database) {
        this.database = database;
        consumer.Received += (model, ea) => {
            ReceiveMessage(model, ea);
        };
    }

    public Customer GetOrCreateCustomer(Order order) {
        var collection = database.GetCollection<BsonDocument>("users");

        var filter = Builders<BsonDocument>.Filter.Eq("Email", order.CustomerEmail);
        var document = collection.Find(filter).FirstOrDefault();
        Customer customer;

        if (document == null) {
            customer = new Customer(order.CustomerEmail);
            document = new BsonDocument {
                { "Email", customer.Email },
                { "Status", customer.Status.ToString() },
                { "CreatedAt", customer.CreatedAt },
                { "BumpedAt", customer.BumpedAt }
            };
            collection.InsertOne(document);
        } else {
            customer = new Customer(document["Email"].ToString())
            {
                Status = Status.Parse(document["Status"].ToString()),
                CreatedAt = document["CreatedAt"].ToUniversalTime(),
                BumpedAt = document["BumpedAt"].ToUniversalTime()
            };
        }
        return customer;
    }

    /* Returns all receipts from the given user within in the last 'days' days */
    public List<Receipt> GetRecentOrderHistory(Customer customer, int days = 30) {
        var collection = database.GetCollection<BsonDocument>("receipts");

        var filterBuilder = Builders<BsonDocument>.Filter;
        var filter = filterBuilder
            .Eq("CustomerEmail", customer.Email)
            & filterBuilder.Gte("CreatedAt", DateTime.Now - TimeSpan.FromDays(days));

        var documents = collection.Find(filter).ToList();
        List<Receipt> receipts = new List<Receipt>();
    
        foreach (var document in documents) {
            receipts.Add(new Receipt(
                document["CustomerEmail"].ToString(),
                float.Parse(document["Price"].ToString()),
                float.Parse(document["Discount"].ToString()),
                document["CreatedAt"].ToUniversalTime(),
                int.Parse(document["OrderId"].ToString())
            ));
        }
        return receipts;
    }

    public void SyncCustomer(Customer customer) {
        var collection = database.GetCollection<BsonDocument>("users");
        var document = new BsonDocument {
            { "Email", customer.Email },
            { "Status", customer.Status.ToString() },
            { "CreatedAt", customer.CreatedAt },
            { "BumpedAt", customer.BumpedAt }
        };
        var filter = Builders<BsonDocument>.Filter.Eq("Email", customer.Email);
        collection.ReplaceOne(filter, document);
    }

    public void StoreReceipt(Receipt receipt) {
        var collection = database.GetCollection<BsonDocument>("receipts");
        var document = new BsonDocument {
            { "CustomerEmail", receipt.CustomerEmail },
            { "Price", receipt.Price },
            { "Discount", receipt.Discount },
            { "CreatedAt", receipt.CreatedAt },
            { "OrderId", receipt.OrderId }
        };
        collection.InsertOne(document);
    }

    
    public void ReceiveMessage(object? model, BasicDeliverEventArgs ea) {

        var body = ea.Body.ToArray();
        Order? order = JsonSerializer.Deserialize<Order>(Encoding.UTF8.GetString(body));
        if (order == null) {
            Console.WriteLine("Failed to deserialize order");
            return;
        }
        Customer customer = GetOrCreateCustomer(order);
        List<Receipt> orderHistory = GetRecentOrderHistory(customer);
 
        var changed = customer.UpdateStatus(orderHistory, order);
        if (changed) {
            SyncCustomer(customer);
        }
        var receipt = new Receipt(order, customer);
        StoreReceipt(receipt);

        Console.WriteLine("Final Price {0}, Discount {1}, Status {2}", receipt.Price, receipt.Discount, customer.Status);
    }
}
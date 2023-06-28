class Receipt {
    public float Price { get; set; }
    public float Discount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CustomerEmail { get; set; }
    public int OrderId { get; set; }

    public Receipt(Order order, Customer customer) {
        this.Discount = customer.Status.Discount;
        this.Price = CalculatePrice(order.Price, this.Discount);
        this.CreatedAt = DateTime.Now;
        this.CustomerEmail = order.CustomerEmail;
        this.OrderId = order.Id;
    }

    public Receipt(string CustomerEmail, float Price, float Discount, DateTime CreatedAt, int OrderId) {
        this.CustomerEmail = CustomerEmail;
        this.Price = Price;
        this.Discount = Discount;
        this.CreatedAt = CreatedAt;
        this.OrderId = OrderId;
    }

    public static float CalculatePrice(float price, float discount) {
        return price * discount;
    }
}


class Customer {
    public Status Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime BumpedAt { get; set; }
    public string Email { get; set; }

    public Customer(string email) {
        this.Email = email;
        this.Status = Status.REGULAR;
        this.CreatedAt = DateTime.Now;
        this.BumpedAt = DateTime.Now;
    }

    public void Bump() {
        this.BumpedAt = DateTime.Now;
        this.Status = this.Status.Upgraded();
    }

    public float TotalSpent(List<Receipt> orderHistory, Order order) {
        var history = orderHistory.Sum((receipt) => receipt.Price);
        var current = Receipt.CalculatePrice(order.Price, this.Status.Discount);
        return history + current;
    }


    /* Updates the status if the customer meets the criteria
    Return true is status changed, false otherwise */
    public bool UpdateStatus(List<Receipt> orderHistory, Order order) {
        if (Status.ReadyToBump(this, orderHistory, order)) {
            Bump();
            return true;
        }
        return false;
    }
}
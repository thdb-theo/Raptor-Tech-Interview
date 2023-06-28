abstract class Status {
    public abstract float Discount { get; set; }
    public abstract override string ToString();
    public abstract bool ReadyToBump(Customer customer, List<Receipt> orderHistory, Order order);
    public abstract Status Upgraded();

    // create singletons
    public static Status REGULAR = new Regular();
    public static Status SILVER = new Silver();
    public static Status GOLD = new Gold();

    public static Status Parse(string status) {
        return status switch
        {
            "REGULAR" => REGULAR,
            "SILVER" => SILVER,
            "GOLD" => GOLD,
            _ => throw new Exception("Invalid status"),
        };
    }
}

class Regular : Status {
    
    public override float Discount { get; set; } = 1.0f;

    public override string ToString() {
        return "REGULAR";
    }

    public override bool ReadyToBump(Customer customer, List<Receipt> orderHistory, Order order) {
        // I assume "2 or more times within the last 30 days" includes the current order
        if (orderHistory.Count < 1) {
            return false;
        } 
        if (customer.TotalSpent(orderHistory, order) <= 300) {
            return false;
        }
        return true;
    }

    public override Status Upgraded() {
        return Status.SILVER;
    }

}

class Silver : Status {
    public override float Discount { get; set; } = 0.9f;

    public override string ToString() {
        return "SILVER";
    }

    public override bool ReadyToBump(Customer customer, List<Receipt> orderHistory, Order order) {
        if (customer.TotalSpent(orderHistory, order) <= 600) {
            return false;
        }
        if (customer.BumpedAt >= DateTime.Now - TimeSpan.FromDays(7)) {
            return false;
        }
        return true;
    }

    public override Status Upgraded() {
        return Status.GOLD;
    }
}

class Gold : Status {

    public override float Discount { get; set; } = 0.85f;
    public override string ToString() {
        return "GOLD";
    }
    public override bool ReadyToBump(Customer customer, List<Receipt> orderHistory, Order order) {
        return false;
    }

    public override Status Upgraded() {
        return this;
    }
}
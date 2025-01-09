using System;
using System.Collections.Generic;

namespace TheLearningHub_Fitness_Center_Management.Models;

public partial class Subscription
{
    public decimal SubscriptionId { get; set; }

    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }

    public decimal UserId { get; set; }

    public decimal PlanId { get; set; }

    public virtual Paidplan Plan { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

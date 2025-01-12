using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheLearningHub_Fitness_Center_Management.Models;

public partial class Paidplan
{
    public decimal PlanId { get; set; }

    public string? PlanTitle { get; set; }

    public decimal? PlanPrice { get; set; }

    public string? PlanDesc { get; set; }

    public string? ImagePath { get; set; }
    [NotMapped]
    public virtual IFormFile PlanImageFile { get; set; }


    public decimal? UserId { get; set; }

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public virtual User? User { get; set; }
}

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
    public IFormFile? PlanImageFile { get; set; }

    public decimal? UserId { get; set; }

    public decimal? TrainerId { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public virtual User? User { get; set; }

    public virtual User? Trainer { get; set; }

    // Add setters to these properties
    public decimal? PlanPrice3Months { get; set; }

    public decimal? PlanPrice1Year { get; set; }
}

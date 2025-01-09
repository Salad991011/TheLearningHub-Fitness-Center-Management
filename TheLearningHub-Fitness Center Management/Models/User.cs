using System;
using System.Collections.Generic;

namespace TheLearningHub_Fitness_Center_Management.Models;

public partial class User
{
    public decimal UserId { get; set; }

    public string Fname { get; set; } = null!;

    public string Lname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? ImagePath { get; set; }

    public decimal? LoginId { get; set; }

    public virtual Login? Login { get; set; }

    public virtual ICollection<Paidplan> Paidplans { get; set; } = new List<Paidplan>();

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public virtual ICollection<Testimonial> Testimonials { get; set; } = new List<Testimonial>();
}

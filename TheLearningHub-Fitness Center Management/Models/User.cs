using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheLearningHub_Fitness_Center_Management.Models;

public partial class User
{
    public decimal UserId { get; set; }

    public string Fname { get; set; } = null!;

    public string Lname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? ImagePath { get; set; }

    [NotMapped]
    public IFormFile? UsersImageFile { get; set; }

    public decimal? LoginId { get; set; }

    public decimal RoleId { get; set; } // Ensure this is named correctly in User.cs


    public virtual Role? Role { get; set; }

    public virtual Login? Login { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<Paidplan> Paidplans { get; set; } = new List<Paidplan>();

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public virtual ICollection<Testimonial> Testimonials { get; set; } = new List<Testimonial>();
}

using System;
using System.Collections.Generic;

namespace TheLearningHub_Fitness_Center_Management.Models;

public partial class Testimonial
{
    public decimal TestId { get; set; }

    public string? TestText { get; set; }

    public string? IsApproved { get; set; }

    public decimal? Rating { get; set; }

    public decimal? UserId { get; set; }

    public virtual User? User { get; set; }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheLearningHub_Fitness_Center_Management.Models;

public partial class Routine
{
    public decimal RoutineId { get; set; }

    public DateTime? RoutineTime { get; set; }

    public string? Desc { get; set; }

    public string? ImagePath { get; set; }
    [NotMapped]
    public IFormFile? RoutineImageFile { get; set; }

    public decimal? UserId { get; set; } // User this routine is assigned to
    public decimal? TrainerId { get; set; } // Trainer who created the routine

    public virtual User? User { get; set; } // Navigation to user
    public virtual User? Trainer { get; set; } // Navigation to trainer

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}

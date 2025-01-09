using System;
using System.Collections.Generic;

namespace TheLearningHub_Fitness_Center_Management.Models;

public partial class Routine
{
    public decimal RoutineId { get; set; }

    public DateTime? RoutineTime { get; set; }

    public string? Desc { get; set; }

    public string? ImagePath { get; set; }
}

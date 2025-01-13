﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheLearningHub_Fitness_Center_Management.Models;

public partial class Schedule
{
    public decimal ScheduleId { get; set; }

    public DateTime? Day { get; set; }

    public DateTime? Time { get; set; }

    public string? ImagePath { get; set; }
    [NotMapped]
    public IFormFile? ScheduleImageFile { get; set; }

    public decimal? PlanId { get; set; }

    public decimal? RoutineId { get; set; }

    public virtual Paidplan? Plan { get; set; }

    public virtual Routine? Routine { get; set; }
}

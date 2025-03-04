﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheLearningHub_Fitness_Center_Management.Models;

public partial class SchedulePageContent
{
    public decimal SchedulePageId { get; set; }

    public string? BackgroundTitle1 { get; set; }

    public string? BackgroundDesc1 { get; set; }

    public string? BackgroundImagePath1 { get; set; }
    [NotMapped]
    public IFormFile? ScheduleImageFile { get; set; }

    public string? ScheduleTitle { get; set; }

    public string? ScheduleDesc { get; set; }
}

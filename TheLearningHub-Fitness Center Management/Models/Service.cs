using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheLearningHub_Fitness_Center_Management.Models;

public partial class Service
{
    public decimal ServiceId { get; set; }

    public string? ServiceName { get; set; }

    public string? ServiceDesc { get; set; }

    public string? ImagePath { get; set; }
    [NotMapped]
    public virtual IFormFile ServicesImageFile { get; set; }
}

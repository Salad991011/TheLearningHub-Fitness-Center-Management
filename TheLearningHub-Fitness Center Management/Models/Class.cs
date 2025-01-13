using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheLearningHub_Fitness_Center_Management.Models;

public partial class Class
{
    public decimal Classid { get; set; }

    public string? Classname { get; set; }

    public DateTime? Classtime { get; set; }

    public DateTime? Classdate { get; set; }

    public string? Classdesc { get; set; }

    public string? Imagepath { get; set; }
    [NotMapped]
    public IFormFile? ClassImageFile { get; set; }

    public decimal? Userid { get; set; }

    public virtual User? User { get; set; }
}

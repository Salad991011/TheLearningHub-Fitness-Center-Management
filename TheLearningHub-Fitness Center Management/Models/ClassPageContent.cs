using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheLearningHub_Fitness_Center_Management.Models;

public partial class ClassPageContent
{
    public decimal ClassPageId { get; set; }

    public string? BackgroundTitle1 { get; set; }

    public string? BackgroundDesc1 { get; set; }

    public string? BackgroundImagePath1 { get; set; }
	[NotMapped]
	public virtual IFormFile BackgroundImageFile1 { get; set; }

	public string? ClassesTitle { get; set; }

    public string? ClassesDesc { get; set; }
}

using System;
using System.Collections.Generic;

namespace TheLearningHub_Fitness_Center_Management.Models;

public partial class Contactu
{
    public decimal ContactId { get; set; }

    public string? Fname { get; set; }

    public string? Lname { get; set; }

    public string? Email { get; set; }

    public string? Subj { get; set; }

    public string? Msg { get; set; }
}

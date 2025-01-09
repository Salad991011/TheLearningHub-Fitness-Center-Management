using System;
using System.Collections.Generic;

namespace TheLearningHub_Fitness_Center_Management.Models;

public partial class Creditcard
{
    public decimal CreditcardId { get; set; }

    public decimal? CreditcardNum { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public decimal? Cvv { get; set; }

    public decimal? Balance { get; set; }
}

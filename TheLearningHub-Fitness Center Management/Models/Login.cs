using System;
using System.Collections.Generic;

namespace TheLearningHub_Fitness_Center_Management.Models;

public partial class Login
{
    public decimal LoginId { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public decimal? RoleId { get; set; }

    public virtual Role? Role { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

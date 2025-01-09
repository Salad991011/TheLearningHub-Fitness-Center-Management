using System;
using System.Collections.Generic;

namespace TheLearningHub_Fitness_Center_Management.Models;

public partial class Role
{
    public decimal RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<Login> Logins { get; set; } = new List<Login>();
}

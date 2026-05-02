using GymManagementSystem.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.Entities
{
    public class Trainee : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public enGender Gender { get; set; }

        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; }

        public DateTime CreationTime { get; set; }

        protected BaseEntity()
        {
            HandleGuidPrimaryKeyGeneration();
            CreationTime = DateTime.UtcNow;
        }

        private void HandleGuidPrimaryKeyGeneration()
        {
            GetType().GetProperty(nameof(Id))?.SetValue(this, Guid.NewGuid());
        }
    }
}

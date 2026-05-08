namespace GymManagementSystem.DTOs.Attendance.Commands
{
    public class CreateAttendanceSessionCommand
    {
        public Guid TraineeId { get; set; }
        public Guid SubscriptionId { get; set; }
        public DateTime SessionDateTime { get; set; }
    }
}

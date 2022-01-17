namespace DAL.Entities.Identity
{
    public class Secretary : User
    {
        public string DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }
    }
}
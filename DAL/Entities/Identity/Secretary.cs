namespace DAL.Entities.Identity
{
    public class Secretary : User
    {
        public virtual Doctor Doctor { get; set; }
    }
}
namespace DAL.Entities
{
    public class Bed
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }
    }
}
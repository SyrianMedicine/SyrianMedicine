namespace Models.Bed.Inputs
{
    public class UpdateBed
    {
        public int Id { get; set; }
        public bool IsAvailable { get; set; }
        public int DepartmentId { get; set; }
    }
}
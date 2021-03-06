using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class DocumentsDoctor
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }
        public string UrlFile { get; set; }
    }
}
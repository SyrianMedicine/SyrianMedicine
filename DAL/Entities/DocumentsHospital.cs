using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class DocumentsHospital
    {
        public int Id { get; set; }
        public int HospitalId { get; set; }
        public virtual Hospital Hospital { get; set; }
        public string UrlFile { get; set; }
    }
}
using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class DocumentsNurse
    {
        public int Id { get; set; }
        public int NurseId { get; set; }
        public virtual Nurse Nurse { get; set; }
        public string UrlFile { get; set; }
    }
}
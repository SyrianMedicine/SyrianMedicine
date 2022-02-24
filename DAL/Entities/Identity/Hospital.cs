using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DAL.Entities.Identity.Enums;

namespace DAL.Entities.Identity
{
    public class Hospital
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PictureUrl { get; set; }
        public string Location { get; set; }
        public string AboutHospital { get; set; }
        public AccountState AccountState { get; set; }
        public string Email { get; set; }
        public string PhoneNumer { get; set; }
        public string HomeNumber { get; set; }
        public string WebSite { get; set; }
        public string City { get; set; }
        public virtual Department Department { get; set; }
        public virtual List<ReserveHospital> ReserveHospitalForUsers { get; set; }
        public virtual List<DocumentsHospital> DocumentsHospital { get; set; }
    }
}
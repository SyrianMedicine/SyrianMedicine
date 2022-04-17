using Models.Helper;

namespace Models.Hospital.Inputs
{
    public class HospitalQuery : Pagination
    {
        public int OldTotal { get; set; } = 0;
        public string SearchString { get; set; } = null;
        public string DepartmentName { get; set; } = null;
        public bool? HasAvilableBed { get; set; } = null;
        public bool OrderByDesc { get; set; } = true;
    }
}
using Models.Helper;

namespace Models.Doctor.Inputs
{
    public class DoctorQuery : Pagination
    {
        public int OldTotal { get; set; } = 0;
        public bool? WorkAtHome { get; set; } = null;
        public string SearchString { get; set; } = null;
        public DateTime StartTimeWork { get; set; } = default;
        public DateTime EndTimeWork { get; set; } = default;
        public int? Gender { get; set; } = null;
        public bool OrderByDesc { get; set; } = true;
    }
}
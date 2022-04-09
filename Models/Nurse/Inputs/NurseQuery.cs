namespace Models.Nurse.Inputs
{
    public class NurseQuery
    {
        private const int maxPageSize = 10;
        private int pageSize = 6;
        public int PageNumber { get; set; } = 1;
        public int PageSize
        {
            set { pageSize = (value > maxPageSize) ? 10 : value; }
            get => pageSize;
        }
        public int OldTotal { get; set; } = 0;
        public bool? WorkAtHome { get; set; } = null;
        public string Specialization { get; set; } = null;
        public string Name { get; set; } = null;
        public string City { get; set; } = null;
        public DateTime? StartTimeWork { get; set; } = null;
        public DateTime? EndTimeWork { get; set; } = null;
        public string Location { get; set; } = null;
        public int? Gender { get; set; } = null;
        public bool OrderByDesc { get; set; } = true;
    }
}
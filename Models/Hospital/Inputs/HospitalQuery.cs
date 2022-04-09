namespace Models.Hospital.Inputs
{
    public class HospitalQuery
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
        public string HospitalName { get; set; } = null;
        public string City { get; set; } = null;
        public string DepartmentName { get; set; } = null;
        public bool? HasAvilableBed { get; set; } = null;
        public bool OrderByDesc { get; set; } = true;
    }
}
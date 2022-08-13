namespace Models.Account.Outputs
{

    public class SickReserveOutput 
    { 
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime TimeReverse { get; set; }
        public string ReserveState { get; set; }
        public string Type { get; set; }
        public string UserName { get; set; } 
        public string DisplayName { get; set; }  
    }
}
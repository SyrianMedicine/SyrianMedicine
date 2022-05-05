namespace Models.Admin.Outputs
{
    public class ValidateAccountOutput
    {
        public int Id { get; set; }             
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime Date { get; set; }
        public List<string> Documents { get; set; }
    }
}
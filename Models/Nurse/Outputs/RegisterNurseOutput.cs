namespace Models.Nurse.Outputs
{
    public class RegisterNurseOutput
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
namespace Models.Doctor.Outputs
{
    public class RegisterDoctorOutput
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Token { get; set; }
    }
}
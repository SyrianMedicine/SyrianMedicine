namespace Models.Account.Outputs
{
    public class LoginOutput
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Token { get; set; }
        public string UserType { get; set; }
    }
}
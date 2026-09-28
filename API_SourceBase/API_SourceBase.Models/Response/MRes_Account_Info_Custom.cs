namespace API_SourceBase.Models.Response
{
    public class MRes_Account_Info_Custom
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName => $"{FirstName} {LastName}".Trim();
    }
}

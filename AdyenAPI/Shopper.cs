namespace WebApplication1.AdyenAPI
{
    public class Shopper
    {
        public static readonly Shopper Default = new Shopper
        {
            Email = "attef@alphorm.com",
            FirstName = "Attef",
            LastName = "Touina",
            Reference = "AttefLolf"
        };
        public string Email { get; set; }
        public string Reference { get; set; }
        public string FirstName { get; internal set; }
        public string LastName { get; internal set; }
    }
}

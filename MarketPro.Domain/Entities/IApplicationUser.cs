namespace MarketPro.Domain.Entities
{
    public interface IApplicationUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}

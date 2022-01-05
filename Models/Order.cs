namespace ProiectP3_BackendApp.Models
{
    public class Order : IComparable<Order>
    {
        public int Id { get; set; }
        public User User { get; set; }
        public List<MenuItem> Items { get; set; }
        public string Adress { get; set; }
        public DateTime Date { get; set; }  

        public int CompareTo(Order? other)
        {
            if(other == null)
                return 1;
            return Date.CompareTo(other.Date);
        }
    }
}

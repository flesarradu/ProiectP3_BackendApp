namespace ProiectP3_BackendApp.Models
{
    public class User : IComparable<User>
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public int CompareTo(User? other)
        {
            if(other == null) 
                return 1;
            else
                return Id.CompareTo(other.Id);
                        
        }

    }
}

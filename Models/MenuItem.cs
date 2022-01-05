namespace ProiectP3_BackendApp.Models
{
    
    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TYPE Type { get; set; }
        public double Price { get; set; }
        public enum TYPE
        {
            APERITIV,
            SUPA_SI_CIORBA,
            FEL_PRINCIPAL,
            PIZZA,
            SALATA,
            BAUTURI,
            DESERT
        }
    }
}

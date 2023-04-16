namespace dotnet_rpg.Models
{
    public class Apparel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Resistance { get; set; }
        public Character? Character { get; set; }
        public int CharacterId { get; set; }
    }
}
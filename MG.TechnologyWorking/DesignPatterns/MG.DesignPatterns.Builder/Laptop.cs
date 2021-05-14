namespace MG.DesignPatterns.Builder
{
    public class Laptop
    {
        public string Model { get; set; }
        public string RAM { get; set; }
        public string Disk { get; set; }

        public override string ToString()
        {
            return $"Model: {Model} - Ram: {RAM} - Disk: {Disk}";
        }
    }
}
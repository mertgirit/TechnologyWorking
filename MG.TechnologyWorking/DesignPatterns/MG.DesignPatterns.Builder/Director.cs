namespace MG.DesignPatterns.Builder
{
    public class Director
    {
        public void Create(LaptopBuilder laptopBuilder)
        {
            laptopBuilder.CombineRam();
            laptopBuilder.CombineDisk();
        }
    }
}
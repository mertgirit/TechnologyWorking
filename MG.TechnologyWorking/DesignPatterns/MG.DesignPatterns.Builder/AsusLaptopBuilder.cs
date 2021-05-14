namespace MG.DesignPatterns.Builder
{
    public class AsusLaptopBuilder : LaptopBuilder
    {
        public AsusLaptopBuilder()
        {
            laptop = new Laptop { Model = "Asus" };
        }
        public override void CombineDisk()
        {
            laptop.RAM = "16";
        }

        public override void CombineRam()
        {
            laptop.Disk = "HDD";
        }
    }
}
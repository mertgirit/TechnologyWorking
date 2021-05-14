namespace MG.DesignPatterns.Builder
{
    public class LevonoLaptopBuilder : LaptopBuilder
    {
        public LevonoLaptopBuilder()
        {
            laptop = new Laptop { Model = "Lenovo"};
        }
        public override void CombineRam()
        {
            laptop.RAM = "32";
        }

        public override void CombineDisk()
        {
            laptop.Disk = "SSD";
        }
    }
}
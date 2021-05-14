namespace MG.DesignPatterns.Builder
{
    public abstract class LaptopBuilder
    {
        protected Laptop laptop;

        public Laptop Laptop
        {
            get { return laptop; }
        }

        public abstract void CombineRam();
        public abstract void CombineDisk();
    }
}
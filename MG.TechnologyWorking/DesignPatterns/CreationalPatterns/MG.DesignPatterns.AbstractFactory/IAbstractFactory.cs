namespace MG.DesignPatterns.AbstractFactory
{
    public interface IAbstractFactory
    {
        IAbstractProductA CreateProductA();

        IAbstractProductB CreateProductB();
    }
}
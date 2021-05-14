using System;

namespace MG.Test.AbstractFactoryTest
{
    using MG.DesignPatterns.AbstractFactory;
    using MG.DesignPatterns.AbstractFactory.Sample;

    class Program
    {
        static void Main(string[] args)
        {
            TestMethod1();
            TestMethod2();
        }

        public static void TestMethod1()
        {
            Console.WriteLine("Client: Testing client code with the first factory type...");
            ClientMethod(new ConcreteFactory1());
            Console.WriteLine();

            Console.WriteLine("Client: Testing the same client code with the second factory type...");
            ClientMethod(new ConcreteFactory2());
        }
        public static void ClientMethod(IAbstractFactory factory)
        {
            var productA = factory.CreateProductA();
            var productB = factory.CreateProductB();

            Console.WriteLine(productB.UsefulFunctionB());
            Console.WriteLine(productB.AnotherUsefulFunctionB(productA));
        }

        public static void TestMethod2()
        {
            try
            {
                ICarCreator carCreator = AbstractCarCreatorFactory.GetCarCreator(CarType.Bike);
                Console.WriteLine(carCreator.Create());

                carCreator = AbstractCarCreatorFactory.GetCarCreator(CarType.Sedan);
                Console.WriteLine(carCreator.Create());

                ICarCreator carCreator2 = AbstractCarCreatorFactory.GetCarCreator(CarType.Truck);
                Console.WriteLine(carCreator2.Create());

                carCreator2 = AbstractCarCreatorFactory.GetCarCreator(CarType.None);
                Console.WriteLine(carCreator2.Create());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }
    }
}
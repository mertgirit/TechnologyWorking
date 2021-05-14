using System;

namespace MG.Test.BuilderTest
{
    using MG.DesignPatterns.Builder;

    class Program
    {
        static void Main(string[] args)
        {
            LaptopBuilder laptopBuilder;

            Director director = new Director();
            laptopBuilder = new LevonoLaptopBuilder();

            director.Create(laptopBuilder);
            Console.WriteLine(laptopBuilder.Laptop.ToString());

            laptopBuilder = new AsusLaptopBuilder();
            director.Create(laptopBuilder);
            Console.WriteLine(laptopBuilder.Laptop.ToString());
        }
    }
}
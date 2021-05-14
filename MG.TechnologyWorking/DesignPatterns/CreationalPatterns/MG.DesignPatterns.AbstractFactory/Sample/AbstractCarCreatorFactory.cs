using System;

namespace MG.DesignPatterns.AbstractFactory.Sample
{
    public abstract class AbstractCarCreatorFactory
    {
        public static ICarCreator GetCarCreator(CarType carType)
        {
            switch (carType)
            {
                case CarType.Sedan:
                    return new SedanCreator();
                case CarType.Truck:
                    return new TruckCreator();
                case CarType.Bike:
                    return new BikeCreator();
                default:
                    throw new ArgumentNullException($"undefined CarType: {carType}");
            }
        }
    }
}
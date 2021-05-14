namespace MG.DesignPatterns.AbstractFactory.Sample
{
    public class TruckCreator : ICarCreator
    {
        public string Create()
        {
            return $"{nameof(TruckCreator)} created";
        }
    }
}
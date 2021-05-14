namespace MG.DesignPatterns.AbstractFactory.Sample
{
    public class BikeCreator : ICarCreator
    {
        public string Create()
        {
            return $"{nameof(BikeCreator)} created";
        }
    }
}
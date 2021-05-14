namespace MG.DesignPatterns.AbstractFactory.Sample
{
    public class SedanCreator : ICarCreator
    {
        public string Create()
        {
            return $"{nameof(SedanCreator)} created";
        }
    }
}
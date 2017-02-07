namespace Assets.Gamelogic.Utils
{
    public interface IWeighted
    {
        float Weighting { get; set; }
    }

    public class WeightedItem<TItem> : IWeighted
    {
        public readonly TItem Item;
        public float Weighting { get; set; }

        public WeightedItem(TItem item, float weighting)
        {
            Item = item;
            Weighting = weighting;
        }
    }
}

using System.Collections.Generic;
using System.Linq;

namespace Assets.Gamelogic.Utils
{
    class WeightsNormalizer<TWeighted> where TWeighted : class, IWeighted
    {
        public static IList<TWeighted> Normalize(IList<TWeighted> weightedEntities)
        {
            float totalWeights = weightedEntities.Sum(w => w.Weighting);
            foreach (var weightedEntity in weightedEntities)
            {
                weightedEntity.Weighting /= totalWeights;
            }
            return weightedEntities;
        }
    }
}
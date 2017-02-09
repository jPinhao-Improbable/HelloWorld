using System;
using UnityEngine;

namespace Assets.Gamelogic.Utils
{
    public struct TimedUpdate<TType>
    {
        public TType Value;
        public float timeStamp;
    }

    public class ClientPredictionInterpolator<TType>
    {
        private Func<TType, TType, float, TType> Interpolate;

        private TimedUpdate<TType> current;
        private TimedUpdate<TType> target;

        private float FullUpdateLength;
        private float LastUpdate;

        public ClientPredictionInterpolator(TimedUpdate<TType> startValue, Func<TType, TType, float, TType> interpolateFunction)
        {
            Interpolate = interpolateFunction;

            current = startValue;
            target = startValue;

            LastUpdate = 0;
            FullUpdateLength = 1;
        }

        public void Update(TimedUpdate<TType> newTarget)
        {
            FullUpdateLength = newTarget.timeStamp - target.timeStamp;
            LastUpdate = Time.fixedTime;

            current = target;
            target = newTarget;
        }

        public TType NextPosition()
        {
            var deltaTime = Time.fixedTime - LastUpdate;

            var lerpRatio = Mathf.Min(deltaTime / FullUpdateLength, 1f);
            return Interpolate(current.Value, target.Value, lerpRatio);
        }
    }
}

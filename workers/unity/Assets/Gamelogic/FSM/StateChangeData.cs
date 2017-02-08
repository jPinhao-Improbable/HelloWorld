using Improbable;
using UnityEngine;

namespace Assets.Gamelogic.FSM
{
    public struct StateChangeData<TStateEnum>
    {
        public TStateEnum NewState;
        public EntityId TargetEntity;
        public Vector3 TargetPosition;
    }
}

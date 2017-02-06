using Improbable;

namespace Assets.Gamelogic.NPC.Wizard.InteractionStrategies
{
    public interface IEntityFinder
    {
        FoundEntity FindEntity();
    }

    public struct FoundEntity
    {
        public EntityId entity;
        public float distance;
    }
}
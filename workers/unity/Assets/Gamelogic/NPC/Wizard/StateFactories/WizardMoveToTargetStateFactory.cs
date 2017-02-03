using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Gamelogic.NPC.Wizard.InteractionStrategies;
using Improbable.Npc;

namespace Assets.Gamelogic.NPC.Wizard.StateFactories
{
    public static class WizardMoveToTargetStateFactory
    {
        public static WizardMoveToTargetState Build(WizardStateMachine owner,
            WizardBehaviour inParentBehaviour,
            TargetNavigation.Writer inTargetNavigation,
            TargetNavigationBehaviour inNavigation)
        {
            IStateChangerStrategy strategy;
            var attack = inParentBehaviour.gameObject.GetComponent<AttackEnemyBehaviour>();
            var defend = inParentBehaviour.gameObject.GetComponent<DefendFriendBehaviour>();

            if (attack != null && defend != null)
            {
                strategy = new AttackOrDefendStrategy(attack, defend);
            }
            else if (attack == null && defend == null)
            {
                strategy = new DoNothing();
            }
            else strategy = attack != null ? (IStateChangerStrategy) attack : defend;


            return new WizardMoveToTargetState(owner, inParentBehaviour, inTargetNavigation, inNavigation, strategy);
        }
    }
}

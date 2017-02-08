using Assets.Gamelogic.Abilities;
using Assets.Gamelogic.Communication;
using Assets.Gamelogic.Core;
using Assets.Gamelogic.Fire;
using Assets.Gamelogic.FSM;
using Assets.Gamelogic.Team;
using Improbable;
using Improbable.Abilities;
using Improbable.Collections;
using Improbable.Math;
using Improbable.Npc;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.NPC.Wizard
{
    [EngineType(EnginePlatform.FSim)]
    public class WizardBehaviour : MonoBehaviour, IFlammable
    {
        [Require] private NPCWizard.Writer npcWizard;
        [Require] private TargetNavigation.Writer targetNavigation;
        [Require] private Spells.Writer spells;

        [SerializeField] private SpellsBehaviour spellsBehaviour;
        [SerializeField] private TeamAssignmentVisualizerFSim teamAssignment;
        [SerializeField] private TargetNavigationBehaviour navigation;
        [SerializeField] private List<Coordinates> cachedTeamHqCoordinates;
        [SerializeField] private NpcSendChatBehaviour sendChat;
        [SerializeField] private WizardChatInterpreterBehaviour chatInterpreter;

        private WizardStateMachine stateMachine;

        private void Awake()
        {
            navigation = gameObject.GetComponentIfUnassigned(navigation);
            teamAssignment = gameObject.GetComponentIfUnassigned(teamAssignment);
            spellsBehaviour = gameObject.GetComponentIfUnassigned(spellsBehaviour);
        }

        private void OnEnable()
        {
            cachedTeamHqCoordinates = new List<Coordinates>(SimulationSettings.TeamHQLocations);
            stateMachine = new WizardStateMachine(this, npcWizard, navigation, teamAssignment, targetNavigation, spellsBehaviour, cachedTeamHqCoordinates);
            stateMachine.OnEnable(npcWizard.Data.currentState);
            stateMachine.StateChanged += OnStateChange;

            chatInterpreter.ChangeStateCommand += OnChangeStateCommand;
        }

        private void OnDisable()
        {
            stateMachine.OnDisable();
            stateMachine.StateChanged -= OnStateChange;

            chatInterpreter.ChangeStateCommand -= OnChangeStateCommand;
        }

        public void OnIgnite()
        {
            stateMachine.TriggerTransition(WizardFSMState.StateEnum.ON_FIRE, EntityId.InvalidEntityId, SimulationSettings.InvalidPosition);
        }

        public void OnExtinguish()
        {
            stateMachine.TriggerTransition(WizardFSMState.StateEnum.IDLE, EntityId.InvalidEntityId, SimulationSettings.InvalidPosition);
        }

        private void OnStateChange(FiniteStateMachine<WizardFSMState.StateEnum> sender, FsmStateChangedArguments<WizardFSMState.StateEnum> args)
        {
            string chatMessage = string.Empty;
            switch (args.NewState)
            {
                case WizardFSMState.StateEnum.IDLE :
                    return;
                case WizardFSMState.StateEnum.MOVING_TO_TARGET:
                    chatMessage = "Changing target";
                    break;
                case WizardFSMState.StateEnum.ATTACKING_TARGET :
                    chatMessage = "Burn maggot";
                    break;
                case WizardFSMState.StateEnum.DEFENDING_TARGET :
                    chatMessage = "To the rescue";
                    break;
                case WizardFSMState.StateEnum.ON_FIRE :
                    chatMessage = "Aaaaaaaaa";
                    break;
                default :
                    Debug.LogError("Wizard changed to unknown state - " + args.NewState.ToString() + " - no chat to notify change");
                    return;
            }

            sendChat.SayChat(chatMessage);
        }

        private void OnChangeStateCommand(StateChangeData<WizardFSMState.StateEnum> newState)
        {
            stateMachine.TriggerTransition(newState.NewState, newState.TargetEntity, newState.TargetPosition);
        }
    }
}

using Assets.Gamelogic.Communication;
using Assets.Gamelogic.Core;
using Assets.Gamelogic.FSM;
using Improbable;
using Improbable.Npc;
using Improbable.Team;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.NPC.Wizard
{
    public delegate void StateChangeRequest<TStateEnum>(StateChangeData<TStateEnum> state);

    [EngineType(EnginePlatform.FSim)]
    public class WizardChatInterpreterBehaviour : MonoBehaviour
    {
        [Require] private TeamAssignment.Reader team;

        [SerializeField] private ChatReceiverBehaviour chatReceiver;

        public event StateChangeRequest<WizardFSMState.StateEnum> ChangeStateCommand;

        private void OnEnable()
        {
            chatReceiver.MessageReceived += ParseMessage;
        }

        private void OnDisable()
        {
            chatReceiver.MessageReceived -= ParseMessage;
        }

        private void ParseMessage(string message, EntityId sender)
        {
            var senderEntity = NPCUtils.GetTargetGameObject(sender);
            if (senderEntity == null)
            {
                Debug.LogError("Couldn't find sender info when parsing chat message");
                return;
            }

            if (!senderEntity.CompareTag("Player") || !NPCUtils.IsInTeam(senderEntity, team.Data))
            {
                //Don't want to parse message which aren't from a player or from the opposite team
                return;
            }
            
            ParseMessageCommand(message, senderEntity);
        }

        private void ParseMessageCommand(string message, GameObject sender)
        {
            switch (message.ToUpper())
            {
                case "COME HERE":
                    var moveToPositionData = new StateChangeData<WizardFSMState.StateEnum>
                    {
                        NewState = WizardFSMState.StateEnum.MOVING_TO_TARGET,
                        TargetEntity = EntityId.InvalidEntityId,
                        TargetPosition = sender.transform.position
                    };
                    OnChangeStateCommand(moveToPositionData);
                    return;

                case "HELP ME":
                    var helpFriendData = new StateChangeData<WizardFSMState.StateEnum>
                    {
                        NewState = WizardFSMState.StateEnum.DEFENDING_TARGET,
                        TargetEntity = sender.EntityId(),
                        TargetPosition = SimulationSettings.InvalidPosition
                    };

                    OnChangeStateCommand(helpFriendData);
                    return;
            }
        }

        private void OnChangeStateCommand(StateChangeData<WizardFSMState.StateEnum> state)
        {
            if (ChangeStateCommand != null)
            {
                ChangeStateCommand(state);
            }
        }
    }
}

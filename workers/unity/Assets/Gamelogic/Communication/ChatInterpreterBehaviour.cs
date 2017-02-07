using Assets.Gamelogic.NPC;
using Assets.Gamelogic.Team;
using Improbable.Communication;
using Improbable.Core;
using Improbable.Entity.Component;
using Improbable.Team;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Communication
{
    [EngineType(EnginePlatform.FSim)]
    public class ChatInterpreterbehaviour : MonoBehaviour
    {
        [Require] private Chat.Writer chat;

        [Require] private TeamAssignment.Reader team;

        void OnEnable()
        {
            chat.CommandReceiver.OnReceiveChat += ParseChatMessage;
        }

        private void ParseChatMessage(ResponseHandle<Chat.Commands.ReceiveChat, ChatMessage, Nothing> request)
        {
            var sender = request.Request.sender;

            var senderEntity = NPCUtils.GetTargetGameObject(sender);
            if (senderEntity == null)
            {
                Debug.LogError("Couldn't find sender info when parsing chat message");
                return;
            }

            if (senderEntity.name != "Player" || !NPCUtils.IsInTeam(senderEntity, team.Data))
            {
                //Don't want to parse message which aren't from a player or from the opposite team
                return;
            }

            //Do somethign with this message
            Debug.LogWarning("Received a message from player: " + request.Request.message);

            request.Respond(new Nothing());
        }

        // Update is called once per frame
        void Update () {
		
        }
    }
}

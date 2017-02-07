using Improbable.Communication;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Communication
{

    [EngineType(EnginePlatform.FSim)]
    public class NpcSendChatBehaviour : MonoBehaviour {

        [Require] Chat.Writer authCheck;

        public void SayChat(string message)
        {
            SpatialOS.Commands.SendCommand(authCheck, Chat.Commands.SendChat.Descriptor, new ChatMessage(message, gameObject.EntityId()), gameObject.EntityId(), response => { });
        }
    }
}

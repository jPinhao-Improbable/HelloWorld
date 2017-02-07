using Improbable.Communication;
using Improbable.Core;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Communication
{
    [EngineType(EnginePlatform.Client)]
    public class SendChatBehaviour : MonoBehaviour
    {
        [Require] private ClientAuthorityCheck.Writer authCheck;
        
        public void SayChat(string message)
        {
			SpatialOS.Commands.SendCommand(authCheck, Chat.Commands.SendChat.Descriptor, new ChatMessage(message, gameObject.EntityId()), gameObject.EntityId(), response => {});
        }
    }
}
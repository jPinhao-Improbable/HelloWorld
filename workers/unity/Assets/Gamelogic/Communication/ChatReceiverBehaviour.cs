using Improbable;
using Improbable.Communication;
using Improbable.Core;
using Improbable.Entity.Component;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Communication
{
    //public delegate void OnChatCommandReceiver<TCommandType>(TCommandType command);
    public delegate void ChatMessageReceived(string message, EntityId sender);

    [EngineType(EnginePlatform.FSim)]
    public class ChatReceiverBehaviour : MonoBehaviour
    {
        [Require] private Chat.Writer chat;

        public event ChatMessageReceived MessageReceived;

        void OnEnable()
        {
            chat.CommandReceiver.OnReceiveChat += ReceiveChatMessage;
        }

        void OnDisable()
        {
            chat.CommandReceiver.OnReceiveChat -= ReceiveChatMessage;
        }

        private void ReceiveChatMessage(ResponseHandle<Chat.Commands.ReceiveChat, ChatMessage, Nothing> request)
        {
            var sender = request.Request.sender;
            var message = request.Request.message;

            OnMessageReceived(message, sender);

            request.Respond(new Nothing());
        }

        private void OnMessageReceived(string message, EntityId sender)
        {
            if (MessageReceived != null)
            {
                MessageReceived(message, sender);
            }
        }
    }
}
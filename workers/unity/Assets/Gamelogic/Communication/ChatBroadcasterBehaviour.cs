using Improbable.Communication;
using Improbable.Core;
using Improbable.Entity.Component;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Communication
{
    [EngineType(EnginePlatform.FSim)]
    public class ChatBroadcasterBehaviour : MonoBehaviour
    {
        [Require] private Chat.Writer chat;

        private void OnEnable()
        {
            chat.CommandReceiver.OnSendChat += HandleSendChat;
        }

        private void HandleSendChat(ResponseHandle<Chat.Commands.SendChat, ChatMessage, Nothing> request)
        {
            var message = request.Request.message;
            var update = new Chat.Update();
            update.AddChatSent(new ChatMessage(message.Substring(0, Mathf.Min(15, message.Length) )));
            chat.Send(update);
            request.Respond(new Nothing());
        }

        private void OnDisable()
        {
            chat.CommandReceiver.OnSendChat -= HandleSendChat;
        }
    }
}

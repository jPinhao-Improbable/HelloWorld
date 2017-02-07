using System.Collections.Generic;
using System.Linq;
using Improbable;
using Improbable.Communication;
using Improbable.Core;
using Improbable.Entity.Component;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Core.EntityQueries;
using Improbable.Unity.Visualizer;
using Improbable.Worker;
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
            update.AddChatSent(new ChatMessage(message.Substring(0, Mathf.Min(15, message.Length) ), gameObject.EntityId()));
            chat.Send(update);

            LocalMessageBroadcast(message);

            request.Respond(new Nothing());
        }

        private void LocalMessageBroadcast(string message)
        {
            var nearbyEntities = Query.And(
                Query.HasComponent<Chat>(),
                Query.InSphere(transform.position.x, transform.position.y, transform.position.z, 20.0)
            ).ReturnOnlyEntityIds();

            SpatialOS.Commands.SendQuery(chat, nearbyEntities, entitiesQuery =>
            {
                if (entitiesQuery.StatusCode != StatusCode.Success)
                {
                    Debug.LogError("Find nearby entities failed with error: " + entitiesQuery.ErrorMessage);
                }
                if (entitiesQuery.Response.Value.EntityCount < 1)
                {
                    return;
                }

                SendMessageToEntities(new ChatMessage(message, gameObject.EntityId()),
                    entitiesQuery.Response.Value.Entities.Keys
                                                            .Where(id => id != gameObject.EntityId())
                                                            .ToList());
            });
        }

        private void SendMessageToEntities(ChatMessage message, IList<EntityId> targets)
        {
            foreach (var targetEntityId in targets)
            {
                SpatialOS.Commands.SendCommand(chat,
                    Chat.Commands.ReceiveChat.Descriptor,
                    message,
                    targetEntityId,
                    result => { });
            }
        }

        private void OnDisable()
        {
            chat.CommandReceiver.OnSendChat -= HandleSendChat;
        }
    }
}

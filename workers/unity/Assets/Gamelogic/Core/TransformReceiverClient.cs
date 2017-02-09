using Assets.Gamelogic.Utils;
using Improbable.Core;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    [EngineType(EnginePlatform.Client)]
    public class TransformReceiverClient : MonoBehaviour
    {
        [Require] private TransformComponent.Reader transformComponent;

        private bool isRemote;

        [SerializeField] private Rigidbody myRigidbody;

        private ClientPredictionInterpolator<Vector3> movementInterpolator;
        private ClientPredictionInterpolator<float> rotationInterpolator;

        private void Awake()
        {
            myRigidbody = gameObject.GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            transformComponent.ComponentUpdated += OnTransformComponentUpdated;
            if (IsNotAnAuthoritativePlayer())
            {
                SetUpRemoteTransform();

                movementInterpolator = new ClientPredictionInterpolator<Vector3>(new TimedUpdate<Vector3> {Value = transformComponent.Data.position.ToVector3(), timeStamp = transformComponent.Data.timeStamp }, Vector3.Lerp );
                rotationInterpolator = new ClientPredictionInterpolator<float>(new TimedUpdate<float> { Value = (float)transformComponent.Data.rotation, timeStamp = transformComponent.Data.timeStamp }, Mathf.Lerp);
            }     
        }

        private void OnDisable()
        {
            transformComponent.ComponentUpdated -= OnTransformComponentUpdated;
            if (isRemote)
            {
                TearDownRemoveTransform();
            }
        }

        private void OnTransformComponentUpdated(TransformComponent.Update update)
        {
            for (int i = 0; i < update.teleportEvent.Count; i++)
            {
                TeleportTo(update.teleportEvent[i].targetPosition.ToVector3());
            }

            if(IsNotAnAuthoritativePlayer())
            {
                if (update.position.HasValue && update.timeStamp.HasValue)
                {
                    movementInterpolator.Update(new TimedUpdate<Vector3> { Value = update.position.Value.ToVector3(), timeStamp = update.timeStamp.Value });
                }
                if (update.rotation.HasValue && update.timeStamp.HasValue)
                {
                    rotationInterpolator.Update(new TimedUpdate<float> { Value = update.rotation.Value, timeStamp = update.timeStamp.Value });
                }
            }
        }

        private void TeleportTo(Vector3 position)
        {
            myRigidbody.velocity = Vector3.zero;
            myRigidbody.MovePosition(position);

            movementInterpolator = new ClientPredictionInterpolator<Vector3>(new TimedUpdate<Vector3> { Value = transformComponent.Data.position.ToVector3(), timeStamp = transformComponent.Data.timeStamp }, Vector3.Lerp);
            rotationInterpolator = new ClientPredictionInterpolator<float>(new TimedUpdate<float> { Value = (float)transformComponent.Data.rotation, timeStamp = transformComponent.Data.timeStamp }, Mathf.LerpAngle);
        }

        private bool IsNotAnAuthoritativePlayer()
        {
            return !gameObject.HasAuthority(ClientAuthorityCheck.ComponentId);
        }
        
        private void FixedUpdate()
        {
            if (IsNotAnAuthoritativePlayer())
            {
                myRigidbody.MovePosition(movementInterpolator.NextPosition());
                myRigidbody.MoveRotation(Quaternion.Euler(0f, QuantizationUtils.DequantizeAngle((uint)rotationInterpolator.NextPosition()), 0f));
            }
            else if(isRemote)
            {
                TearDownRemoveTransform();
            }
        }

        private void SetUpRemoteTransform()
        {
            isRemote = true;
            myRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            myRigidbody.isKinematic = true;
        }

        private void TearDownRemoveTransform()
        {
            isRemote = false;
            myRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            myRigidbody.isKinematic = false;
        }
    }
}

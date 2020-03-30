using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS_Demo.KCC
{
    public struct PlayerInputState
    {
        public float MoveAxisRight;
        public float MoveAxisForward;
        public Quaternion CameraRotation;
    }

    public class PlayerKinematicController : MonoBehaviour, ICharacterController
    {

        public KinematicCharacterMotor motor;
        public Transform playerRoot;

        [Header("Stable Movement")]
        public float MaxStableMoveSpeed = 10f;
        public float StableMovementSharpness = 15;
        public float OrientationSharpness = 10;

        [Header("Air Movement")]
        public float MaxAirMoveSpeed = 10f;
        public float AirAccelerationSpeed = 5f;
        public float Drag = 0.1f;

        [Header("Misc")]
        public Vector3 Gravity = new Vector3(0, -30f, 0);


        private Vector3 _lookCameraPlane = Vector3.forward;
        private Vector3 _lookCameraDirection = Vector3.forward;
        private Vector3 _moveInputVector = Vector3.zero;


        // Start is called before the first frame update
        void Start()
        {
            motor.CharacterController = this;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetPlayerInput(ref PlayerInputState inputState)
        {

            //_lookCameraDirection = (inputState.CameraRotation * Vector3.forward).normalized;
           // Debug.Log("Normalized Look Direction: " + _lookCameraDirection);

            // Clamp input
            Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputState.MoveAxisRight, 0f, inputState.MoveAxisForward), 1f);

            // Calculate camera direction and rotation on the character plane
            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputState.CameraRotation * Vector3.forward, motor.CharacterUp).normalized;
            if (cameraPlanarDirection.sqrMagnitude == 0f)
            {
                cameraPlanarDirection = Vector3.ProjectOnPlane(inputState.CameraRotation * Vector3.up, motor.CharacterUp).normalized;
            }
            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, motor.CharacterUp);

            // Move and look inputs
            //_moveInputVector = cameraPlanarRotation * moveInputVector;
            _moveInputVector = cameraPlanarRotation * moveInputVector;
            _lookCameraPlane = cameraPlanarDirection;
        }


        public void PostInputUpdate(float deltaTime)
        {
                Quaternion newRotation = Quaternion.identity;
                HandleRotation(ref newRotation, deltaTime);
                playerRoot.rotation = newRotation;
            
        }

        private void HandleRotation(ref Quaternion inputRotation, float deltaTime)
        {
            if (_lookCameraPlane != Vector3.zero)
            {
                inputRotation = Quaternion.LookRotation(_lookCameraPlane, motor.CharacterUp);
            }


        }



        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            if (_lookCameraPlane != Vector3.zero)
            {
               currentRotation = Quaternion.LookRotation(_lookCameraPlane, motor.CharacterUp);
            }
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            Vector3 targetMovementVelocity = Vector3.zero;
            if (motor.GroundingStatus.IsStableOnGround)
            {
                // Reorient velocity on slope
                currentVelocity = motor.GetDirectionTangentToSurface(currentVelocity, motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;

                // Calculate target velocity
                Vector3 inputRight = Vector3.Cross(_moveInputVector, motor.CharacterUp);
                Vector3 reorientedInput = Vector3.Cross(motor.GroundingStatus.GroundNormal, inputRight).normalized * _moveInputVector.magnitude;
                targetMovementVelocity = reorientedInput * MaxStableMoveSpeed;

                // Smooth movement Velocity
                currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-StableMovementSharpness * deltaTime));
            }
            else
            {
                // Add move input
                if (_moveInputVector.sqrMagnitude > 0f)
                {
                    targetMovementVelocity = _moveInputVector * MaxAirMoveSpeed;

                    // Prevent climbing on un-stable slopes with air movement
                    if (motor.GroundingStatus.FoundAnyGround)
                    {
                        Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(motor.CharacterUp, motor.GroundingStatus.GroundNormal), motor.CharacterUp).normalized;
                        targetMovementVelocity = Vector3.ProjectOnPlane(targetMovementVelocity, perpenticularObstructionNormal);
                    }

                    Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, Gravity);
                    currentVelocity += velocityDiff * AirAccelerationSpeed * deltaTime;
                }

                // Gravity
                currentVelocity += Gravity * deltaTime;

                // Drag
                currentVelocity *= (1f / (1f + (Drag * deltaTime)));
            }
        }

        //These Functions are from the ICharacterController interface, but I haven't implemented them yet. 
        #region Not Yet Implemented
        public void AfterCharacterUpdate(float deltaTime)
        {

        }

        public void BeforeCharacterUpdate(float deltaTime)
        { 

        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            return true;
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {

        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {

        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {

        }

        public void PostGroundingUpdate(float deltaTime)
        {

        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {

        }
        #endregion


    }
}

using Cinemachine;
using FPS_Demo.Camera;
using FPS_Demo.KCC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FPS_Demo.PlayerController
{
    public class PlayerInputController : MonoBehaviour
    {
        [SerializeField]
        private PlayerKinematicController kinController;

        public CameraController cameraController;

        //New input system variables.
        private PlayerInput playerInput;
        private InputAction moveAction;
        private InputAction lookAction;


        private Vector2 _lookDelta = Vector2.zero;
        private Vector2 _moveVector = Vector2.zero;


        // Start is called before the first frame update
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Awake()
        {

            this.playerInput = GetComponent<PlayerInput>(); 
            moveAction = playerInput.actions["Move"];
            lookAction = playerInput.actions["Look"];
        }

        

        // Update is called once per frame
        void Update()
        {
            HandleCameraProcessing();
            HandleControllerProcessing();

            kinController.PostInputUpdate(Time.deltaTime);
        }

        private void HandleControllerProcessing()
        {
            PlayerInputState inputState = new PlayerInputState();
            Vector2 _moveVector = moveAction.ReadValue<Vector2>();

            inputState.MoveAxisRight = _moveVector.x;
            inputState.MoveAxisForward = _moveVector.y;
            inputState.CameraRotation = cameraController.GetCameraRotation();

            kinController.SetPlayerInput(ref inputState);
        }


        private void HandleCameraProcessing()
        {
            Vector2 lookVector = lookAction.ReadValue<Vector2>();
            cameraController.SetLookVector(lookVector);
        }
    }
}

using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FPS_Demo.Camera
{
    public class CameraController : MonoBehaviour
    {
        private CinemachineVirtualCamera CM_Vcam;

        private Vector2 _lookDelta;

        private const string MOUSEX = "Mouse X";
        private const string MOUSEY = "Mouse Y";

        // Start is called before the first frame update
        void Start()
        {
            CM_Vcam = GetComponent<CinemachineVirtualCamera>();

            CinemachineCore.GetInputAxis = GetAxisCustom;
        }

        private float GetAxisCustom(string axisName)
        {
            switch (axisName)
            {
                case MOUSEX:
                    return _lookDelta.x;
                case MOUSEY:
                    return _lookDelta.y;
            }

            return 0.0f;
        }

        public void SetLookVector(Vector2 look)
        {
            _lookDelta = look;
        }

        public Quaternion GetCameraRotation()
        {
            return CM_Vcam.State.RawOrientation;
        }
        

    }

}

using System;
using UnityEngine;

namespace Battleplan
{
    public class CameraController : MonoBehaviour
    {
        private Camera _camera;
        private Vector3 _mouseWorldOrigin;

        public float ZoomSpeed = 10;

        public void Start()
        {
            _camera = GetComponent<Camera>();
        }

        public void LateUpdate()
        {
            var zoom = Input.GetAxis("Mouse ScrollWheel");

            if (Math.Abs(zoom) > 0.001)
            {
                _camera.orthographicSize -= zoom * ZoomSpeed;
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                _mouseWorldOrigin = _camera.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetKey(KeyCode.Mouse1))
            {
                PanCameraWorld(Input.mousePosition);
            }
        }

        private void PanCameraWorld(Vector3 position)
        {
            var worldPosition = _camera.ScreenToWorldPoint(position);
            var worldDelta = _mouseWorldOrigin - worldPosition;

            transform.Translate(worldDelta, Space.World);
        }
    }
}

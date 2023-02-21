using UnityEngine;

namespace UnscriptedLogic.Raycast
{
    public static class RaycastLogic
    {
        public static bool FromCameraCenter(Camera camera, out RaycastHit hit, float distance = 100f)
        {
            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
            return Physics.Raycast(ray, out hit, distance);
        }

        public static bool FromCameraCenter(Camera camera, LayerMask layer, out RaycastHit hit, float distance = 100f)
        {
            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
            return Physics.Raycast(ray, out hit, distance, layer);
        }

        public static bool FromCameraCenterGetComp<T>(Camera camera, out T? component, float distance = 100f)
        {
            if (FromCameraCenter(camera, out RaycastHit hit, distance))
            {
                return hit.collider.TryGetComponent(out component);
            }

            component = default;
            return false;
        }

        public static bool FromCameraCenterGetComp<T>(Camera camera, LayerMask layer, out T? component, float distance = 100f)
        {
            if (FromCameraCenter(camera, layer, out RaycastHit hit, distance))
            {
                return hit.collider.TryGetComponent(out component);
            }

            component = default;
            return false;
        }

        public static bool FromMousePos3D(Camera camera, out RaycastHit hit, float distance = 100f)
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out hit, distance);
        }

        public static bool FromMousePos3D(Camera camera, out RaycastHit hit, LayerMask layer, float distance = 100f)
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out hit, distance);
        }
    }
}

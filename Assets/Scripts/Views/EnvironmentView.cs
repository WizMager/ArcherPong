using UnityEngine;

namespace Views
{
    public class EnvironmentView : MonoBehaviour
    {
        [SerializeField] private GameObject shootlessArea;
        [SerializeField] private Transform joystickPosition;

        public ShootlessAreaView GetShootlessAreaView => shootlessArea.GetComponent<ShootlessAreaView>();
        public Transform GetJoystickPosition => joystickPosition;
    }
}
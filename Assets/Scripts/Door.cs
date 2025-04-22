using UnityEngine;

namespace DoorScript
{
    [RequireComponent(typeof(AudioSource), typeof(Collider))]
    public class Door : MonoBehaviour
    {
        [Header("Rotation Settings")]
        public bool open = false;
        public float smooth = 1.0f;
        public float DoorOpenAngle = -90f;
        public float DoorCloseAngle = 0f;

        [Header("Key Unlock")]
        [Tooltip("Assign the key prefab required to unlock this door. Leave null for no key needed.")]
        public GameObject requiredKeyPrefab;

        [Header("Sound")]
        public AudioSource asource;
        public AudioClip openDoor;
        public AudioClip closeDoor;

        void Start()
        {
            asource = GetComponent<AudioSource>();
        }

        void Update()
        {
            // Smoothly swing door between closed and open angles
            Quaternion target = open
                ? Quaternion.Euler(0f, DoorOpenAngle, 0f)
                : Quaternion.Euler(0f, DoorCloseAngle, 0f);

            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                target,
                Time.deltaTime * 5f * smooth
            );
        }

        public void ToggleDoor()
        {
            open = !open;
            asource.clip = open ? openDoor : closeDoor;
            asource.Play();
        }
    }
}

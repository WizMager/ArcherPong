using UnityEngine;

public class Arrow : MonoBehaviour
{
        [SerializeField] private float moveSpeed = 3f;

        private void Start()
        {
                //Destroy(gameObject, 3f);
        }

        private void Update()
        {
                transform.Translate(transform.up * moveSpeed * Time.deltaTime);
        }
}
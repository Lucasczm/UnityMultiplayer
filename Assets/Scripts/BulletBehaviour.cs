using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public float bulletSpeed;
    void Start()
    {
        Destroy(gameObject, 3);
    }
    void FixedUpdate()
    {
        transform.position += transform.TransformDirection(Vector3.forward) * Time.fixedDeltaTime * bulletSpeed;
    }
}

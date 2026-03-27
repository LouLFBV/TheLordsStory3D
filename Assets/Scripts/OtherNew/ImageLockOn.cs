using UnityEngine;

public class ImageLockOn : MonoBehaviour
{
    public float rotationSpeed = 180f; // Vitesse de rotation en degrés par seconde

    void Update()
    {
        // Rotation continue autour de l'axe Y
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}
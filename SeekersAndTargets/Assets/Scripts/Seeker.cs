using UnityEngine;

public class Seeker : MonoBehaviour
{
    public Vector3 Direction;

    void Update()
    {
        transform.position += Direction * Time.deltaTime;
    }
}

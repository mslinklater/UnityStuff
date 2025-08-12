using UnityEngine;

public class Target : MonoBehaviour
{
    public Vector3 Direction;

    void Update()
    {
        transform.position += Direction * Time.deltaTime;
    }
}

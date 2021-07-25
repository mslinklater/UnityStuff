using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointLocation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		string name = "JointLocation.png";
		Gizmos.DrawIcon(transform.position, name, true);
	}
#endif
}

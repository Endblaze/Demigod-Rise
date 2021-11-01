using UnityEngine;

public class MenuCamera_Positon : MonoBehaviour
{

    public Transform p1, p2;

    private Vector3 center;

    private void Update()
    {

        center = p1.position;
        center = p1.position + new Vector3(Vector3.Distance(p1.position, p2.position) / 2, .4f, 0);

        transform.LookAt(center);

        transform.position = center + new Vector3(0, 0, -Vector3.Distance(p1.position, p2.position));

    }
    
}
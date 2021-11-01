using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Limit : MonoBehaviour
{

    private MeshRenderer mr;

    //This script shows the map limits when players collide with it
    //Also it reverses the normals of the limits

    //Start
    private void Start()
    {

        ReverseNormals();

        mr = GetComponent<MeshRenderer>();

        mr.enabled = false;

    }


    //Collisions
    private void OnCollisionEnter(Collision collision)
    {

        mr.enabled = true;

    }

    private void OnCollisionExit(Collision collision)
    {

        mr.enabled = false;

    }

    //Reverse normals
    private void ReverseNormals()
    {
        MeshFilter filter = GetComponent(typeof(MeshFilter)) as MeshFilter;
        if (filter != null)
        {
            Mesh mesh = filter.mesh;

            Vector3[] normals = mesh.normals;
            for (int i = 0; i < normals.Length; i++)
                normals[i] = -normals[i];
            mesh.normals = normals;

            for (int m = 0; m < mesh.subMeshCount; m++)
            {
                int[] triangles = mesh.GetTriangles(m);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    int temp = triangles[i + 0];
                    triangles[i + 0] = triangles[i + 1];
                    triangles[i + 1] = temp;
                }
                mesh.SetTriangles(triangles, m);
            }
        }

        this.GetComponent<MeshCollider>().sharedMesh = filter.mesh;

    }

}
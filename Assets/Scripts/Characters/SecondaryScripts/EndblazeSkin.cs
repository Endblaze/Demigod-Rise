using UnityEngine;

public class EndblazeSkin : MonoBehaviour
{

    private Renderer rend;

    private void Start()
    {

        rend = transform.GetChild(0).GetComponent<Renderer>();

    }

    void Update()
    {
        rend.material.mainTextureOffset += new Vector2(0, -Time.deltaTime/5);
    }

}
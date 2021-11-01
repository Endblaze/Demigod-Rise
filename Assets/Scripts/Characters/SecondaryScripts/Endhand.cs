using UnityEngine;

public class Endhand : MonoBehaviour
{

    public GameObject hitPref;
    private GameObject hit;
    private Hitbox_Manager hitS;

    public Transform hitPoint;

    public float damage, speed;

    private void Awake()
    {

        hit = Instantiate(hitPref);
        hitS = hit.GetComponent<Hitbox_Manager>();

    }

    private void OnEnable()
    {
        ActivateHit();
    }

    private void Update()
    {

        transform.position += new Vector3(0, -speed, 0) * Time.deltaTime;

        if (transform.position.y < -2)
        {
            hit.SetActive(false);
            gameObject.SetActive(false);
        }

        if(!hit.activeSelf)
        {
            ActivateHit();
        }

    }

    public void ActivateHit()
    {

        hit.SetActive(true);
        hitS.SetTarget(hitPoint, damage, 10);

        if (hit.tag == "Untagged")
        {
            if (tag == "PlayerOne")
            {
                hit.tag = "Hit_PlayerOne";
            }
            else if (tag == "PlayerTwo")
            {
                hit.tag = "Hit_PlayerTwo";
            }
        }

    }

}
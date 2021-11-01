using UnityEngine;

public class EnergyBall : MonoBehaviour
{

    public GameObject hitPref;

    public GameObject explosionObject;
    private ParticleSystem explosionParticle;

    private GameObject hit;

    public float damage, speed, timeLife;

    private void Awake()
    {

        explosionObject = Instantiate(explosionObject);
        explosionParticle = explosionObject.GetComponent<ParticleSystem>();

        if (GameObject.Find("Game_Controller")){
            explosionObject.transform.parent = GameObject.Find("Game_Controller").transform;
        }

    }

    private void Update()
    {

        transform.position += transform.forward * speed * Time.deltaTime;

        if (!hit.activeSelf)
        {

            explosionObject.transform.position = transform.position;
            explosionParticle.Play();
            
            gameObject.SetActive(false);

        }

    }

    public void SetValues(string tg, Vector3 pos, Vector3 dir)
    {

        tag = tg;
        transform.position = pos;
        transform.forward = dir;

        hit = PoolManager.Instance.RequestPool(tag, transform, damage, timeLife);

    }

}
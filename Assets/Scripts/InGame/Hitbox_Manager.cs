using UnityEngine;

public class Hitbox_Manager : MonoBehaviour
{

    private Transform target;

    public float damage;

    public float lifeTime;         //Tiempo en segundos para desaparecer

    public GameObject hitParticle;

    private GameObject hitParticleObject;
    private ParticleSystem hitParticleSystem;

    private float hitCounter;

    private float originalTime;

    private void OnDisable()
    {

        if(hitCounter <= 0) { return; }

        if (hitParticleObject != null)
        {

            hitParticleObject.transform.position = transform.position;
            hitParticleSystem.Play();

        }

        transform.position -= new Vector3(0, 10, 0);

    }

    private void Awake()
    {

        originalTime = lifeTime;


        hitParticleObject = Instantiate(hitParticle);
        hitParticleSystem = hitParticleObject.GetComponent<ParticleSystem>();

        if (GameObject.Find("Game_Controller"))
        {
            transform.parent = GameObject.Find("Game_Controller").transform;
            hitParticleObject.transform.parent = GameObject.Find("Game_Controller").transform;
        }

        hitParticleSystem.Stop();

    }

    private void Update()
    {
        
        transform.position = target.position;
        transform.eulerAngles = target.eulerAngles;

        if (hitCounter > 0)
        {
            hitCounter -= Time.deltaTime;
        }
        else
        {
            gameObject.SetActive(false);
        }
        
    }

    public void SetTarget(Transform t, float dmg, float time)
    {

        if (t != null)
        {
            target = t;
        }
        else
        {
            target = transform;
        }

        damage = dmg;

        if (time != 0)
        {
            lifeTime = time;
        }
        else
        {
            lifeTime = originalTime;
        }

        hitCounter = lifeTime;

    }

}
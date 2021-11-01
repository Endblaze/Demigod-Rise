using System.Collections;
using UnityEngine;

public class Spider_Manager : MonoBehaviour
{

    //Public Variables
    public float speed;
    public float damage;
    public float attackTime;
    public float maxDistance;

    public GameObject hitbox;           //Hitbox pref

    private Animator anim;              //Animator

    private Transform target, summoner; //Players

    private bool attack;                //AttackMode
    private bool addDmg;                //Add damage on AttackMode

    private float dir;                  //Random Movement's direction

    private GameObject hit;             //Access to instantiated pref
    private Hitbox_Manager hitScript;

    private float hitCd;                //Cooldown between each hit

    private void Awake()
    {

        transform.position = new Vector3(transform.position.x, -1, transform.position.z);

        anim = GetComponent<Animator>();

        hit = Instantiate(hitbox);
        hitScript = hit.GetComponent<Hitbox_Manager>();

        hit.SetActive(false);

        StartCoroutine("RandomMovement");


    }

    private void Update()
    {

        //Spider climb
        if (transform.position.y < 0)
        {

            if (Game_Controller.Instance.initialTime <= 0)
            {

                transform.position += new Vector3(0, Time.deltaTime, 0);

            }
            
            return;
        
        }


        //Active harmless hit
        if (!hit.activeSelf && hitCd > 0.5f)
        {

            hit.SetActive(true);
            hitScript.SetTarget(transform, 0, 100);

            if (attack) { attack = false; }

            hitCd = 0;

        }
        
        hitCd += Time.deltaTime;

        //Attack mode
        if (attack)
        {

            transform.LookAt(new Vector3(target.position.x, 0, target.position.z));

            transform.position += transform.forward * speed * 2 * Time.deltaTime;

            if (addDmg)
            {

                if (!hit.activeSelf) { hit.SetActive(true); }
                hitScript.SetTarget(transform, damage, attackTime);

                addDmg = false;

            }

        }
        //Random Movement and Follow Ally
        else if(Time.timeScale != 0)
        {

            if (Vector3.Distance(transform.position, summoner.position) < maxDistance)
            {
                
                if(transform.eulerAngles.x != 0 || transform.eulerAngles.z != 0) {
                    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                }

                transform.Rotate(0, dir / 2, 0);
                transform.position += transform.forward * speed * Time.deltaTime * Mathf.Abs(dir);

            }
            else
            {

                transform.LookAt(new Vector3(summoner.position.x, 0, summoner.position.z));
                transform.position += transform.forward * speed * Time.deltaTime;

            }

        }

    }

    //Animator Udpdate
    private void LateUpdate()
    {

        if (attack || Vector3.Distance(transform.position, summoner.position) >= maxDistance)
        {
            anim.SetInteger("Speed", 1);
        }
        else
        {
            anim.SetInteger("Speed", (int)dir);
        }

    }

    //Random Movement Logic
    private IEnumerator RandomMovement()
    {

        while (true)
        {

            dir = Random.Range(-1, 2);

            if (dir == 0) { dir = Random.Range(-1, 2); }

            if (dir == 0)
            {

                yield return new WaitForSeconds(Random.Range(1, 2));

            }
            else
            {

                yield return new WaitForSeconds(Random.Range(0.2f, 1));

            }

        }

    }

    public void SetSpider(Transform ally, Transform enemy)
    {

        summoner = ally;
        target = enemy;

        string playerTag = ally.tag;

        if (playerTag == "PlayerOne")
        {
            hit.tag = "Hit_PlayerOne";
        }
        else
        {
            hit.tag = "Hit_PlayerTwo";
        }

        

    }

    public void Attack()
    {

        attack = true;
        addDmg = true;

    }

}
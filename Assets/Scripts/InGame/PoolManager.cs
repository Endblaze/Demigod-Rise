using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{

    private static PoolManager _instance;

    static public PoolManager Instance
    {
        get
        {
            return _instance;
        }
    }


    private GameObject prefab1, prefab2;

    [SerializeField]
    private List<GameObject> pool1, pool2;
    private List<Hitbox_Manager> scriptPool1, scriptPool2;

    private List<GameObject> energyballPool;
    private List<EnergyBall> energyballScript;
    private List<GameObject> fireballPool;
    private List<EnergyBall> fireballScript;

    public GameObject energyballPref;
    public GameObject fireballPref;

    private void Awake()
    {

        _instance = this;

    }

    private void Start()
    {

        GeneratePool();

    }

    private void GeneratePool()
    {

        prefab1 = GameObject.FindGameObjectWithTag("PlayerOne").GetComponent<Player_Manager>().hitbox;
        prefab2 = GameObject.FindGameObjectWithTag("PlayerTwo").GetComponent<Player_Manager>().hitbox;

        //Hit

        pool1 = new List<GameObject>();
        pool2 = new List<GameObject>();
        scriptPool1 = new List<Hitbox_Manager>();
        scriptPool2 = new List<Hitbox_Manager>();

        for (int i = 0; i < 2; i++)
        {

            GameObject pref = Instantiate(prefab1, new Vector3(0, -10, 0), Quaternion.identity);

            pref.transform.parent = transform;

            pref.tag = "Hit_PlayerOne";

            pref.SetActive(false);

            pool1.Add(pref);
            scriptPool1.Add(pref.GetComponent<Hitbox_Manager>());

        }

        for (int i = 0; i < 2; i++)
        {

            GameObject pref = Instantiate(prefab2, new Vector3(0, -10, 0), Quaternion.identity);

            pref.transform.parent = transform;

            pref.tag = "Hit_PlayerTwo";

            pref.SetActive(false);

            pool2.Add(pref);
            scriptPool2.Add(pref.GetComponent<Hitbox_Manager>());

        }

        //Energyball

        energyballPool = new List<GameObject>();
        energyballScript = new List<EnergyBall>();

        for (int i = 0; i < 2; i++)
        {

            GameObject pref = Instantiate(energyballPref, new Vector3(0, -10, 0), Quaternion.identity);

            pref.transform.parent = transform;

            pref.SetActive(false);

            energyballPool.Add(pref);
            energyballScript.Add(pref.GetComponent<EnergyBall>());

        }

        //Fireball

        fireballPool = new List<GameObject>();
        fireballScript = new List<EnergyBall>();

        for (int i = 0; i < 2; i++)
        {

            GameObject pref = Instantiate(fireballPref, new Vector3(0, -10, 0), Quaternion.identity);

            pref.transform.parent = transform;

            pref.SetActive(false);

            fireballPool.Add(pref);
            fireballScript.Add(pref.GetComponent<EnergyBall>());

        }

    }

    //Request hit
    public GameObject RequestPool(string playerTag, Transform t, float dmg, float time)
    {

        if (playerTag == "PlayerOne")
        {

            for (int i = 0; i < pool1.Count; i++)
            {

                if (pool1[i].activeSelf == false)
                {

                    pool1[i].SetActive(true);
                    scriptPool1[i].SetTarget(t, dmg, time);

                    return pool1[i];

                }

            }

            GameObject newPfb = Instantiate(prefab1, new Vector3(0, -10, 0), Quaternion.identity);

            newPfb.transform.parent = transform;
            newPfb.tag = "Hit_PlayerOne";

            newPfb.GetComponent<Hitbox_Manager>().SetTarget(t, dmg, time);

            pool1.Add(newPfb);
            scriptPool1.Add(newPfb.GetComponent<Hitbox_Manager>());

            return newPfb;

        }
        
        if (playerTag == "PlayerTwo")
        {

            for (int i = 0; i < pool2.Count; i++)
            {

                if (pool2[i].activeSelf == false)
                {

                    pool2[i].SetActive(true);
                    scriptPool2[i].SetTarget(t, dmg, time);

                    return pool2[i];

                }

            }

            GameObject newPfb = Instantiate(prefab2, new Vector3(0,-10,0), Quaternion.identity);

            newPfb.transform.parent = transform;
            newPfb.tag = "Hit_PlayerTwo";

            newPfb.GetComponent<Hitbox_Manager>().SetTarget(t, dmg, time);

            pool2.Add(newPfb);
            scriptPool2.Add(newPfb.GetComponent<Hitbox_Manager>());

            return newPfb;

        }

        return null;

    }

    //Request energyball (Protoxking)
    public void RequestEnergyball(string tag, Vector3 pos, Vector3 dir)
    {

        for (int i = 0; i < energyballPool.Count; i++)
        {

            if (energyballPool[i].activeSelf == false)
            {

                energyballPool[i].SetActive(true);
                energyballScript[i].SetValues(tag, pos, dir);

                return;

            }

        }

        GameObject newPfb = Instantiate(energyballPref, new Vector3(0, -10, 0), Quaternion.identity);

        newPfb.transform.parent = transform;
        newPfb.GetComponent<EnergyBall>().SetValues(tag, pos, dir);

        energyballPool.Add(newPfb);
        energyballScript.Add(newPfb.GetComponent<EnergyBall>());

    }

    //Request fireball (Endblaze)
    public void RequestFireball(string tag, Vector3 pos, Vector3 dir)
    {

        for (int i = 0; i < fireballPool.Count; i++)
        {

            if (fireballPool[i].activeSelf == false)
            {

                fireballPool[i].SetActive(true);
                fireballScript[i].SetValues(tag, pos, dir);

                return;

            }

        }

        GameObject newPfb = Instantiate(fireballPref, new Vector3(0, -10, 0), Quaternion.identity);

        newPfb.transform.parent = transform;
        newPfb.GetComponent<EnergyBall>().SetValues(tag, pos, dir);

        fireballPool.Add(newPfb);
        fireballScript.Add(newPfb.GetComponent<EnergyBall>());

    }

}
using UnityEngine;

public class Singleton : MonoBehaviour
{

    public bool persistent = true;

    private bool initiated = false;

    virtual public void Awake()
    {
        this.MakeSingleton();
    }

    protected bool MakeSingleton()
    {

        if (this.initiated)
        {

            return (false);

        }

        this.initiated = true;

        string singletonName = this.name + "Singleton";

        if(GameObject.Find(singletonName) != null)
        {

            this.gameObject.tag = "Untagged";

            Object.Destroy(this.gameObject);

            return (false);

        }

        this.gameObject.name = singletonName;

        if (this.persistent)
        {
            DontDestroyOnLoad(this.gameObject);
        }

        return true;

    }

}
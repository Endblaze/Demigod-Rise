using System.IO;
using UnityEngine;

public class Controls_Manager:MonoBehaviour
{

    private static Controls_Manager _instance;

    public static Controls_Manager Instance
    {

        set { _instance = value; }

        get { return _instance; }
    
    }

    private void Start()
    {

        _instance = this;

        LoadValues();

    }

    private string route;

    public Controls_Object controls;

    private void LoadValues()
    {

        route = Application.persistentDataPath + "/GameControls.json";

        if (File.Exists(route))
        {

            string readJson = File.ReadAllText(route);

            controls = JsonUtility.FromJson<Controls_Object>(readJson);

        }
        else
        {

            ControlsDefaultValues();

        }

    }

    public void SaveControls()
    {

        string stringControls = JsonUtility.ToJson(controls);

        File.WriteAllText(route, stringControls);

    }

    public void ControlsDefaultValues()
    {

        controls = new Controls_Object();

        SaveControls();

    }

}
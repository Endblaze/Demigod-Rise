using UnityEngine;
using UnityEngine.UI;

public class Game_Controller : MonoBehaviour
{

    private GameObject p1, p2;                                  //GameObjects para preparar a los dos jugadores en el escenario

    public GameObject[] characters;                             //Array con todos los personajes jugables

    private Image crazyModeMask;

    [HideInInspector]
    public Slider lifebar1, lifebar2, energybar1, energybar2;   //Sliders de la interfaz de combate

    private Text timer, initialTimer, p1Name, p2Name;           //Textos de la interfaz de combate

    [HideInInspector]
    public float time, initialTime;                             //Temporizadores

    [HideInInspector]
    public static float gameMode;                               //Modo de juego

    //HitCounter

    private Text hitText1, hitText2;
    private float hitTextTimer1, hitTextTimer2;

    private float hitCounter1, hitCounter2;
    private float hitTimer1, hitTimer2;

    private static Game_Controller _instance;                   //Singleton

    //Singleton
    public static Game_Controller Instance
    {
        get
        {

            return _instance;

        }
    }

    //Awake
    private void Awake()
    {

        _instance = this;

        //Game Mode
        gameMode = PlayerPrefs.GetFloat("GameMode");

        LoadPlayers();
        LoadInterface();

    }

    //Update
    private void Update()
    {   

        UpdateInterface();

        HitCounter();
        
    }

    //Carga de personajes y modalidad (PJ O PNJ)
    private void LoadPlayers()
    {

        p1Name = GameObject.Find("PlayerOneName").GetComponent<Text>();
        p2Name = GameObject.Find("PlayerTwoName").GetComponent<Text>();
        
        //Players Character Selection
        p1 = characters[(int)PlayerPrefs.GetFloat("Character1", 0)];

        if (gameMode != 0)
        {
            p2 = characters[(int)PlayerPrefs.GetFloat("Character2", 0)];
        }
        else
        {
            p2 = characters[(int)PlayerPrefs.GetFloat("DemigodRiseEnemy", 0)];
        }

        p1Name.text = characters[(int)PlayerPrefs.GetFloat("Character1", 0)].name;
        p2Name.text = characters[(int)PlayerPrefs.GetFloat("Character2", 0)].name;

        p1 = Instantiate(p1, new Vector3(0, -10, 0), Quaternion.identity);
        p2 = Instantiate(p2, new Vector3(0, -10, 0), Quaternion.identity);

        p1.tag = "PlayerOne";
        p2.tag = "PlayerTwo";

        if (PlayerPrefs.GetString("p1Selector") == "CPU" && gameMode != 0)
        {
            p1.GetComponent<Player_Manager>().EnableAI();
        }

        if (PlayerPrefs.GetString("p2Selector") == "CPU" || gameMode == 0)
        {
            p2.GetComponent<Player_Manager>().EnableAI();
        }

    }

    //Carga de interfaz en el combate
    private void LoadInterface()
    {

        //Lifebars
        lifebar1 = GameObject.Find("LifebarPlayerOne").GetComponent<Slider>();
        lifebar2 = GameObject.Find("LifebarPlayerTwo").GetComponent<Slider>();

        //Energybars
        energybar1 = GameObject.Find("EnergybarPlayerOne").GetComponent<Slider>();
        energybar2 = GameObject.Find("EnergybarPlayerTwo").GetComponent<Slider>();

        lifebar1.value = 0;
        lifebar2.value = 0;

        energybar1.value = 1;
        energybar2.value = 1;

        //Timer
        initialTimer = GameObject.Find("InitialTimer").GetComponent<Text>();
        initialTime = 4f;

        timer = GameObject.Find("Timer").GetComponent<Text>();
        time = 60;
        if (gameMode != 2) { timer.text = time + ""; } else { timer.text = "00"; }

        //CrazyMode Image
        crazyModeMask = GameObject.Find("CrazyModeImage").GetComponent<Image>();
        Color col = crazyModeMask.color;
        col.a = 0;
        crazyModeMask.color = col;

        //HitCounter
        hitText1 = GameObject.Find("HitCounter1").GetComponent<Text>();
        hitText2 = GameObject.Find("HitCounter2").GetComponent<Text>();

        hitText1.text = "";
        hitText2.text = "";

    }


    //Update de la interfaz a lo largo del combate, dependiendo de cada modo
    private float elapsedTime = 0;

    private void UpdateInterface()
    {

        //Cuenta atrás
        if (lifebar1.value > 0 && lifebar2.value > 0 && gameMode != 2 && initialTime <= 0)
        {

            time -= Time.deltaTime;

        }

        //Modo entrenamiento
        if (gameMode == 2) {

            if (initialTimer.gameObject.activeSelf)
            {

                initialTimer.gameObject.SetActive(false);
                initialTime = 0;

            }

            return;
        
        }
        //Update normal - Normal Font
        if (initialTime > 0)
        {

            initialTime -= Time.deltaTime;

            if (Time.timeScale != 0)
            {

                elapsedTime += Time.deltaTime;

                lifebar1.value += Mathf.Lerp(0, 1, elapsedTime / 10f);
                lifebar2.value = lifebar1.value;

                energybar1.value += Mathf.Lerp(0, 1, elapsedTime / 10f);
                energybar2.value = energybar1.value;

            }

            if (initialTime > 1)
            {
                initialTimer.text = (int)initialTime + "";
            }
            else
            {

                initialTimer.fontSize++;

                Color color = initialTimer.color;
                color.a -= Time.deltaTime * 2;

                initialTimer.color = color;

                initialTimer.text = "GO!";

            }

        }
        //Update con contador menor a 0 - Red Font
        else
        {

            if (initialTimer.gameObject.activeSelf) {

                if(initialTimer.color.a <= 0)
                initialTimer.gameObject.SetActive(false);
            
            }

            timer.text = (int)time + "";

            if (time <= 0)
            {

                if (timer.color != Color.red)
                {

                    timer.color = Color.red;

                    RenderSettings.skybox = crazyModeMaterial;

                }

                CrazyMode();

            }

        }

    }

    private bool increase;
    private float alpha;

    public Material crazyModeMaterial;

    private void CrazyMode()
    {

        if (increase)
        {

            if(alpha < 1)
            {
                alpha += Time.deltaTime / 2;
            }
            else
            {
                increase = false;
            }

        }
        else
        {

            if (alpha > .5f)
            {
                alpha -= Time.deltaTime / 2;
            }
            else
            {
                increase = true;
            }

        }

        Color col = crazyModeMask.color;
        col.a = alpha;

        crazyModeMask.color = col;

    }

    private Color col;

    private void HitCounter()
    {

        //Player 1
        if(hitTextTimer1 > 0)
        {

            hitTextTimer1 -= Time.deltaTime;

            hitText1.transform.localScale += Vector3.one * 2 * Time.deltaTime;

        }
        else if (hitText1.color.a > 0)
        {

            col = hitText1.color;
            col.a -= Time.deltaTime;
            
            hitText1.color = col;
            hitText1.transform.localScale -= Vector3.one * Time.deltaTime;

        
        }

        if(hitTimer1 > 0)
        {
            hitTimer1 -= Time.deltaTime;
        }
        else if (hitCounter1 != 0)
        {
            hitCounter1 = 0;
        }


        //Player 2
        if (hitTextTimer2 > 0)
        {

            hitTextTimer2 -= Time.deltaTime;

            hitText2.transform.localScale += Vector3.one * 2 * Time.deltaTime;

        }
        else if (hitText2.color.a > 0)
        {

            col = hitText2.color;
            col.a -= Time.deltaTime;

            hitText2.color = col;
            hitText2.transform.localScale -= Vector3.one * Time.deltaTime;


        }

        if (hitTimer2 > 0)
        {
            hitTimer2 -= Time.deltaTime;
        }
        else if (hitCounter2 != 0)
        {
            hitCounter2 = 0;
        }

    }

    public void ResetHitCounter(string player)
    {

        int index;

        if(player == "PlayerOne")
        {
            index = 2;
        }
        else
        {
            index = 1;
        }

        //Player 1
        if(index == 1)
        {

            hitCounter1++;

            hitText1.text = hitCounter1 + "\nHITS!";

            hitTimer1 = 1.5f;

            hitTextTimer1 = .1f;

            col = hitText1.color;
            col.a = 1;

            hitText1.color = col;
            hitText1.transform.localScale = Vector3.one;

            return;

        }

        //Player 2

        hitCounter2++;

        hitText2.text = hitCounter2 + "\nHITS!";

        hitTimer2 = 1.5f;

        hitTextTimer2 = .1f;

        col = hitText2.color;
        col.a = 1;

        hitText2.color = col;
        hitText2.transform.localScale = Vector3.one;

    }

}
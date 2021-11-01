using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu_Manager : MonoBehaviour
{

    public GameObject main, options, selector;


    //Options
    public Slider volumeMain, volumeMenu, volumeMusic, volumeSFX;

    public Toggle fullScreen;

    public GameObject controlsPanel, newButtonPanel;

    private int buttonIndex;

    public Text[] txtControls;

    private bool closeOptions = true;

    //Character Selector
    public Text p1Selector, p2Selector;

    public GameObject[] characters;

    public GameObject[] sceneImages;

    public RectTransform imgSelector1, imgSelector2, imgSelectorScenary;

    //Demigod Rise Mode

    public GameObject[] buttons;

    //Menu Sounds
    public MenuSounds_Manager msM;

    //Start
    void Start()
    {

        main.SetActive(true);
        options.SetActive(true);
        selector.SetActive(false);

        UpdateScreen();

        //Opciones iniciales Menu de Seleccion

        
        if (PlayerPrefs.GetFloat("CharacterSelect") == 1) //Al abrir el menú de selección desde el combate
        {

            main.SetActive(false);
            selector.SetActive(true);

            PlayerPrefs.SetFloat("CharacterSelect", 0);

        }

        if (PlayerPrefs.GetFloat("Scenary") == 0)
        {
            PlayerPrefs.SetFloat("Scenary", 1);
        }

        p1Selector.text = PlayerPrefs.GetString("p1Selector", "Player 1");
        p2Selector.text = PlayerPrefs.GetString("p2Selector", "Player 2");

        CharacterSelector(PlayerPrefs.GetFloat("Character1", 0));
        CharacterSelector(PlayerPrefs.GetFloat("Character2", 0) + characters.Length / 2);

        for (int i = 0; i < sceneImages.Length; i++)
        {

            if (i != PlayerPrefs.GetFloat("Scenary", 1) - 1)
            {
                sceneImages[i].SetActive(false);
            }
            else
            {

                sceneImages[i].SetActive(true);

                Vector3 newPos = imgSelectorScenary.anchoredPosition;
                newPos.x = ((i) * 320) + 640;
                imgSelectorScenary.anchoredPosition3D = newPos;

            }

        }

        if (PlayerPrefs.GetFloat("GameMode") == 0)
        {
            DemigodRiseModeSettings();
        }

        //Controles en el menú de opciones
        LoadControls();

    }

    //Update
    private void Update()
    {

        if (closeOptions)
        {
            options.SetActive(false);
            closeOptions = false;
        }

        if (newButtonPanel.activeSelf)
        {

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                newButtonPanel.SetActive(false);
            }
            else if (Input.anyKeyDown)
            {
                ChangeButton();
            }
        }

    }

    //Update Screen Mode
    private void UpdateScreen()
    {

        fullScreen.isOn = PlayerPrefs.GetFloat("FullScreen", 1) == 1;
        Screen.fullScreen = fullScreen.isOn;

    }

    //Set default values in options
    public void DefaultValues()
    {

        //Sound
        volumeMain.value = .7f;
        volumeMusic.value = .4f;
        volumeSFX.value = 1;
        volumeMenu.value = 1;

        //FullScreen
        PlayerPrefs.SetFloat("FullScreen", 1);

        UpdateScreen();

        //Default Controls
        Controls_Manager.Instance.ControlsDefaultValues();
        LoadControls();

    }

    //Load saved controls
    public void LoadControls()
    {

        for (int i = 0; i < txtControls.Length; i++)
        {

            if (i < 6)
            {
                txtControls[i].text = Controls_Manager.Instance.controls.player1[i] + "";
            }
            else
            {
                txtControls[i].text = Controls_Manager.Instance.controls.player2[i - 6] + "";
            }

        }

    }

    //Detect the pressed button on Controls Panel
    public void GetButtonIndex(int index)
    {

        newButtonPanel.SetActive(true);

        buttonIndex = index;

    }

    //Change an specific control on Controls Panel
    private void ChangeButton()
    {

        KeyCode newButton = new KeyCode();

        foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(vKey))
            {
                newButton = vKey;
            }
        }

        if (buttonIndex < 6)
        {

            Controls_Manager.Instance.controls.player1[buttonIndex] = newButton;

        }
        else
        {

            Controls_Manager.Instance.controls.player2[buttonIndex - 6] = newButton;

        }

        Controls_Manager.Instance.SaveControls();

        LoadControls();

        newButtonPanel.SetActive(false);

    }

    //Open Options Menu
    public void OpenOptions()
    {

        msM.PlayAudio(3);

        main.SetActive(false);
        selector.SetActive(false);
        options.SetActive(true);

    }

    //Open Main Menu
    public void OpenMenu()
    {

        main.SetActive(true);
        selector.SetActive(false);
        options.SetActive(false);

        controlsPanel.SetActive(false);

    }

    //DemigodRise Mode Menu Settings
    private void DemigodRiseModeSettings()
    {

        p1Selector.text = "Player 1";

        sceneImages[sceneImages.Length - 1].SetActive(true);

        foreach (var btn in buttons)
        {
            btn.SetActive(false);
        }

        for (int i = characters.Length / 2; i < characters.Length; i++)
        {

            characters[i].SetActive(false);

        }

        imgSelectorScenary.gameObject.SetActive(false);
        imgSelector2.gameObject.SetActive(false);

    }

    //Open Character Selector according the game mode
    public void OpenSelector(float gameMode)
    {

        PlayerPrefs.SetFloat("GameMode", gameMode);

        msM.PlayAudio((int)gameMode);

        main.SetActive(false);
        selector.SetActive(true);
        options.SetActive(false);

        if(gameMode == 0)
        {

            DemigodRiseModeSettings();

        }
        else
        {

            sceneImages[sceneImages.Length - 1].SetActive(false);

            p1Selector.text = PlayerPrefs.GetString("p1Selector", "Player 1");

            foreach (var btn in buttons)
            {
                btn.SetActive(true);
            }

            for (int i = characters.Length / 2; i < characters.Length; i++)
            {

                CharacterSelector(PlayerPrefs.GetFloat("Character2") + characters.Length / 2);

            }

            imgSelectorScenary.gameObject.SetActive(true);
            imgSelector2.gameObject.SetActive(true);

        }

    }

    //Change screen mode
    public void ChangeScreen()
    {

        Screen.fullScreen = fullScreen.isOn;

        PlayerPrefs.SetFloat("FullScreen", fullScreen.isOn ? 1 : 0);

    }

    //Change if the character is controlled by a real player or the CPU
    public void ChangeCharacterController(bool p1)
    {

        if(PlayerPrefs.GetFloat("GameMode") == 0) { return; }

        if (p1)
        {

            if (p1Selector.text == "CPU")
            {
                p1Selector.text = "Player 1";
            }
            else
            {
                p1Selector.text = "CPU";
            }

            PlayerPrefs.SetString("p1Selector", p1Selector.text);

        }
        else
        {

            if (p2Selector.text == "CPU")
            {
                p2Selector.text = "Player 2";
            }
            else
            {
                p2Selector.text = "CPU";
            }

            PlayerPrefs.SetString("p2Selector", p2Selector.text);

        }

    }

    //Change the scenary selection
    public void ScenarySelector(float index)
    {

        PlayerPrefs.SetFloat("Scenary", index);

        sceneImages[(int)index - 1].SetActive(true);

        for(int i=0; i<sceneImages.Length; i++)
        {

            if(i != index - 1)
            {
                sceneImages[i].SetActive(false);
            }
            else
            {
                sceneImages[i].SetActive(true);
            }

        }

        Vector3 newPos = imgSelectorScenary.anchoredPosition;
        newPos.x = ((index - 1) * 320) + 640;
        imgSelectorScenary.anchoredPosition3D = newPos;

    }

    //Change the character selection
    public void CharacterSelector(float index)
    {

        if (index <= (characters.Length/2)-1)
        {

            PlayerPrefs.SetFloat("Character1", index);

            for (int i = 0; i < characters.Length/2; i++)
            {

                if (i != index)
                {

                    characters[i].SetActive(false);

                }
                else
                {

                    characters[(int)index].SetActive(true);

                }

            }

            Vector3 newPos = imgSelector1.anchoredPosition;
            newPos.x = index * 320 + 160;
            imgSelector1.anchoredPosition3D = newPos;

        }
        else
        {

            PlayerPrefs.SetFloat("Character2", index-3);

            for (int i = 3; i < characters.Length; i++)
            {

                if (i != index)
                {

                    characters[i].SetActive(false);

                }
                else
                {

                    characters[(int)index].SetActive(true);

                }

            }

            Vector3 newPos = imgSelector2.anchoredPosition;
            newPos.x = - (((index - 3) * 320) + 160);
            imgSelector2.anchoredPosition3D = newPos;

        }

    }

    //Start the fight
    public void GoBattle()
    {

        if (PlayerPrefs.GetFloat("GameMode") == 0)
        {

            PlayerPrefs.SetFloat("DemigodRiseEnemy", Random.Range(0, characters.Length / 2));
            PlayerPrefs.SetFloat("DemigodRiseScenary", Random.Range(1, sceneImages.Length));
            PlayerPrefs.SetFloat("DemigodRiseWins", 0);

            switch (PlayerPrefs.GetFloat("DemigodRiseScenary"))
            {

                case 1:
                    SceneManager.LoadScene("RockScene");
                    break;

                case 2:
                    SceneManager.LoadScene("ForestScene");
                    break;

                case 3:
                    SceneManager.LoadScene("IceScene");
                    break;

            }

        }
        else
        {

            switch (PlayerPrefs.GetFloat("Scenary"))
            {

                case 1:
                    SceneManager.LoadScene("RockScene");
                    break;

                case 2:
                    SceneManager.LoadScene("ForestScene");
                    break;

                case 3:
                    SceneManager.LoadScene("IceScene");
                    break;

            }

        }

    }

    //Exit Game
    public void ExitGame()
    {

        Application.Quit();

    }

    //Show Players Controls
    public void ShowControls()
    {

        controlsPanel.SetActive(!controlsPanel.activeSelf);

    }

}
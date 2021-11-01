using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game_Menu : MonoBehaviour
{

    private bool pause;

    private GameObject pausePanel, endPanel;

    private Text txtWinner;

    private bool endShowable = true;

    //Demigod Rise

    private GameObject demigodPanel;

    private Button btnNextBattle;

    private Text txtWins;

    //Awake
    private void Awake()
    {

        pausePanel = GameObject.Find("PausePanel");
        endPanel = GameObject.Find("EndPanel");
        demigodPanel = GameObject.Find("DemigodRisePanel");

        txtWinner = GameObject.Find("txt_winner").GetComponent<Text>();

        txtWins = GameObject.Find("txt_wins").GetComponent<Text>();

        btnNextBattle = GameObject.Find("btn_nextBattle").GetComponent<Button>();

        pausePanel.SetActive(false);
        endPanel.SetActive(false);
        demigodPanel.SetActive(false);

    }

    //Update
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && endShowable)
        {

            PauseResumeGame();

        }
        
        if((Game_Controller.Instance.lifebar1.value <= 0 || Game_Controller.Instance.lifebar2.value <= 0) && Game_Controller.gameMode != 2)
        {

            if (Game_Controller.Instance.lifebar1.value <= 0 && Game_Controller.Instance.lifebar2.value <= 0) { txtWinner.text = "DRAW!"; } else
            if (Game_Controller.Instance.lifebar1.value <= 0) { txtWinner.text = "PLAYER 2 WINS!"; } else
            if (Game_Controller.Instance.lifebar2.value <= 0) { txtWinner.text = "PLAYER 1 WINS!"; }

            if (endShowable && Game_Controller.Instance.initialTime <= 0)
            {

                endShowable = false;

                StartCoroutine("EndBattle");

            }

        }


    }

    //End Battle Menu
    private IEnumerator EndBattle()
    {

        Time.timeScale = 0.8f;

        yield return new WaitForSeconds(2);

        pausePanel.SetActive(false);

        if (Game_Controller.gameMode != 0)
        {
            endPanel.SetActive(true);
        }
        else
        {

            if(Game_Controller.Instance.lifebar1.value <= 0)
            {
                btnNextBattle.interactable = false;
            }
            else
            {
                PlayerPrefs.SetFloat("DemigodRiseWins", PlayerPrefs.GetFloat("DemigodRiseWins") + 1);
            }

            if(PlayerPrefs.GetFloat("DemigodRiseWins") > PlayerPrefs.GetFloat("DemigodRiseRecord"))
            {
                PlayerPrefs.SetFloat("DemigodRiseRecord", PlayerPrefs.GetFloat("DemigodRiseWins"));
            }

            txtWins.text = "WINS: " + PlayerPrefs.GetFloat("DemigodRiseWins", 0) + "\nRECORD OF WINS: " + PlayerPrefs.GetFloat("DemigodRiseRecord", 0);

            demigodPanel.SetActive(true);
        
        }

    }

    //Next Battle Panel, DemigodRise Mode
    public void DemigodRiseNextBattle()
    {

        PlayerPrefs.SetFloat("DemigodRiseEnemy", Random.Range(0, 3));
        PlayerPrefs.SetFloat("DemigodRiseScenary", Random.Range(1, 4));

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

    //Pause
    public void PauseResumeGame()
    {

        pause = !pause;

        pausePanel.SetActive(pause);

        Time.timeScale = pause ? 0f : 1;

    }

    //Return to Main Menu
    public void ReturnToMenu()
    {

        Time.timeScale = 1;

        SceneManager.LoadScene(0, LoadSceneMode.Single);

    }

    //Return to Character Selector
    public void CharacterSelect()
    {

        Time.timeScale = 1;

        PlayerPrefs.SetFloat("CharacterSelect", 1);
        SceneManager.LoadScene(0, LoadSceneMode.Single);

    }

}
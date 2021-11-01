using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LockedPlayables : MonoBehaviour
{

    //Este script controla si puedes usar o no ciertos personajes y escenarios en base al avance en el modo DemigodRise, activando los cheats desde el menú se desbloquean todos

    public Toggle toggleCheats;

    private bool cheats, reset;

    public Button[] playables;

    private void Awake()
    {

        if (toggleCheats.isOn != (PlayerPrefs.GetFloat("Cheats") != 0 ? true : false))
        {

            toggleCheats.isOn = PlayerPrefs.GetFloat("Cheats") != 0 ? true : false;

        }
        else
        {

            CheckUnlockables();

        }

    }

    public void CheckUnlockables()
    {

        cheats = toggleCheats.isOn;

        if (reset && !cheats)
        {

            PlayerPrefs.SetFloat("Character1", 0);
            PlayerPrefs.SetFloat("Character2", 0);
            PlayerPrefs.SetFloat("Scenary", 1);

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        }
        else
        {
            reset = true;
        }

        PlayerPrefs.SetFloat("Cheats", cheats ? 1 : 0);

        float record = PlayerPrefs.GetFloat("DemigodRiseRecord");

        for(int i = 0; i < playables.Length; i++)
        {
            if(playables[i].interactable != false)
            {
                playables[i].interactable = false;
            }
        }

        if (record >= 3 || cheats)
        {

            playables[0].interactable = true;
            playables[1].interactable = true;

            if (record >= 4 || cheats)
            {

                playables[2].interactable = true;

                if (record >= 5 || cheats)
                {

                    playables[3].interactable = true;
                    playables[4].interactable = true;

                    if (record >= 6 || cheats)
                    {

                        playables[5].interactable = true;

                    }

                }

            }

        }

    }

}
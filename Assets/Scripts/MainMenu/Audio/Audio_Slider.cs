using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Audio_Slider : MonoBehaviour
{

    public AudioMixer aMixer;
    public enum AudioChannel { Master, Menu, SFX, Music }
    public AudioChannel channel;

    private Slider slider;

    public Text slText;
    public int lowestDeciblesBeforeMute = -20; //Sonido más bajo en decibelios antes del mute

    public AudioClip[] menuExampleSounds;

    //Awake
    private void Awake()
    {

        slider = GetComponent<Slider>();

    }

    //Start
    private void Start()
    {

        //Obtenemos el valor guardado por el jugador (el segundo parámetro es el valor por defecto que saldrá)
        int masterVolume = PlayerPrefs.GetInt("MasterVolume", 70);
        int menuVolume = PlayerPrefs.GetInt("MenuVolume", 100);
        int soundVolume = PlayerPrefs.GetInt("SoundVolume", 100);
        int musicVolume = PlayerPrefs.GetInt("MusicVolume", 40);

        //Cambiamos el valor de los slider dependiendo del channel
        switch (channel)
        {

            case AudioChannel.Master:
                slider.value = masterVolume / 100f;
                break;

            case AudioChannel.Menu:
                slider.value = menuVolume / 100f;
                break;

            case AudioChannel.SFX:
                slider.value = soundVolume / 100f;
                break;

            case AudioChannel.Music:
                slider.value = musicVolume / 100f;
                break;

        }

        //Actualizamos los valores de audio y texto
        UpdateSoundLvl();

    }

    //Update
    private void Update()
    {

        UpdateSoundLvl();

        if (Input.GetKeyDown(KeyCode.Space))
        {

            //Guardar datos y cerrar
            PlayerPrefs.Save();
            Application.Quit();

        }

    }

    //Actualizar valores de audio y texto
    public void UpdateSoundLvl()
    {

        float sValue = slider.value * 100;

        SetVolume(channel, (int)sValue); //Establecer volumen

        slText.text = channel + ": " + (int)sValue + " / 100"; //Cambiar texto

    }

    public void SetVolume(AudioChannel channel, int volume)
    {

        //Transformación a decibelios
        float adjustedVolume = lowestDeciblesBeforeMute + (-lowestDeciblesBeforeMute / 5 * volume / 20);

        //Mute
        if (volume == 0)
        {
            adjustedVolume = -100;
        }

        //Establecemos el nuevo volumen y lo guardamos en PlayerPrefs 
        switch (channel)
        {

            case AudioChannel.Master:
                aMixer.SetFloat("MasterVolume", adjustedVolume);
                PlayerPrefs.SetInt("MasterVolume", volume);
                break;

            case AudioChannel.Menu:
                aMixer.SetFloat("MenuVolume", adjustedVolume);
                PlayerPrefs.SetInt("MenuVolume", volume);
                break;

            case AudioChannel.SFX:
                aMixer.SetFloat("SoundVolume", adjustedVolume);
                PlayerPrefs.SetInt("SoundVolume", volume);
                break;

            case AudioChannel.Music:
                aMixer.SetFloat("MusicVolume", adjustedVolume);
                PlayerPrefs.SetInt("MusicVolume", volume);
                break;

        }

    }

    public void PlayExampleSound(AudioSource aS)
    {

        if (channel == AudioChannel.SFX)
        {

            if (!aS.isPlaying)
            {
                aS.PlayOneShot(aS.clip);
                aS.pitch = Random.Range(0.5f, 1.5f);
            }

        }
        
        if (channel == AudioChannel.Menu)
        {

            if (!aS.isPlaying)
            {
                aS.PlayOneShot(menuExampleSounds[ Random.Range(0, menuExampleSounds.Length) ]);
            }

        }

    }

}
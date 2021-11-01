using UnityEngine;

public class MenuSounds_Manager : MonoBehaviour
{

    //Este script reproduce los audios que se escuchan en el menú (sin contar la música)

    private AudioSource aS;

    public AudioClip[] snds;

    private void Awake()
    {

        aS = GetComponent<AudioSource>();

    }

    //Play AudioClip
    public void PlayAudio(int index)
    {

        aS.PlayOneShot(snds[index]);

    }

}
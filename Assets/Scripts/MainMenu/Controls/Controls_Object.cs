using UnityEngine;

public class Controls_Object
{

    public KeyCode[] player1, player2;

    public Controls_Object()
    {

        player1 = new KeyCode[6];
        player2 = new KeyCode[6];

        DefaultValues();

    }

    //Función que se activa al pulsar Default Values en Options
    public void DefaultValues()
    {

        player1[0] = KeyCode.D;
        player1[1] = KeyCode.A;
        player1[2] = KeyCode.W;
        player1[3] = KeyCode.S;
        player1[4] = KeyCode.V;
        player1[5] = KeyCode.C;

        player2[0] = KeyCode.RightArrow;
        player2[1] = KeyCode.LeftArrow;
        player2[2] = KeyCode.UpArrow;
        player2[3] = KeyCode.DownArrow;
        player2[4] = KeyCode.P;
        player2[5] = KeyCode.O;

    }

}
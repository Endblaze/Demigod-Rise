using UnityEngine;

public class Camera_Manager : MonoBehaviour
{

    private Transform playerOne, playerTwo;

    public float speed, minDistance, maxDistance; //Parámetros de la cámara

    private float _speed;

    private Vector3 center, camPosition;

    private GameObject pivot;

    private float distance;

    //Start
    private void Start()
    {

        playerOne = GameObject.FindGameObjectWithTag("PlayerOne").transform;
        playerTwo = GameObject.FindGameObjectWithTag("PlayerTwo").transform;

        pivot = new GameObject();

        pivot.transform.position = playerOne.position;
        pivot.transform.parent = playerOne;

    }

    //Update
    private void Update()
    {

        pivot.transform.LookAt(playerTwo.position);

        CameraControl();

    }

    private void CameraControl()
    {

        distance = Vector3.Distance(playerOne.localPosition, playerTwo.localPosition);

        center = playerOne.position + (playerTwo.position - playerOne.position).normalized * distance/2;

        if (distance < maxDistance)
        {
            camPosition = center + pivot.transform.right * (Mathf.Abs(distance) + minDistance);
        }
        else
        {
            camPosition = center + pivot.transform.right * maxDistance;
        }

        camPosition.y = 1;
        center.y = 1;

        if(distance < minDistance)
        {
            _speed = speed * 2;
        }
        else
        {
            _speed = speed;
        }

        transform.position = Vector3.Slerp(transform.position, camPosition, _speed * Time.deltaTime);
        transform.LookAt(center);

    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    readonly float rotationSpeed = 50;
    readonly float rotMax = 310, rotMin = 50;
    //float totalRotation = 0;
    //float fovMin = 55;
    //float fovMax = 85;
    private Camera mainCamera;
    readonly float speed = 75;
    readonly float YMAX = 95, YMIN = 10;

    GameObject cameraHolder;
    readonly float horVerSpeed = 50;
    float posX, posY, posZ;
    readonly float XMAX = 120, XMIN = 0, ZMAX = 110, ZMIN = -10;

    float timer = 0;
    readonly float resetTimer = 4;
    // Use this for initialization
    void Awake()
    {
        mainCamera = GetComponent<Camera>();
        cameraHolder = GameObject.FindGameObjectWithTag("CamerHolder");
        posX = mainCamera.transform.position.x;
        posY = mainCamera.transform.position.y;
        posZ = mainCamera.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraHolder.transform.eulerAngles.x <= rotMax && cameraHolder.transform.eulerAngles.x >= (rotMax - 10))
        {
            cameraHolder.transform.Rotate(rotationSpeed * Time.deltaTime * Time.deltaTime, 0, 0, Space.Self);
        }
        else if (cameraHolder.transform.eulerAngles.x <= rotMin && cameraHolder.transform.eulerAngles.x >= 0)
        {
            cameraHolder.transform.Rotate(-rotationSpeed * Time.deltaTime * Time.deltaTime, 0, 0, Space.Self);
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            //mainCamera.transform.position = new Vector3(posX, mainCamera.transform.position.y + speed * Time.deltaTime, posZ);
            posY += speed * Time.deltaTime;
            if (cameraHolder.transform.eulerAngles.x >= rotMax || (cameraHolder.transform.eulerAngles.x <= rotMin && cameraHolder.transform.eulerAngles.x <= 0))
            {
                cameraHolder.transform.Rotate(rotationSpeed * Time.deltaTime, 0, 0, Space.Self);
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            posY -= speed * Time.deltaTime;
            //mainCamera.transform.position = new Vector3(posX, mainCamera.transform.position.y - speed * Time.deltaTime, posZ);
            if (cameraHolder.transform.eulerAngles.x >= rotMax || (cameraHolder.transform.eulerAngles.x <= rotMin && cameraHolder.transform.eulerAngles.x <= 0))
            {
                cameraHolder.transform.Rotate(-rotationSpeed * Time.deltaTime, 0, 0, Space.Self);
            }
            
        }
        if (Input.GetKey(KeyCode.W))
        {
            posZ += horVerSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            posX -= horVerSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            posZ -= horVerSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            posX += horVerSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            cameraHolder.transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0, Space.World); // * 0.7f
        }
        if (Input.GetKey(KeyCode.E))
        {
            cameraHolder.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.World);
        }
        if (Input.GetKey(KeyCode.R))
        {
            cameraHolder.transform.eulerAngles = new Vector3(0, 0, 0);
            timer = 0;
        }

        if (timer <= resetTimer && cameraHolder.transform.eulerAngles.y != 0)// || cameraHolder.transform.eulerAngles.z == 0))
        {
            timer += Time.deltaTime;
        }
        if (timer > resetTimer)
        {
            //cameraHolder.transform.eulerAngles = new Vector3(cameraHolder.transform.eulerAngles.x, 0, 0);
            timer = 0;
        }

        mainCamera.transform.position = new Vector3(Mathf.Clamp(posX, XMIN, XMAX), 
            Mathf.Clamp(posY, YMIN, YMAX), Mathf.Clamp(posZ, ZMIN, ZMAX));

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class MoveCamera : MonoBehaviour {

    public Button CamResetButton;
    private bool isChangingCamPos;

    private Color startColor;
    private Vector3 startPos;
    private Quaternion startRot;
    private bool save;

	// Use this for initialization
	void Start () {
        initCameraPosition();
        isChangingCamPos = false;
        save = false;
        CamResetButton.onClick.AddListener(delegate
        {
            changeCameraPositionInit();
        });
        
    }

    private void changeCameraPositionInit()
    {
        isChangingCamPos = true;
        startPos = this.gameObject.transform.position;
        startRot = this.gameObject.transform.rotation;

        startColor = CamResetButton.image.color;
        CamResetButton.image.color = Color.red;
        CamResetButton.onClick.RemoveAllListeners();

        CamResetButton.onClick.AddListener(delegate
        {
            save = true;
            saveCameraPositionInit();
        });
    }

    private void saveCameraPositionInit()
    {
        isChangingCamPos = false;

        CamResetButton.image.color = startColor;
        CamResetButton.onClick.RemoveAllListeners();

        CamResetButton.onClick.AddListener(delegate
        {
            changeCameraPositionInit();
        });
        if (save)
        {
            saveCameraPosition();
        }
    }

    private void initCameraPosition()
    {
        if(PlayerPrefs.HasKey("CamPosX") && PlayerPrefs.HasKey("CamRotX"))
        {
            float x = PlayerPrefs.GetFloat("CamPosX");
            float y = PlayerPrefs.GetFloat("CamPosY");
            float z = PlayerPrefs.GetFloat("CamPosZ");
            this.gameObject.transform.position = new Vector3(x, y, z);

            x = PlayerPrefs.GetFloat("CamRotX");
            y = PlayerPrefs.GetFloat("CamRotY");
            z = PlayerPrefs.GetFloat("CamRotZ");
            this.gameObject.transform.rotation = Quaternion.Euler(x, y, z);
        }
    }

    private void changeCameraPosition()
    {
        if (GamePad.GetState(0).IsConnected)
        {
            if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 || Input.GetAxis("UpDown") != 0)
            {
                Vector3 newPos = this.gameObject.transform.position;

                newPos.x += Input.GetAxis("Horizontal") * Time.deltaTime;
                newPos.y += Input.GetAxis("UpDown") * Time.deltaTime;
                newPos.z += Input.GetAxis("Vertical") * Time.deltaTime;
                
                this.gameObject.transform.position = newPos;
            }
            
            if(Input.GetAxis("Rotate") != 0)
            {
                Vector3 newRotation = this.transform.rotation.eulerAngles;
                newRotation.y += Input.GetAxis("Rotate") * Time.deltaTime;
                this.gameObject.transform.rotation = Quaternion.Euler(newRotation);
            }
        }
             
    }

    private void restoreCameraPosition()
    {
        this.gameObject.transform.position = startPos;
        this.gameObject.transform.rotation = startRot;
    }

    private void saveCameraPosition()
    {
        PlayerPrefs.SetFloat("CamPosX", this.gameObject.transform.position.x);
        PlayerPrefs.SetFloat("CamPosY", this.gameObject.transform.position.y);
        PlayerPrefs.SetFloat("CamPosZ", this.gameObject.transform.position.z);
        PlayerPrefs.SetFloat("CamRotX", this.gameObject.transform.rotation.eulerAngles.x);
        PlayerPrefs.SetFloat("CamRotY", this.gameObject.transform.rotation.eulerAngles.y);
        PlayerPrefs.SetFloat("CamRotZ", this.gameObject.transform.rotation.eulerAngles.z);
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown("joystick button 4"))
        {
            if (!isChangingCamPos)
            {
                changeCameraPositionInit();
            }
            else
            {
                restoreCameraPosition();
                saveCameraPositionInit();
            }
        }
        
        if (isChangingCamPos)
        {
            if (Input.GetKeyDown("joystick button 5"))
            {
                save = true;
                saveCameraPositionInit();
            }
            else
            {
                changeCameraPosition();
            }
        }
    }


}

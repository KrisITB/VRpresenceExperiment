using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class ExperimentVIEW : MonoBehaviour
{
    public InputField InputSampleID;
    public Toggle InputToggleGroup1;
    public Toggle InputToggleGroup2;
    public Button ButtonStart;
    public Text StatusDisplayText;
    public Slider ProgressTotal;
    public Slider ProgressPhase;
    public Slider Fidelity;

    public ExperimentCTRL ExperimentController;
    public ExperimentMODEL ExperimentModel;

    private int sampleID;
    private int groupID;

    private ColorBlock defaultColors;
    private ColorBlock newCB;

    public Button QuitButton;
    public Button RecordButton;
    //private bool isRecording;

    int below50;
    private float totalExperimentTime = 0f;
    
    void Start()
    {
        newCB = ButtonStart.colors;
        defaultColors = newCB;
        below50 = 0;
    }

    public void INIT() // called by controler
    {
        //set up initial values disable toggles and start enable sample input #:
        InputSampleID.interactable = true;
        InputToggleGroup1.interactable = false;
        InputToggleGroup2.interactable = false;
        ButtonStart.interactable = false;
        StatusDisplayText.text = "Enter Sample ID#";

        if (ExperimentModel != null)
        {
            totalExperimentTime = ExperimentModel.GetTotalExperimentTimeLength();
        }
        else
        {
            Debug.LogError("ExperimentModell is null");
        }
        
        //set up listeners to enable group selection
        InputSampleID.onValueChanged.AddListener(delegate {
            enableGroupSelection();
        });

        //set up listeners to enable start button
        InputToggleGroup1.onValueChanged.AddListener(delegate {
            enableStartButton();
        });
        InputToggleGroup2.onValueChanged.AddListener(delegate {
            enableStartButton();
        });
        /*
        isRecording = false;
        RecordButton.onClick.AddListener(delegate
        {
            startRecording();
        });
        */
        ButtonStart.onClick.AddListener(delegate
        {
            validateAndStartExperiment();
        });
    }
    /*
    private void startRecording()
    {
        RecordButton.onClick.RemoveAllListeners();
        ColorBlock TempCB = RecordButton.colors;
        TempCB.disabledColor = Color.yellow;
        RecordButton.colors = TempCB;
        RecordButton.interactable = false;
        isRecording = true;

        ExperimentController.StartRecording();
    }
    */
    private void validateAndStartExperiment()
    {
        bool validateGroupID = false;

        if (InputToggleGroup1.isOn)
        {
            groupID = 1;
            validateGroupID = true;
        }
        else if (InputToggleGroup2.isOn)
        {
            groupID = 2;
            validateGroupID = true;
        }
        else
        {
            validateGroupID = false;
            StatusDisplayText.text = "invalid group ID \nmake sure to tick one of the groups";
        }

        bool validateSampleID = int.TryParse(InputSampleID.text, out sampleID);
        if (validateSampleID && validateGroupID)
        {
            InputSampleID.interactable = false;
            InputToggleGroup1.interactable = false;
            InputToggleGroup2.interactable = false;
            DisarmStartButton();

            ExperimentController.StartRecording();
            ExperimentController.StartExperiment(sampleID, groupID);
                       
            StatusDisplayText.text = "experiment started";
        }
        else
        {
            StatusDisplayText.text = "invalid sample ID \nonly integers allowed";
        }
    }

    private void enableStartButton()
    {
        if (ButtonStart.interactable == false)
        {
            ButtonStart.interactable = true;
            StatusDisplayText.text = "All set \npress start to collect data";
        }
    }

    private void enableGroupSelection()
    {
        if (InputSampleID.text.Length > 0)
        {
            InputToggleGroup1.interactable = true;
            InputToggleGroup2.interactable = true;
            StatusDisplayText.text = "Select the group #";
        }
    }    

    public void ArmStartButton()
    {
        // add code to make sure it does differently than last time
        ButtonStart.interactable = true;
        ButtonStart.onClick.AddListener(delegate
        {
            StartAdaptationPhaseVE();
        });
        StatusDisplayText.text = "Ready to collect data in\nVirtual Enviornmnet";
    }

    public void PrepStartButton()
    {
        newCB.disabledColor = Color.yellow;
        ButtonStart.colors = newCB;
    }

    public void StartAdaptationPhaseVE()
    {
        DisarmStartButton();
        if (GamePad.GetState(0).IsConnected)
        {
            GamePad.SetVibration(0, 0f, 0f);
        }
        ExperimentController.StartVirtualEnvironmentStage();
    }
    
    public void DisarmStartButton()
    {
        ButtonStart.colors = defaultColors;
        ButtonStart.onClick.RemoveAllListeners();
        ButtonStart.interactable = false;
    }

    public void UpdateDisplay(string phase, float totalTime, float inPhaseTime, float progressInPhase, float fidelity, int trialNumber, float fps)
    {
        ProgressTotal.value = totalTime / totalExperimentTime;
        ProgressPhase.value = progressInPhase;
        Fidelity.value = fidelity;

        if(fps < 50)
        {
            below50++;
        }

        StatusDisplayText.text = "phase : " + phase + "\ntotal time : \n" + totalTime + "\nin phase time : \n" + inPhaseTime + "\nTrial # : " + trialNumber + "\nFPS below 50 : " + below50;
    }

    public void UpdateDisplay(float fps)
    {
        StatusDisplayText.text = "fps : " + fps.ToString() + "\nApplication.persistentDataPath : \n" + Application.persistentDataPath.ToString();
    }

    public void UpdateDisplay(string text)
    {
        StatusDisplayText.text = text;
    }

    public void SetTotalExperimentTime(float t)
    {
        totalExperimentTime = t;
    }


    public void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

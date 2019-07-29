using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.XR;
using XInputDotNetPure;
using System.IO;

public class ExperimentCTRL : MonoBehaviour
{
    public ExperimentVIEW ExperimentDisplay;
    private int sampleID;
    private int groupID;

    public ExperimentMODEL ExperimentModel;
    private float controlPhaseLenght;
    private float adaptation_PE_PhaseLenght;
    private float adaptation_VE_PhaseLenght;
    private float interventionPhaseLenght;
    private float totalExperimentTime;
    private int noOfTrials;
    private int currentTrialNumber;
    private float trialLenght;
    private float periodLenght;
    private float samplingFreq;

    public AudioSource Audio1;
    private bool playedAudio1 = false;
    private string playingAudio = "none";
    public AudioSource Audio2;
    private bool playedAudio2 = false;
    private bool prepedStartButton = false;
    private bool armedStartButton = false;

    //time controllers
    private long experimentStartTime;
    private float currentExperimentTime;
    private float currentInPhaseTime;
    private long currentPhaseStartTime;
    private float currentInTrialTime;
    private float currentInPeriodTime;

    private int experimentPhase;
    /*
     * 0 - adaptation PE
     * 1 - adaptation VE
     * 2 - intervention
     * 3 - control
     * 4 - completed
     * 9 - paused
     * 88 - preparation
    */
    private float progressionThroughPhase;

    private int periodType;
    /*
     * 1 - decay = 1 -> 0
     * 2 - sustain low = 0
     * 3 - attack = 0 -> 1
     * 4 - sustain high = 1
    */
    private string periodTypeStr = "none";

    private float currentFidelity;

    //private float lastSamplingTime = 0f;

    private float pausedTime = 1f; 

    //gates
    private bool paused;
    private bool experimentIsOn;
    private bool interventionIsOn;

    public FOV_CTRL FovControler;
    private float currentFov;

    public PostProcessingCTRL PostProcessingControler;
    private float currentSaturation;

    public FpsCTRL FpsCtrl;
    private float currentFPS;
    private float threadSleep;

    public PolyCTRL PolyCtrl;
    private int currentModel = 0;

    public AppQualityCTRL AppQualityCtrl;
    private string currentQualityLevel;

    public ResolutionCTRL ResolutionCtrl;
    private string currentResolution;
    private string currentPixels;

    //private DataCollector Data;
    private bool isRecording;

    private Thread newThread;

    public SaveToCSV saveClass;
    private bool saving;

    long lastTick;
    float toSecondsMod = 10000000f;

    //float experimentStart = 0f;
    //float experimentTime = 0f;

    string phase;

    public GameObject Transition;

    public PolyCounter PolyCounter;
    private string polyCount;

    private int frameCounter = 0;
    
    string filePath;
    StreamWriter streamWriter;



    //set up initial bool values on gates:
    private void Start()
    {
        INIT();
    }

    private void Update()
    {
        
        if (isRecording)
        {
            frameCounter++; // increase frame count
            currentFPS = 1 / Time.deltaTime; // calc current fps
            if (!experimentIsOn && experimentPhase == 88) // init for adaptationPE
            {
                currentInPhaseTime = (System.DateTime.Now.Ticks - experimentStartTime) / toSecondsMod;
            }
            progressExperiment();
        }
        
    }

    private void INIT()
    {
        isRecording = false;
        paused = false;
        experimentIsOn = false;
        interventionIsOn = false;

        experimentPhase = 88; // set experiment phase to preparation
        if(ExperimentDisplay != null)
        {
            ExperimentDisplay.INIT();
        }
    }

    public void StartRecording()
    {
        experimentPreINIT();

        currentPhaseStartTime = System.DateTime.Now.Ticks;

        if(ExperimentModel != null)
        {
            samplingFreq = ExperimentModel.GetSamplingFrequency(); // set erlier or 0?!
        }
        else
        {
            Debug.LogError("Experiment Model is null");
        }

        //Data = new DataCollector();
        //string tempText = setDataSetHeader();
        //Debug.Log("Recording started for : " + tempText);

        isRecording = true;
        experimentStartTime = currentPhaseStartTime;
        newThread = new Thread(dataUpdate);
        newThread.Start();

        lastTick = currentPhaseStartTime;
    }

    private void experimentPreINIT()
    {
        //initial values for Preparation Phase
        experimentPhase = 88;
        phase = "Preparation";
        currentModel = 999;
        currentFov = 999f;
        currentFidelity = 999f;
        currentSaturation = 999f;
        currentQualityLevel = "PE";
        currentResolution = "PE";
        currentPixels = "PE";
        polyCount = "PE";
        threadSleep = 999f;
    }

    public void StartExperiment(int sampID, int grID)
    {
        experimentINIT(sampID, grID);

        experimentStartTime = System.DateTime.Now.Ticks;
        experimentIsOn = true;


        //StartCoroutine("progressExperiment");
        //InvokeRepeating("progressExperiment", 0f, samplingFreq); //used update instead
    }
    private void experimentINIT(int sampID, int grID)
    {
        sampleID = sampID;
        groupID = grID;

        adaptation_PE_PhaseLenght = ExperimentModel.GetAdaptation_PE_PhaseLenght();
        adaptation_VE_PhaseLenght = ExperimentModel.GetAdaptation_VE_PhaseLenght();
        interventionPhaseLenght = ExperimentModel.GetInterventionPhaseLenght();
        controlPhaseLenght = ExperimentModel.GetControlPhaseLenght();
        totalExperimentTime = ExperimentModel.GetTotalExperimentTimeLength(); // to ticks * 10000000f no pause included at this stage

        noOfTrials = ExperimentModel.GetNoOfTrials();
        trialLenght = ExperimentModel.GetTrialLenght();
        periodLenght = ExperimentModel.GetPeriodLenght();

        string prefix = "0";
        int sampIDlength = sampleID.ToString().Length;

        if (sampIDlength == 1)
        {
            prefix = "000";
        }
        else if(sampIDlength == 2)
        {
            prefix = "00";
        }
        else if(sampIDlength == 3)
        {
            prefix = "0";
        }
        else
        {
            prefix = "TEST-";
        }

        filePath = Application.persistentDataPath + "/Data/" + "Sample_Number-" + prefix + sampleID + "_Group_Number-" + groupID + ".csv";
        if (!File.Exists(filePath))
        {
            streamWriter = new StreamWriter(filePath);
        }
        else
        {
            bool noFile = false;
            int i = 1;
            while (!noFile)
            {
                filePath = Application.persistentDataPath + "/Data/" + "Sample_Number-" + prefix + sampleID + "_Group_Number-" + groupID + "_(" + i + ").csv";
                if (!File.Exists(filePath))
                {
                    streamWriter = new StreamWriter(filePath);
                    noFile = true;
                }
                else
                {
                    i++;
                }
            }
        }

        
        streamWriter.AutoFlush = true;
        setDataSetHeader();
    }

  
    void progressExperiment()
    {
        if (experimentIsOn)
        {
            setExperimentPhase();
            updatePhase();
            if (experimentPhase == 2)
            {
                moveThroughTrials();
            }
            updateView();
        }
    }

    private void setExperimentPhase()
    {
        long currentTime = System.DateTime.Now.Ticks;
        currentExperimentTime = (currentTime - experimentStartTime) / toSecondsMod;

        // set up phase based on time
        if (currentExperimentTime <= adaptation_PE_PhaseLenght)
        {
            if (experimentPhase != 0)
            {
                experimentPhase = 0;
                phase = "adaptation PE";
                currentPhaseStartTime = System.DateTime.Now.Ticks;
            }
        }
        else if (currentExperimentTime <= adaptation_PE_PhaseLenght + pausedTime || paused)
        {
            if (experimentPhase != 9)
            {
                experimentPhase = 9;
                phase = "break";
                currentPhaseStartTime = System.DateTime.Now.Ticks;

                currentQualityLevel = "transition";
                currentResolution = "transition";
                currentPixels = "transition";
                polyCount = "transition";
            }
        }
        else if (currentExperimentTime <= adaptation_PE_PhaseLenght + adaptation_VE_PhaseLenght + pausedTime)
        {
            if (experimentPhase != 1)
            {
                experimentPhase = 1;
                phase = "adaptation VE";
                currentPhaseStartTime = System.DateTime.Now.Ticks;

                currentModel = PolyCtrl.initModels();
                currentFov = ExperimentModel.GetHMDfov();
                currentQualityLevel = QualitySettings.names[QualitySettings.GetQualityLevel()];
                currentResolution = ResolutionCtrl.GetCurrentResolution();
                currentPixels = ResolutionCtrl.GetCurrentPixels();
                currentSaturation = ExperimentModel.GetCurrentSaturation();
                threadSleep = 0f;
                currentFidelity = 1f;
            }
        }
        else if (groupID == 1)
        {
            if (currentExperimentTime <= adaptation_PE_PhaseLenght + adaptation_VE_PhaseLenght + pausedTime + interventionPhaseLenght)
            {
                if (experimentPhase != 2)
                {
                    experimentPhase = 2;
                    phase = "intervention";
                    currentPhaseStartTime = System.DateTime.Now.Ticks;
                }
            }
            else if (currentExperimentTime <= totalExperimentTime)
            {
                if (experimentPhase != 3)
                {
                    experimentPhase = 3;
                    phase = "control";
                    currentPhaseStartTime = System.DateTime.Now.Ticks;
                }
            }
            else if (currentExperimentTime > totalExperimentTime)
            {
                if (experimentPhase != 4)
                {
                    experimentPhase = 4;
                    phase = "completed";
                    currentPhaseStartTime = System.DateTime.Now.Ticks;
                    experimentIsOn = false;
                    
                }
            }
            else
            {
                experimentPhase = 99;
                phase = "unknown";
                Debug.LogError("Whopps!");
            }
        }
        else if (groupID == 2)
        {
            if (currentExperimentTime <= adaptation_PE_PhaseLenght + adaptation_VE_PhaseLenght + pausedTime + controlPhaseLenght)
            {
                if (experimentPhase != 3)
                {
                    experimentPhase = 3;
                    phase = "control";
                    currentPhaseStartTime = System.DateTime.Now.Ticks;
                }
            }
            else if (currentExperimentTime <= totalExperimentTime)
            {
                if (experimentPhase != 2)
                {
                    experimentPhase = 2;
                    phase = "intervention";
                    currentPhaseStartTime = System.DateTime.Now.Ticks;
                }
            }
            else if (currentExperimentTime > totalExperimentTime)
            {
                if (experimentPhase != 4)
                {
                    experimentPhase = 4;
                    phase = "completed";
                    currentPhaseStartTime = System.DateTime.Now.Ticks;
                    experimentIsOn = false;
                }
            }
            else
            {
                experimentPhase = 99;
                phase = "unknown";
                Debug.LogError("Whopps!");
            }
        }
        else
        {
            Debug.LogError("Unknown group!!!");
        }
    }

    private void updatePhase()
    {
        //long currentTime = System.DateTime.Now.Ticks;
        //currentExperimentTime = (currentTime - experimentStartTime) / toSecondsMod;

        switch (experimentPhase)
        {
            case 0: //adaptation PE phase
                currentInPhaseTime = currentExperimentTime;
                progressionThroughPhase = currentInPhaseTime / adaptation_VE_PhaseLenght;
                break;
            case 1: // adaptation VE phase
                currentInPhaseTime = currentExperimentTime - adaptation_PE_PhaseLenght - pausedTime;
                progressionThroughPhase = currentInPhaseTime / adaptation_VE_PhaseLenght;
                break;
            case 2: // intervention phase
                if (!interventionIsOn)
                {
                    interventionIsOn = true;
                }
                if (groupID == 1)
                {
                    currentInPhaseTime = currentExperimentTime - adaptation_PE_PhaseLenght - adaptation_VE_PhaseLenght - pausedTime;
                }
                else if (groupID == 2)
                {
                    currentInPhaseTime = currentExperimentTime - adaptation_PE_PhaseLenght - adaptation_VE_PhaseLenght - pausedTime - controlPhaseLenght;
                }
                else
                {
                    Debug.LogError("Uknown group!");
                }
                progressionThroughPhase = currentInPhaseTime / interventionPhaseLenght;
                break;
            case 3: // control phase
                if (interventionIsOn)
                {
                    interventionIsOn = false;
                }
                if (periodType != 0)
                {
                    periodType = 0;
                    periodTypeStr = "none";
                    currentInPeriodTime = 0;
                }
                if (groupID == 1)
                {
                    currentInPhaseTime = currentExperimentTime - adaptation_PE_PhaseLenght - adaptation_VE_PhaseLenght - pausedTime - interventionPhaseLenght;
                }
                else if (groupID == 2)
                {
                    currentInPhaseTime = currentExperimentTime - adaptation_PE_PhaseLenght - adaptation_VE_PhaseLenght - pausedTime;
                }
                else
                {
                    Debug.LogError("Uknown group!");
                }
                progressionThroughPhase = currentInPhaseTime / controlPhaseLenght;
                break;
            case 4: // completed
                if (interventionIsOn)
                {
                    interventionIsOn = false;
                }
                if (progressionThroughPhase != 1)
                {
                    progressionThroughPhase = 1;
                }
                if (!playedAudio2)
                {
                    Audio2.Play();
                    playedAudio2 = true;
                }
                Debug.Log("Experiment completed");
                // arm save button here instead:
                cleanUp();
                break;
            case 88:
                currentInPhaseTime = currentExperimentTime;
                break;
            case 9: // paused
                onPause();
                break;
            default:
                Debug.LogError("Unknown phase!!!");
                break;
        }
    }

    private void onPause()
    {
        if (!paused)
        {
            paused = true;
            Audio1.Play();
        }

        pausedTime = (System.DateTime.Now.Ticks - experimentStartTime) / toSecondsMod - adaptation_PE_PhaseLenght;
        currentInPhaseTime = pausedTime;
        totalExperimentTime = ExperimentModel.GetTotalExperimentTimeLength() + pausedTime;
        ExperimentDisplay.SetTotalExperimentTime(totalExperimentTime);

        //Debug.Log("pausedTime : " + pausedTime);

        if (!playingAudio.Equals("Audio_1") && Audio1.isPlaying)
        {
            playingAudio = "Audio_1";
        }

        if (!prepedStartButton)
        {
            prepedStartButton = true;
            ExperimentDisplay.PrepStartButton();
        }

        if (!Audio1.isPlaying && playedAudio1 != true)
        {
            playedAudio1 = true;
            if (!playingAudio.Equals("none"))
            {
                playingAudio = "none";
            }
        }

        if (playedAudio1 && !armedStartButton)
        {
            if (GamePad.GetState(0).IsConnected)
            {
                GamePad.SetVibration(0, 1f, 1f);
            }
            armedStartButton = true;
            ExperimentDisplay.ArmStartButton();
        }
        if (armedStartButton)
        {
            if (GamePad.GetState(0).IsConnected)
            {
                if (Input.GetKeyDown("joystick button 7"))
                {
                    GamePad.SetVibration(0, 0f, 0f);
                    ExperimentDisplay.StartAdaptationPhaseVE();
                }
            }
        }
    }

    public void StartVirtualEnvironmentStage()
    {
        Transition.SetActive(false);
        paused = false;
    }
    

    private void moveThroughTrials()
    {
        if(currentInPhaseTime > currentTrialNumber * trialLenght)
        {
            currentTrialNumber++;
        }
        
        if (currentTrialNumber <= noOfTrials)
        {
            moveThroughPeriods();
        }
    }

    private void moveThroughPeriods()
    {
        currentInTrialTime = currentInPhaseTime - (currentTrialNumber - 1) * trialLenght;
       
        if (currentInTrialTime < periodLenght) 
        {
            periodType = 1;
            periodTypeStr = "decay";
        }
        else if(currentInTrialTime < periodLenght * 2)
        {
            periodType = 2;
            periodTypeStr = "sustain low";
        }
        else if (currentInTrialTime < periodLenght * 3)
        {
            periodType = 3;
            periodTypeStr = "attack";
        }
        else if (currentInTrialTime < periodLenght * 4)
        {
            periodType = 4;
            periodTypeStr = "sustain high";
        }
        else
        {
            periodType = 55;
            periodTypeStr = "error";
        }

        currentInPeriodTime = currentInTrialTime - (periodType - 1) * periodLenght;
        modulateFidelity(periodType);
        
    }

    private void modulateFidelity(int periodType)
    {
        
        switch (periodType)
        {
            case 1:
                currentFidelity = 1 - currentInPeriodTime / periodLenght;
                break;
            case 2:
                if (currentFidelity != 0f)
                {
                    currentFidelity = 0f;
                }
                break;
            case 3:
                currentFidelity = currentInPeriodTime / periodLenght;
                break;
            case 4:
                if (currentFidelity != 1f)
                {
                    currentFidelity = 1f;
                }
                break;
            default:
                Debug.LogError("Unknown Period Type!");
                break;
        }
        
        currentFidelity = ((int)(currentFidelity * 100)) / 100f; // round up to 2 decimal places for consistency;
        //modulation:
        modulate();
    }

    private void modulate()
    {
        currentFov = FovControler.ModulateFOV(currentFidelity);
        currentSaturation = PostProcessingControler.ModulateSaturation(currentFidelity);
        threadSleep = FpsCtrl.ModulateFrameRate(currentFidelity);
        currentModel = PolyCtrl.ModulateFidelity(currentFidelity) + 1;
        //currentQualityLevel = AppQualityCtrl.modulateAppQuality(currentFidelity);
        currentResolution = ResolutionCtrl.ModulateResolution(currentFidelity);
        currentPixels = ResolutionCtrl.GetCurrentPixels();
        //polyCount = PolyCounter.polyCounter();
    }

    private void updateView()
    {
        ExperimentDisplay.UpdateDisplay(phase, currentExperimentTime, currentInPhaseTime, progressionThroughPhase, currentFidelity, currentTrialNumber, currentFPS);
    }


    private string setDataSetHeader()
    {
        string dataToSend = "Time stamp,Ticks," +
            "Frame count," +
            "Phase,Time in Phase,Real Time in Phase," +
            "Trial Number,Trial Time," +
            "Period Type,Time in Period," +
            "Fidelity," +
            "Model," +
            "FOV," +
            "Saturation," +
            //"Quality Level," +
            "Resolution," + "Pixel count," +
            "ThreadSleep,FPS real," +
            "Audio Clip,";
        // + "Polygon count,";
        //Data.UpdateDataSet(dataToSend);
        saveData(dataToSend);
        return dataToSend;
    }

    private void dataUpdate()
    {
        while (!saving)
        {
            float realInPhaseTime = (System.DateTime.Now.Ticks - currentPhaseStartTime)/toSecondsMod;

            long ticksFromLastUpdate = System.DateTime.Now.Ticks - lastTick;
            float secondsFromLastUpdate = ticksFromLastUpdate / toSecondsMod;

            if (secondsFromLastUpdate >= samplingFreq)
            {
                
                //Data.UpdateDataSet(dataToUpdate);
                
                string dataToUpdate = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff") + "," + System.DateTime.Now.Ticks + "," +
                    frameCounter.ToString() + "," +
                    phase + "," + currentInPhaseTime.ToString() + "," + realInPhaseTime.ToString() + "," +
                    currentTrialNumber.ToString() + "," + currentInTrialTime.ToString() + "," +
                    periodTypeStr + "," + currentInPeriodTime.ToString() + "," +
                    currentFidelity.ToString() + "," +
                    currentModel.ToString() + "," +
                    currentFov.ToString() + "," +
                    currentSaturation.ToString() + "," +
                    //currentQualityLevel + "," +
                    currentResolution + "," + currentPixels + "," +
                    threadSleep.ToString() + "," + currentFPS.ToString() + "," +
                    playingAudio + ",";
                // + polyCount;
                saveData(dataToUpdate);

                lastTick = System.DateTime.Now.Ticks;
            }
            if (!isRecording)
            {
                Thread.Sleep(1000);
            }
        }
    }

    public void cleanUp()
    {
        if (isRecording)
        {
            isRecording = false;
            newThread.Abort();
        }
        saving = true;
        //saveClass.SaveCsv(sampleID, groupID, Data.GetDataSet());
    }

    private void saveData(string data)
    {
        streamWriter.WriteLine(data);
    }

    private void OnApplicationQuit()
    {
        streamWriter.Close();
        if (isRecording)
        {
            isRecording = false;
            if (newThread.IsAlive)
            {
                newThread.Abort();
                Debug.Log("newThread aborted");
            }
        }
    }
}



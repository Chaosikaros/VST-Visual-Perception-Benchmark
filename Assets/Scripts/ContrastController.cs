using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;
using System;
using Random = UnityEngine.Random;
using System.Globalization;

public class ContrastController : MonoBehaviour
{
    public TextMeshProUGUI wordText;
    //public GameObject[] buttons; 
    public TextMeshProUGUI deviceText;
    public TextMeshProUGUI resultText;
    //public TextMeshProUGUI testText;

    public TextMeshProUGUI idText;
    public TextMeshProUGUI deviceChooseText;
    public TextMeshProUGUI trailText;

    public TMP_Dropdown participantID;
    public TMP_Dropdown deviceName;
    public TMP_Dropdown participantTrial;
    public GameObject startButton;

    private String rightInput;
    private string userId = "1"; 
    private string currentDevice = "eyes";
    public string currentTrail = "Both";
    public string startTime;
    public string chooseTime;
    public Double interval;

    private float maxContrast = 1f;
    private float minContrast = 0f;
    private float currentContrast;
    private bool isTesting = false;
    private Dictionary<string, float> results = new Dictionary<string, float>();
    private char correctFirstLetter; 
    private char correctSecondLetter; 
    private bool isFirstLetterCorrect = false; 
    private string userInput = ""; 
    public float resultData;

    void IdValueChanged(TMP_Dropdown change)
    {
        Debug.Log("当前选中: " + change.options[change.value].text);
        userId = change.options[change.value].text;
    }

    void deviceNameChanged(TMP_Dropdown change)
    {
        Debug.Log("当前选中: " + change.options[change.value].text);
        currentDevice = change.options[change.value].text;
    }

    void participantTrialChanged(TMP_Dropdown change)
    {
        Debug.Log("当前选中: " + change.options[change.value].text);
        currentTrail = change.options[change.value].text;
    }

    void Start()
    {
        resultText.gameObject.SetActive(false);
        List<string> options = new List<string> { };
        for (int i = 1; i < 65; i++)
            options.Add(item: i.ToString());
        participantID.ClearOptions();
        participantID.AddOptions(options);
        participantID.onValueChanged.AddListener(delegate {
            IdValueChanged(participantID);
        });

        deviceName.ClearOptions();
        deviceName.AddOptions(new List<string> { "eyes", "A", "B", "C" });
        deviceName.onValueChanged.AddListener(delegate {
            deviceNameChanged(deviceName);
        });

        participantTrial.ClearOptions();
        participantTrial.AddOptions(new List<string> { "Both", "L", "R" });
        participantTrial.onValueChanged.AddListener(delegate {
            participantTrialChanged(participantTrial);
        });

        CreateDirectoryIfNotExist("Recordings");
    }

    private void CreateDirectoryIfNotExist(string folderName)
    {
        string folderPath = Path.Combine(Application.dataPath, folderName);
#if UNITY_ANDROID
        string folderPath = Path.Combine(Application.persistentDataPath, folderName);
#endif

        if (!Directory.Exists(folderPath))
        {

            Directory.CreateDirectory(folderPath);
            Debug.Log($"Folder created: {folderPath}");
        }
        else
        {
            Debug.Log($"Folder already exists: {folderPath}");
        }
    }

    private void EvaluateInput()
    {
        string correctLetters = correctFirstLetter.ToString() + correctSecondLetter.ToString();
        interval = CalculateTimeDifferenceInSeconds(chooseTime, GetTimeForFileName());
        chooseTime = GetTimeForFileName();
        float contrastSensetive=1.0f/currentContrast;
        resultData = contrastSensetive;
        bool isCorrect = userInput == correctLetters;
        if (isTesting)
        {
            string filePath = Application.dataPath + "/Recordings/VCS_ID_" + userId + "_Device_" + currentDevice + "_Trail_" + currentTrail + startTime + ".csv";
#if UNITY_ANDROID
            string filePath = Application.persistentDataPath + "/Recordings/VCS_ID_" + userId + "_Device_" + currentDevice + "_Trail_" + currentTrail + startTime + ".csv";
#endif
            string csvContent = $"{GetTimeForFileName()},{userId},{currentDevice},{currentTrail},{interval},{currentContrast},{minContrast},{maxContrast}," +
                                $"{correctLetters},{userInput},{isCorrect},{contrastSensetive}\n";

            if (!File.Exists(filePath))
            {
                string csvHeader = "Timestamp,ID,Device,trail,time interval,Virtual contrast,currentMinContrast,currentMaxContrast,Correct letter,Pressed letter,Is Correct,Virtual contrast sensitivity\n";
                File.WriteAllText(filePath, csvHeader);
            }

            File.AppendAllText(filePath, csvContent);
        }
        if (userInput == correctLetters)
        {
            maxContrast = currentContrast; 
        }
        else
        {
            minContrast = currentContrast; 
        }
        AdjustContrast();
    }

    public static string GetTimeForFileName()
    {
        return System.DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss_ffffff");
    }

    public static double CalculateTimeDifferenceInSeconds(string time1, string time2)
    {
        var format = "yyyy_MM_dd_hh_mm_ss_ffffff";
        var provider = CultureInfo.InvariantCulture;

        DateTime dateTime1 = DateTime.ParseExact(time1, format, provider);
        DateTime dateTime2 = DateTime.ParseExact(time2, format, provider);

        TimeSpan timeDifference = dateTime2 - dateTime1;

        return timeDifference.TotalSeconds;
    }

    public void StartTest()
    {
        startTime = GetTimeForFileName();
        chooseTime = GetTimeForFileName();
        deviceText.text = currentDevice;
        resultText.gameObject.SetActive(false);
        wordText.gameObject.SetActive(true);
        ResetTest();
        isTesting = true;
    }

    void Update()
    {
        //testText.text = currentContrast.ToString();
        if (isTesting)
        {
            ProcessInput();
        }
    }

    private void ResetTest()
    {
        idText.gameObject.SetActive(false);
        deviceChooseText.gameObject.SetActive(false);
        trailText.gameObject.SetActive(false);
        participantID.gameObject.SetActive(false);
        deviceName.gameObject.SetActive(false);
        participantTrial.gameObject.SetActive(false);
        startButton.SetActive(false);
        startTime =GetTimeForFileName();
        minContrast = 0f;
        maxContrast = 2f;
        currentContrast = (maxContrast+minContrast)/2;
        UpdateWordContrast(currentContrast);
        GenerateRandomLetters();
    }

    private void ProcessInput()
    {
        if (Input.anyKeyDown)
        {
            foreach (char c in Input.inputString)
            {
                char upperCaseChar = char.ToUpper(c);
                if (upperCaseChar >= 'A' && upperCaseChar <= 'Z')
                {
                    userInput += upperCaseChar;
                    if (userInput.Length == 2)
                    {
                        EvaluateInput();
                        userInput = ""; 
                    }
                }
            }
        }
    }


    private void GenerateRandomLetters()
    {
        correctFirstLetter = (char)('A' + Random.Range(0, 26));
        do
        {
            correctSecondLetter = (char)('A' + Random.Range(0, 26));
        } while (correctSecondLetter == correctFirstLetter);

        wordText.text = correctFirstLetter + " " + correctSecondLetter;
        isFirstLetterCorrect = false; 
    }

    private void UpdateWordContrast(float contrast)
    {
        Color baseColor = new Color(1f,1f, 1f);
        Color targetColor = Color.black;
        Color currentColor = Color.Lerp(baseColor, targetColor, contrast);
        wordText.color = currentColor;
    }


    private void AdjustContrast()
    {
        if (Mathf.Abs(maxContrast - minContrast) < 0.0001f)
        {
            EndTest();
        }
        else
        {
            currentContrast = (maxContrast + minContrast) / 2;
            UpdateWordContrast(currentContrast);
            GenerateRandomLetters();
        }
    }

    private void EndTest()
    {
        isTesting = false;
        wordText.gameObject.SetActive(false);
        resultText.gameObject.SetActive(true);
        resultText.text = $"The result for this test is: {resultData:F4}";
        idText.gameObject.SetActive(true);
        deviceChooseText.gameObject.SetActive(true);
        trailText.gameObject.SetActive(true);
        participantID.gameObject.SetActive(true);
        deviceName.gameObject.SetActive(true);
        participantTrial.gameObject.SetActive(true);
        startButton.SetActive(true);
    }
}

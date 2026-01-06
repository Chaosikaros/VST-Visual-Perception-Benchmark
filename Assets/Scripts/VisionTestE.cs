using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine.UI;
using System.Globalization;
using Unity.VisualScripting;

public class VisionTestE : MonoBehaviour
{
    //用于显示的UI
    public TextMeshProUGUI deviceText;//显示正在测试的设备
    public TextMeshProUGUI resultText;//测试结果
    public TextMeshProUGUI VATest;//当前视力
    public TextMeshProUGUI sizeText; // 显示size值的TextMeshProUGUI组件
    public TextMeshProUGUI VerticalHint;
    public TextMeshProUGUI HorizontalhHint;
    public TextMeshProUGUI idText;
    public TextMeshProUGUI deviceChooseText;
    public TextMeshProUGUI trailText;

    public TMP_Dropdown participantID;
    public TMP_Dropdown deviceName;
    public TMP_Dropdown participantTrial;
    public GameObject startButton;

    public GameObject saveButton;
    public GameObject resetButton;
    public GameObject calibrationButton;

    //public GameObject[] buttons; // 按钮数组
    public GameObject visionChart;
    public GameObject target; // 目标对象，用于计算屏幕空间大小
    public GameObject objectToMove; // 引用到需要上下移动的物体
    public GameObject targetObject; // 需要调整Z值的目标物体

    
    public Slider verticalSlider; // 引用到场景中的Slider
    public Slider zValueSlider; // 拖动滑块以调整size值的Slider
    public Slider horizontalSlider; // 引用场景中的Slider

    private String rightDirection;
    private String inputDirection;
    private string userId="1"; // 用户ID
    private string currentDevice="eyes";
    public string currentTrail="Both";
    public string startTime;
    public string chooseTime;
    public Double interval;
    

    private float maxDistance = 3f;
    private float minDistance = 0.01f;
    private float currentDistance;
    private float scale;
    private int currentDirection;
    private bool isTesting = false;
    private Dictionary<string, float> results = new Dictionary<string, float>();
    private int correctCount = 0;

    //计算过程中需要初始化或赋予的值
    public float calibration = 0.0f;
    public float eyeDistance = 0.8f;
    public float gapSize = 0.0f;
    public float calibratEPixel;
    public float calibratESize = 0.024f;

    //计算过程中被计算的值
    public float pixerPerMeter;
    public float VA = 1.0f;
    public float newEPixel;
    public float newESize;
    public float newEGapSize;
    public float angle;

    private bool isVisible = false; 

    public void calibrationControl()
    {
        isVisible = !isVisible;
        saveButton.SetActive(isVisible);
        resetButton.SetActive(isVisible);
        verticalSlider.gameObject.SetActive(isVisible);
        VerticalHint.gameObject.SetActive(isVisible);
        horizontalSlider.gameObject.SetActive(isVisible);
        HorizontalhHint.gameObject.SetActive(isVisible);
        zValueSlider.gameObject.SetActive(isVisible);
        sizeText.gameObject.SetActive(isVisible);
    }

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


    public void ResetTransformData()
    {
        // 设置默认值
        PlayerPrefs.SetFloat("positionX", 0);
        PlayerPrefs.SetFloat("positionY", 0);
        PlayerPrefs.SetFloat("scaleX", 5);

        // 保存更改
        PlayerPrefs.Save();
        LoadTransformData();
    }

    public void SaveTransformData()
    {
        if (target != null)
        {
            PlayerPrefs.SetFloat("positionX", target.transform.position.x);
            PlayerPrefs.SetFloat("positionY", target.transform.position.y);
            PlayerPrefs.SetFloat("scaleX", target.transform.localScale.x);
            PlayerPrefs.Save(); // 保存更改
        }
    }

    public void LoadTransformData()
    {
        if (target != null)
        {
            // 检查PlayerPrefs是否包含特定的键
            float positionX = PlayerPrefs.HasKey("positionX") ? PlayerPrefs.GetFloat("positionX") : target.transform.position.x;
            float positionY = PlayerPrefs.HasKey("positionY") ? PlayerPrefs.GetFloat("positionY") : target.transform.position.y;
            float scaleX = PlayerPrefs.HasKey("scaleX") ? PlayerPrefs.GetFloat("scaleX") : target.transform.localScale.x;

            // 设置物体的位置和scaleX
            Vector3 position = new Vector3(positionX, positionY, target.transform.position.z);
            Vector3 scale = new Vector3(scaleX, scaleX, scaleX);

            target.transform.position = position;
            target.transform.localScale = scale;
        }
    }

    private void CreateDirectoryIfNotExist(string folderName)
    {
        string folderPath = Path.Combine(Application.dataPath, folderName);
#if UNITY_ANDROID
        // 构造文件夹完整路径
        string folderPath = Path.Combine(Application.persistentDataPath, folderName);
#endif 
        // 检查文件夹是否存在
        if (!Directory.Exists(folderPath))
        {
            // 文件夹不存在，创建它
            Directory.CreateDirectory(folderPath);
            Debug.Log($"Folder created: {folderPath}");
        }
        else
        {
            Debug.Log($"Folder already exists: {folderPath}");
        }
    }

    public float getVisualAcuity(float input)
    {
        float va = 0;
        pixerPerMeter = calibratESize/calibratEPixel;
        newEPixel = input;
        newESize = newEPixel * pixerPerMeter;
        newEGapSize = 0.2f * newESize;
        angle = 0;
        VisualAngleConversion(ref angle, newEGapSize, eyeDistance);
        va= 1.0f / DegreeToVisualAngleFloat(angle);
        return va;
    }

    void Start()
    {
        if (zValueSlider != null)
        {
            zValueSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }
        verticalSlider.onValueChanged.AddListener(HandleVerticalSliderValueChanged);
        horizontalSlider.onValueChanged.AddListener(HandleHorizontalSliderValueChanged);
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

        // 检查并创建VisionAll文件夹
        //CreateDirectoryIfNotExist("VisionAll");

        // 检查并创建VisionRect文件夹
        CreateDirectoryIfNotExist("Recordings");

        VATest.gameObject.SetActive(false);
        LoadTransformData();
        //target.transform.localScale = calibration * Vector3.one;
        //SaveTransformData();
        calibratEPixel = getFinalRect();
        VisualAngleDebug();
        //visionChart.SetActive(false);
        resultText.gameObject.SetActive(false);
        calibrationButton.SetActive(true);


    }

    void HandleHorizontalSliderValueChanged(float value)
    {
        // 设置物体的X值为Slider的当前值
        Vector3 newPosition = objectToMove.transform.position;
        newPosition.x = value;
        objectToMove.transform.position = newPosition;
    }
    void HandleVerticalSliderValueChanged(float value)
    {
        // 设置物体的Y值为Slider的当前值
        Vector3 newPosition = objectToMove.transform.position;
        newPosition.y = value;
        objectToMove.transform.position = newPosition;
    }

    public void OnSliderValueChanged(float value)
    {
        // 改变目标物体的Z值
        if (targetObject != null)
        {
            targetObject.transform.localScale = value * Vector3.one;
            calibration = value;
            calibratEPixel = getFinalRect();
            // 输出新的Z值到TextMeshProUGUI组件中
            if (sizeText != null)
            {
                sizeText.text = "E size: " + value.ToString("F2"); // "F2"格式化为显示两位小数
            }
        }
    }

    public void StartTest()
    {
        startTime = GetTimeForFileName();
        chooseTime = GetTimeForFileName();
        //currentDevice = deviceName;
        deviceText.text = currentDevice;
        //ToggleButtons(false);
        resultText.gameObject.SetActive(false);
        visionChart.SetActive(true);
        ResetTest();
        isTesting = true;
    }

    void Update()
    {
        if (isTesting && Input.anyKeyDown)
        {
            ProcessInput();
        }
    }

    private void ResetTest()
    {
        saveButton.SetActive(false);
        resetButton.SetActive(false);
        //VATest.gameObject.SetActive(true);
        verticalSlider.gameObject.SetActive(false);
        VerticalHint.gameObject.SetActive(false);
        horizontalSlider.gameObject.SetActive(false);
        HorizontalhHint.gameObject.SetActive(false);
        zValueSlider.gameObject.SetActive(false);
        sizeText.gameObject.SetActive(false);
        idText.gameObject.SetActive(false);
        deviceChooseText.gameObject.SetActive(false);
        trailText.gameObject.SetActive(false);
        participantID.gameObject.SetActive(false);
        deviceName.gameObject.SetActive(false);
        participantTrial.gameObject.SetActive(false);
        startButton.SetActive(false);
        calibrationButton.SetActive(false);
        minDistance = 0;
        maxDistance = target.transform.localScale.x*2;
        currentDistance = (minDistance+maxDistance)/2;
        correctCount = 0;
        LoadTransformData();
        //visionChart.transform.localScale = currentDistance * Vector3.one;
        //SaveTransformData();
        SetRandomDirection();
    }

    private void ProcessInput()
    {
        //Debug.Log("current VA is: " + getVisualAcuity(getFinalRect()).ToString());
        VATest.text = "current VA is: " + getVisualAcuity(getFinalRect()).ToString();
        KeyCode[] keyCodes = new KeyCode[] { KeyCode.RightArrow,KeyCode.DownArrow,KeyCode.LeftArrow,KeyCode.UpArrow };
        for (int i = 0; i <= 3; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                getRightDirection();
                bool isCorrect = (i+1 == currentDirection);
                if (isCorrect)
                {
                    correctCount++;
                    if (correctCount >= 8) // 如果连续2次正确
                    {
                        maxDistance = currentDistance; // 增加最小测试距离
                        correctCount = 0; // 重置连续正确次数
                        AdjustChartPosition();
                    }
                    else
                    {
                        // 如果还没有达到连续2次，则在相同距离进行新的测试
                        SetRandomDirection();
                    }
                }
                else
                {
                    correctCount = 0;
                    minDistance = currentDistance; // 减小最大测试距离
                    AdjustChartPosition();
                }

                RecordKeyPressAndRectData(i, isCorrect);
                break;
            }
        }
    }

    private void RecordKeyPressAndRectData(int keyPressed, bool isCorrect)
    {
        // 获取屏幕空间数据
        GetRect(out float width, out float height);

        // 保存按键数据和屏幕空间数据到CSV文件
        SaveDataToCSV(keyPressed, isCorrect, width, height);
    }

    public void GetRect(out float width, out float height)
    {
        Camera camera = Camera.main;
        Renderer renderer = target.GetComponent<Renderer>();
        Bounds bounds = renderer.bounds;

        Vector3 minPoint = camera.WorldToScreenPoint(bounds.min);
        Vector3 maxPoint = camera.WorldToScreenPoint(bounds.max);

        Rect rect = new Rect(minPoint.x, minPoint.y, maxPoint.x - minPoint.x, maxPoint.y - minPoint.y);
        //Debug.Log("Screen Space Size: " + rect.width + "x" + rect.height);

        width = rect.width;
        height = rect.height;
    }
    public void getRightDirection()
    {
        switch (currentDirection)
        {
            case 1:
                rightDirection = "right";
                break;
            case 2:
                rightDirection = "down";
                break;
            case 3:
                rightDirection = "left";
                break;
            case 4:
                rightDirection = "up";
                break;
        }
    }
    public float getFinalRect()
    {
        Camera camera = Camera.main;
        Renderer renderer = target.GetComponent<Renderer>();
        Bounds bounds = renderer.bounds;

        Vector3 minPoint = camera.WorldToScreenPoint(bounds.min);
        Vector3 maxPoint = camera.WorldToScreenPoint(bounds.max);

        Rect rect = new Rect(minPoint.x, minPoint.y, maxPoint.x - minPoint.x, maxPoint.y - minPoint.y);
        
        return rect.width;
        
    }

    private void SaveDataToCSV(int keyPressed, bool isCorrect, float width, float height)
    {
        interval = CalculateTimeDifferenceInSeconds(chooseTime, GetTimeForFileName());
        chooseTime = GetTimeForFileName();
        switch (keyPressed)
        {
            case 0:
                inputDirection = "right";
                break;
            case 1:
                inputDirection = "down";
                break;
            case 2:
                inputDirection = "left";
                break;
            case 3:
                inputDirection = "up";
                break;
        }
        if (isTesting)
        {
            VATest.text = "current VA is: " + getVisualAcuity(getFinalRect()).ToString();
            string filePath = Application.dataPath + "/Recordings/VA_ID_" + userId + "_Device_" + currentDevice + "_Trail_" + currentTrail + startTime + ".csv";
#if UNITY_ANDROID
            string filePath = Application.persistentDataPath + "/Recordings/VA_ID_" + userId + "_Device_" + currentDevice + "_Trail_" + currentTrail + startTime + ".csv";
#endif
            string csvContent = $"{GetTimeForFileName()},{userId},{currentDevice},{currentTrail},{interval},{target.transform.localScale.x},{minDistance},{maxDistance},{width},{height}," +
                                $"{rightDirection},{inputDirection},{isCorrect},{getVisualAcuity(getFinalRect()).ToString()}\n";

            if (!File.Exists(filePath))
            {
                string csvHeader = "Timestamp,ID,Device,trail,time interval,scale,currentMinScale,currentMaxScale,Width,Height,Correct Button,Pressed Button,Is Correct,VA\n";
                File.WriteAllText(filePath, csvHeader);
            }

            File.AppendAllText(filePath, csvContent);
        }
    }
    public static string GetTimeForFileName()
    {
        return System.DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss_ffffff");
    }

    public static double CalculateTimeDifferenceInSeconds(string time1, string time2)
    {
        var format = "yyyy_MM_dd_hh_mm_ss_ffffff";
        var provider = CultureInfo.InvariantCulture;

        // 将字符串转换为DateTime对象
        DateTime dateTime1 = DateTime.ParseExact(time1, format, provider);
        DateTime dateTime2 = DateTime.ParseExact(time2, format, provider);

        // 计算时间差
        TimeSpan timeDifference = dateTime2 - dateTime1;

        // 返回以秒为单位的时间差
        return timeDifference.TotalSeconds;
    }

    private void AdjustChartPosition()
    {
        currentDistance = (maxDistance + minDistance) / 2;
        visionChart.transform.localScale = currentDistance * Vector3.one;

        if (Mathf.Abs(maxDistance - minDistance) < 0.001f)
        {
            EndTest();
            return;
        }

        if (correctCount == 0)
        {
            SetRandomDirection();
        }
    }

    private void SetRandomDirection()
    {
        currentDirection = UnityEngine.Random.Range(1, 5);
        visionChart.transform.localEulerAngles = new Vector3(0, 0, (currentDirection - 1) * -90f);
    }

    private void EndTest()
    {
        isTesting = false;
        visionChart.transform.localScale = currentDistance * Vector3.one;
        VA = getVisualAcuity(getFinalRect());
        //visionChart.SetActive(false);
        LoadTransformData();
        //ToggleButtons(true);
        resultText.gameObject.SetActive(true);
        resultText.text = $"The result for this test is: {currentDistance:F2}";
        results[currentDevice] = currentDistance;

        //SaveVisionData();
        //saveButton.SetActive(true);
        //resetButton.SetActive(true);
        //verticalSlider.gameObject.SetActive(true);
        //VerticalHint.gameObject.SetActive(true);
        //horizontalSlider.gameObject.SetActive(true);
        //HorizontalhHint.gameObject.SetActive(true);
        //zValueSlider.gameObject.SetActive(true);
        //sizeText.gameObject.SetActive(true);
        idText.gameObject.SetActive(true);
        deviceChooseText.gameObject.SetActive(true);
        trailText.gameObject.SetActive(true);
        participantID.gameObject.SetActive(true);
        deviceName.gameObject.SetActive(true);
        participantTrial.gameObject.SetActive (true);
        startButton.SetActive(true);
        calibrationButton.SetActive(true);
        isVisible = false;
        //CheckAllTestsDone();
    }

    

    private void SaveVisionData()
    {
        string visionDataPath = Application.dataPath + "/VisionAll/VisionData.csv";
#if UNITY_ANDROID
        string visionDataPath = Application.persistentDataPath + "/VisionAll/VisionData.csv";
#endif
        string visionDataContent = $"{userId},{currentDevice},{currentDistance:F2},{VA}\n";

        if (!File.Exists(visionDataPath))
        {
            string visionHeader = "UserID,Device,Distance,VA\n";
            File.WriteAllText(visionDataPath, visionHeader);
        }

        File.AppendAllText(visionDataPath, visionDataContent);
    }
    
    public float VisualAngleConversion(ref float visualAngle, float objectSize, float objectDistance)
    {
        if (visualAngle == 0 && objectSize != 0 && objectDistance != 0)
        {
            visualAngle = (float)(2 * Mathf.Atan(objectSize / (2 * objectDistance)) * (180 / Math.PI));
            return visualAngle;
        }
        else if (visualAngle != 0 && objectSize == 0 && objectDistance != 0)
        {
            return (float)(2 * objectDistance * Mathf.Tan((float)(visualAngle * Math.PI / 360)));
        }
        else if (visualAngle != 0 && objectSize != 0 && objectDistance == 0)
        {
            return (float)((objectSize / 2) / Mathf.Tan((float)(visualAngle * Math.PI / 360)));
        }
        else
        {
            return 0;
        }
    }

    public string DegreeToVisualAngle(float visualAngle)
    {
        float d = Mathf.Floor(visualAngle);
        float m = Mathf.Floor(60.0f * (visualAngle - d));
        float s = (visualAngle - d - m / 60.0f) * 60.0f;
        return d + "°" + m + "'" + s.ToString("F2") + "''";
    }

    public float DegreeToVisualAngleFloat(float visualAngle)
    {
        float d = Mathf.Floor(visualAngle);
        float m = Mathf.Floor(60.0f * (visualAngle - d));
        float s = (visualAngle - d - m / 60.0f) * 60.0f;
        return (float)d * 60.0f + (float)m + (float)s * 60.0f / 60.0f;
    }
    public void VisualAngleDebug()
    {
        float angle = (float)1 / 60;
        //Debug.Log(VisualAngleConversion(ref angle, 0, 3) + "; VisualAngle:" + DegreeToVisualAngle(angle));
        angle = (float)1 / 60;
        Debug.Log(VisualAngleConversion(ref angle, 0, 3) + "; VisualAngle:" + DegreeToVisualAngle(angle)
            + "; VisualAngle float:" + DegreeToVisualAngleFloat(angle)
            + "; Visual Acuity:" + 1.0f / DegreeToVisualAngleFloat(angle));
        for (int i = 0; i < 300; i++)
        {
            angle = 0;
            float distance = i * 0.01f;
            float gapRatioForE = 0.2f;
            float eSize = 0.024f;
            Debug.Log("Distance: " + distance + "; " + VisualAngleConversion(ref angle, gapRatioForE * eSize, distance) + "; VisualAngle:" + DegreeToVisualAngle(angle)
                + "; VisualAngle float:" + DegreeToVisualAngleFloat(angle)
                + "; Visual Acuity:" + 1.0f / DegreeToVisualAngleFloat(angle));
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using SimpleJSON;
using System;
using Random = System.Random;
using System.Text;
using TMPro;

public class GridGenerator : MonoBehaviour
{
    public GameObject rawImagePrefab; // Assign in inspector
    public int rows = 4;
    public int columns = 22;
    public float imageSize = 40f;
    public float spacing = 2f;
    public float rowSpacing = 10f;
    public List<Color> colors = new List<Color>(); // List to hold the colors
    public List<Color> colors1 = new List<Color>();
    public List<Color> colors2 = new List<Color>();
    public List<Color> colors3 = new List<Color>();
    public List<Color> colors4 = new List<Color>();

    public List<Vector2>position1=new List<Vector2>();
    public List<Vector2>position2=new List<Vector2>();
    public List<Vector2>position3=new List<Vector2>();
    public List<Vector2>position4=new List<Vector2>();

    public List<GameObject>team1=new List<GameObject>();
    public List<GameObject>team2=new List<GameObject>();
    public List<GameObject>team3=new List<GameObject>();
    public List<GameObject>team4=new List<GameObject>();

    public List<int>wrongIndex1=new List<int>();
    public List<int>wrongIndex2=new List<int>();
    public List<int>wrongIndex3=new List<int>();
    public List<int>wrongIndex4=new List<int>();

    public TextMeshProUGUI idText;
    public TextMeshProUGUI deviceChooseText;
    public TextMeshProUGUI trailText;
    public GameObject startButton;
    public TMP_Dropdown participantID;
    public TMP_Dropdown deviceName;
    public TMP_Dropdown participantTrial;

    private string userId = "1"; // 用户ID
    private string currentDevice = "eyes";
    public string currentTrail = "Both";

    public string fileTime;


    private DateTime startTime;
    private DateTime endTime;
    private TimeSpan duration;

    private Random rand = new Random(); // 创建Random实例作为成员变量

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
    
    public List<T> ShuffleMiddle<T>(List<T> list)
    {
        // 创建一个新的列表复制原始列表
        List<T> newList = new List<T>(list);

        if (newList.Count <= 2) return newList; // 如果列表元素不足以打乱，则直接返回新列表

        // 从第二个元素到倒数第二个元素进行遍历
        for (int i = 1; i < newList.Count - 1; i++)
        {
            int j = rand.Next(i, newList.Count - 1); // 生成一个随机索引，范围从i到n-2
            // 交换当前元素和随机索引处的元素
            T temp = newList[i];
            newList[i] = newList[j];
            newList[j] = temp;
        }

        return newList; // 返回已打乱中间元素的新列表
    }

    public static string GetTimeForFileName()
    {
        return System.DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss_ffffff");
    }

    public void ShuffleRandomly()
    {
        idText.gameObject.SetActive(false);
        deviceChooseText.gameObject.SetActive(false);
        trailText.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
        participantID.gameObject.SetActive(false);
        deviceName.gameObject.SetActive(false);
        participantTrial.gameObject.SetActive(false);

        startTime = DateTime.Now;
        //Debug.Log("Test started at: " + startTime.ToString("yyyy-MM-dd HH:mm:ss"));
        fileTime = GetTimeForFileName();
        team1=ShuffleMiddle(team1);
        team2=ShuffleMiddle(team2);
        team3=ShuffleMiddle(team3);
        team4=ShuffleMiddle(team4);
        for(int i=0; i<team1.Count; i++)
        {
            team1[i].GetComponent<RectTransform>().anchoredPosition = position1[i];
        }
        for (int i = 0; i < team1.Count; i++)
        {
            team2[i].GetComponent<RectTransform>().anchoredPosition = position2[i];
        }
        for (int i = 0; i < team1.Count; i++)
        {
            team3[i].GetComponent<RectTransform>().anchoredPosition = position3[i];
        }
        for (int i = 0; i < team1.Count; i++)
        {
            team4[i].GetComponent<RectTransform>().anchoredPosition = position4[i];
        }
    }

    void Start()
    {
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
        LoadColorsFromJson();
        GenerateGrid();
    }

    void LoadColorsFromJson()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "colors.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            var N = sJSON.Parse(json);
            for (int i = 1; i <= 88; i++)
            {
                string colorStr = N[i.ToString()].Value;
                Color color;
                if (ColorUtility.TryParseHtmlString(colorStr, out color))
                {
                    colors.Add(color);
                    if (i <= 22)
                    {
                        colors1.Add(color);
                    }
                    
                    if(i>22 && i <= 44)
                    {
                        colors2.Add(color);
                    }
                    if (i > 44 && i <=66 )
                    {
                        colors3.Add(color);
                    }
                    if(i>66)
                    {
                        colors4.Add(color);
                    }
                }
            }
        }
    }

    void GenerateGrid()
    {
        int colorIndex = 0; // Start with the first color
        int index = 1;
        // Assuming the parent GameObject of this script is the Canvas or a child of the Canvas
        RectTransform canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        // Calculate total grid width and height
        float totalWidth = (columns - 1) * (imageSize + spacing) + imageSize;
        float totalHeight = (rows - 1) * (imageSize + rowSpacing) + imageSize;

        // Calculate the start position to center the grid
        Vector2 startPosition = new Vector2(-totalWidth / 2, totalHeight / 2);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector2 position = new Vector2(startPosition.x + j * (imageSize + spacing), startPosition.y - i * (imageSize + rowSpacing));
                if (i == 0)
                {
                    position1.Add(position);
                }  
                if (i == 1)
                {
                    position2.Add(position);
                }
                if (i == 2)
                {
                    position3.Add(position);
                }
                if (i == 3)
                {
                    position4.Add(position);
                }
                GameObject rawImageObj = Instantiate(rawImagePrefab, transform);
                if (i == 0)
                {
                    team1.Add(rawImageObj);
                }
                if (i == 1)
                {
                    team2.Add(rawImageObj);
                }
                if (i == 2)
                {
                    team3.Add(rawImageObj);
                }
                if (i == 3)
                {
                    team4.Add(rawImageObj);
                }

                rawImageObj.GetComponent<RectTransform>().anchoredPosition = position;
                rawImageObj.GetComponent<RectTransform>().sizeDelta = new Vector2(imageSize, imageSize);
                rawImageObj.GetComponent<RectTransform>().localScale = Vector3.one; // Ensure the scale is set to 1
                rawImageObj.GetComponent<Attribute>().index = index;
                if (index != 1 && index != 22 && index != 23 && index != 44 && index != 45 && index != 66 && index != 67 && index != 88)
                {
                    rawImageObj.AddComponent<DraggableImage>();
                }
                index++;
                var image = rawImageObj.GetComponent<RawImage>();
                image.color = colors[colorIndex % colors.Count];
                colorIndex++;
                //if (j == columns - 1 && i < rows - 1) // If it's the last in the row and not the last row
                //{
                //    colorIndex--; // Repeat the last color for the next row's first
                //}
            }
        }

        // Center the grid generator object itself in the canvas
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.zero; // Center in parent Canvas
        rectTransform.sizeDelta = new Vector2(totalWidth, totalHeight); // Adjust size to grid
    }
    public void getResult()
    {
        endTime = DateTime.Now;
        Debug.Log("Test ended at: " + endTime.ToString("yyyy-MM-dd HH:mm:ss"));

        duration = endTime - startTime;
        Debug.Log("Test duration: " + duration.Seconds + " seconds");
        int result = CalculateTotalErrorScore(team1, team2, team3, team4);
        GenerateCSV(team1, team2, team3, team4, result);
        Debug.Log(result);
    }
    private int ComputeErrorScore(List<GameObject> team)
    {
        if (team.Count < 3) return 0; // 如果队伍中的游戏对象少于3个，则误差得分为0

        int errorScore = 0;
        for (int i = 0; i < team.Count; i++)
        {
            int current = team[i].GetComponent<Attribute>().index;
            int next = i < team.Count - 1 ? team[i + 1].GetComponent<Attribute>().index : team[0].GetComponent<Attribute>().index;
            int prev = i > 0 ? team[i - 1].GetComponent<Attribute>().index : team[team.Count - 1].GetComponent<Attribute>().index;

            // 由于是连续的数字，所以正确顺序的差值应该总是1，除了首尾特殊处理
            if (i == 0 || i == team.Count - 1)
            {
                // 首尾元素不计算误差得分
                continue;
            }
            else
            {
                errorScore += Mathf.Abs(current - prev) + Mathf.Abs(current - next) - 2;
            }
        }

        return errorScore;
        
    }

    // 计算所有队伍的总误差得分
    public int CalculateTotalErrorScore(List<GameObject> team1, List<GameObject> team2, List<GameObject> team3, List<GameObject> team4)
    {
        int totalErrorScore = 0;
        totalErrorScore += ComputeErrorScore(team1);
        totalErrorScore += ComputeErrorScore(team2);
        totalErrorScore += ComputeErrorScore(team3);
        totalErrorScore += ComputeErrorScore(team4);
        return totalErrorScore;
    }
    
    public void GenerateCSV(List<GameObject> team1, List<GameObject> team2, List<GameObject> team3, List<GameObject> team4, int totalScore)
    {
        string folderPath = Path.Combine(Application.dataPath, "Recordings");
#if UNITY_ANDROID
        string folderPath = Path.Combine(Application.persistentDataPath, "Recordings");
#endif

        // 检查目录是否存在，不存在则创建
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh(); // 刷新Unity编辑器的资产数据库
#endif
        }

        // 文件名中时间的格式化
        //string fileTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filePath = Path.Combine(folderPath, $"VC_ID_{userId}_Device_{currentDevice}_Trail_{currentTrail}{fileTime}.csv");

        StringBuilder csvContent = new StringBuilder();

        // 添加表头
        csvContent.AppendLine("id,currentDevice,currentTrail,row1,row2,row3,row4,errorScore,test_time");

        // 添加内容行
        string row1 = ConvertTeamToCSVRow(team1);
        string row2 = ConvertTeamToCSVRow(team2);
        string row3 = ConvertTeamToCSVRow(team3);
        string row4 = ConvertTeamToCSVRow(team4);
        string testTime = duration.TotalSeconds.ToString(); // 将时间间隔格式化为总秒数

        // 使用逗号分隔每个字段，对于队伍序号使用分号分隔
        csvContent.AppendLine($"{userId},{currentDevice},{currentTrail},{row1},{row2},{row3},{row4},{totalScore},{testTime}");

        // 写入文件
        File.WriteAllText(filePath, csvContent.ToString());
        
    }

    private string ConvertTeamToCSVRow(List<GameObject> team)
    {
        List<string> indices = new List<string>();
        foreach (var gameObject in team)
        {
            indices.Add(gameObject.GetComponent<Attribute>().index.ToString());
        }
        return string.Join(";", indices); // 使用分号分隔队伍中的序号
    }
}



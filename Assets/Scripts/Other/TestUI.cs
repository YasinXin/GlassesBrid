using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TestUI : Manager {

    public Camera uiCamera;
    public Transform uiPanel;
    public Text text1;
    public Text text2;
    public Text text3;
    public RawImage image;
    public GameObject ball;
    public static GameObject[] allBall = new GameObject[68];

    public Transform grid;

    public static Text myText1;
    public static Text myText2;
    public static Text myText3;
    public static RawImage testImage;
    public static bool option = false;
    public static float sizeRate = 1;

    private RenderCamera rendCamera;
    private BuffingShader bufferShader;
    public Text filterName;
    public Text filterLevel;
    public Text colorTemperature;
    public Text bufferLevel;
    public Text lightness;
    public Text glassSize;

    public Slider filterLevel_slider;
    public Slider colorTemperature_slider;
    public Slider bufferLevel_slider;
    public Slider lightness_slider;
    public Slider glassSize_slider;

    string[] filterType =
          {
        "Origin",
        "Lolita",
        "Coral",
        "Rosy",
        "Crisp",
        "Nature",
        "Clean",
        "Vivid",
        "Sweety",
        "Sunset",
        "Grass",
        "Pink",
        "Brannan",
        "Inkwell"
    };

    public Texture[] filterTexture;
    public Shader[] filterShader;

    public struct FrameData
    {
        //滤镜Shader
        public string _filterShader;
        public int _filterTexture;
        public float _filterLevel;
        public float _colorTemperature;
        public float _bufferLevel;
        public float _lightness;
        public float _glassSize;
    }

    private FrameData frameData;

    void Awake()
    {
        frameData._filterShader = "Clean";
        frameData._filterTexture = 6;
        frameData._filterLevel = 0.4f;
        frameData._colorTemperature = 15;
        frameData._bufferLevel = 0f;
        frameData._lightness = 1;
        frameData._glassSize = 1;
    }

    void OnEnable()
    {
        TouchEvent.SingleClick += OnSingleClick;
        TouchEvent.SingleLongPress += OnSingleLongPress;
        TouchEvent.MouseRightCilck += OnMouseRightClick;
        TouchEvent.MouseLeftCilck += OnMouseLeftClick;
    }

    void OnDisable()
    {
        TouchEvent.SingleClick -= OnSingleClick;
        TouchEvent.SingleLongPress -= OnSingleLongPress;
        TouchEvent.MouseRightCilck -= OnMouseRightClick;
        TouchEvent.MouseLeftCilck -= OnMouseLeftClick;
    }

    public void StartTrack()
    {
        WebCamMgr.m_webCamera.Bol_UpdateTexture = true;
    }
    // Use this for initialization
    void Start () {

        //Debug.Log("sssssssssssssssssssssssss");
        //string str = "/storage/emulated/0/Android/data/com.gqiqi.deviceapp/cache/lidcv/Model/material/btn_2d_b109";
        //Debug.Log(str.IndexOf("2d_") + "!!!!!!!!!!!!!!!!!!!!");
        myText1 = text1;
        myText2 = text2;
        myText3 = text3;
        testImage = image;

        for (int i = 0; i < grid.childCount; i++)
        {
            grid.GetChild(i).GetChild(0).GetComponent<Text>().text = filterType[i];
            grid.GetChild(i).GetChild(0).GetComponent<Text>().name = i.ToString();
        }

        bufferShader = uiCamera.GetComponent<BuffingShader>();
        rendCamera = uiCamera.GetComponent<RenderCamera>();

        //PlayerPrefs.SetInt("IsFrist", 0);
        if (PlayerPrefs.GetInt("IsFrist") == 0)
        {
            SetFrameData();
            PlayerPrefs.SetInt("IsFrist", 1);
        }

        for (int i = 0; i < 68; i++)
        {
            GameObject go = Instantiate(ball);
            go.transform.GetChild(0).GetComponent<TextMesh>().text = (i+1).ToString();
            allBall[i] = go;
        }

        GetFrameData();
        InitFrame();
    }

    // Update is called once per frame
    private float rateTime;
	void Update () {
        if (Time.time - rateTime > 0)
        {
            myText1.text = (1 / Time.deltaTime).ToString();
            rateTime = Time.time + 0.1f;
        }
    }
    
    //调用事件
    public void GetEvent()
    {
        string str;
        str = "FunctionName=GetEvent|Value=ScreenState";  // ScreenState: 获取是否分屏
        str = "FunctionName=GetEvent|Value=ModelSide";    // ModelSide: 获取模型挂载在哪边
        GameMgr.message(str);
    } 

    //清理缓存
    public void CleanCatch(Text text)
    {     
        option = !option;
        text.text = option.ToString();
    }

    #region 事件
    //单指触碰
    void OnSingleClick()
    {
        Debug.Log("click");
    }

    //单指长按
    void OnSingleLongPress()
    {
        //Test.myText2.text = "longPress";
        Debug.Log("longpress");
    }

    //鼠标左击
    void OnMouseLeftClick()
    {

        Ray ray = uiCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            Debug.Log("onUI");
        }
        else
        {
            Debug.Log("onWorld");
        }
        Debug.Log("left~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
    }

    //鼠标右击
    void OnMouseRightClick()
    {
        //SetUIPanelActive(true);
        Debug.Log("right~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
    }
    #endregion

    //相框
    public void OpenFrame()
    {
        string path = "";
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        path = "file://" + Application.streamingAssetsPath + "/Texture/frame.png";
#else
        path = Application.streamingAssetsPath + "/Texture/frame.png";
#endif
        WebCamMgr.SetFrameMask(path);
    }

    //控制uiPanel的显影
    public void ShowUIPanelClick(Toggle toggle)
    {
        string str;
        if (toggle.isOn)
        {
            str = "FunctionName=ShowFrameControl|Bool=true";  //显示
        }
        else
        {
            str = "FunctionName=ShowFrameControl|Bool=false";  //隐藏
        }

        GameMgr.message(str);
    }

    public void SetUIPanelActive(bool value)
    {
        uiPanel.gameObject.SetActive(value);
    }

    //更换滤镜
    public void ChangeFilter(Text text)
    {
        //FilteMgr.CameraFilter(text.text);
        FilterControl(int.Parse(text.name), text.text);
    }

    void FilterControl(int index, string str)
    {
        rendCamera.SCTexture = filterTexture[index];
        rendCamera.enabled = false;
        if (str.Equals("Brannan"))
        {
            rendCamera.curShader = filterShader[1];
        }
        else if (str.Equals("Inkwell"))
        {
            rendCamera.curShader = filterShader[2];
        }
        else
        {
            rendCamera.curShader = filterShader[0];
        }
        rendCamera.enabled = true;
        filterName.text = str;

        frameData._filterShader = str;
        frameData._filterTexture = index;
    }

    //调整滤镜强度
    public void FilterLevel(Slider slider)
    {
        FilterLevelControl(slider.value);
    }
     
    void FilterLevelControl(float value)
    {
        if (value > 0)
        {
            rendCamera.enabled = true;
        }
        else
        {
            rendCamera.enabled = false;
        }


        rendCamera.level = value;
        filterLevel.text = value.ToString();

        frameData._filterLevel = value;
    }

    //调整色温
    public void FilterColorTemperature(Slider slider)
    {
        FilterColorTemperatureControl(slider.value);
    }

    void FilterColorTemperatureControl(float value)
    {
        rendCamera.blueColorLevel = value;
        colorTemperature.text = value.ToString();

        frameData._colorTemperature = value;
    }

    //调整磨皮强度
    public void BufferLevel(Slider slider)
    { 
        BufferLevelControl(slider.value);
    }

    void BufferLevelControl(float value)
    {
        if(value > 0)
        {
            bufferShader.enabled = true;
        }
        else
        {
            bufferShader.enabled = false;
        }

        bufferShader.beauty = value;
        bufferLevel.text = value.ToString();

        frameData._bufferLevel = value;
    }

    //改变亮度
    public void ChangeLightness(Slider slider)
    {
        //ChangeLightnessControl(slider.value);
    }

    void ChangeLightnessControl(float value)
    {
        //GameMgr.message("FunctionName=SetBrightness|Value=" + value.ToString());
        lightness.text = value.ToString();

        frameData._lightness = value;
    }

    //调整眼镜大小
    public void ChangeGlassSize(Slider slider)
    {
        ChangeGlassSizeControl(slider.value);
    }

    void ChangeGlassSizeControl(float value)
    {
        glassSize.text = value.ToString();
        sizeRate = value;
        frameData._glassSize = value;
    }

    public void SaveFrameData()
    {
        SetFrameData();
        SetUIPanelActive(false);
        PlatformMgr.SavedFrameData();
    }

    public void CancelOperation()
    {
        GetFrameData();
        InitFrame();
        SetUIPanelActive(false);
        PlatformMgr.SavedFrameData();
    }

    void GetFrameData()
    {
        frameData._filterShader = PlayerPrefs.GetString("_filterShader");
        frameData._filterTexture = PlayerPrefs.GetInt("_filterTexture");
        frameData._filterLevel = PlayerPrefs.GetFloat("_filterLevel");
        frameData._colorTemperature = PlayerPrefs.GetFloat("_colorTemperature");
        frameData._bufferLevel = PlayerPrefs.GetFloat("_bufferLevel");
        frameData._lightness = PlayerPrefs.GetFloat("_lightness");
        frameData._glassSize = PlayerPrefs.GetFloat("_glassSize");
    }

    void SetFrameData()
    {
        PlayerPrefs.SetString("_filterShader", frameData._filterShader);
        PlayerPrefs.SetInt("_filterTexture", frameData._filterTexture);
        PlayerPrefs.SetFloat("_filterLevel", frameData._filterLevel);
        PlayerPrefs.SetFloat("_colorTemperature", frameData._colorTemperature);
        PlayerPrefs.SetFloat("_bufferLevel", frameData._bufferLevel);
        PlayerPrefs.SetFloat("_lightness", frameData._lightness);
        PlayerPrefs.SetFloat("_glassSize", frameData._glassSize);
    }

    void InitFrame()
    {
        FilterControl(frameData._filterTexture, frameData._filterShader);
        FilterLevelControl(frameData._filterLevel);
        FilterColorTemperatureControl(frameData._colorTemperature);
        BufferLevelControl(frameData._bufferLevel);
        ChangeLightnessControl(frameData._lightness);
        ChangeGlassSizeControl(frameData._glassSize);

        filterLevel_slider.value = frameData._filterLevel;
        colorTemperature_slider.value = frameData._colorTemperature;
        bufferLevel_slider.value = frameData._bufferLevel;
        lightness_slider.value = frameData._lightness;
        glassSize_slider.value = frameData._glassSize;
    }
}

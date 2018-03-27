using UnityEngine;
using System.Collections.Generic;
public class Base : MonoBehaviour
{
    private AppFacade m_Facade;
    private WebCamManager m_WebCamMgr;
    private MaterialsManager m_MaterialsMgr;
    private SendPlatformManager m_PlatformMgr;
    private ReceiveMgsManager m_ReceiveMgr;
    private GameManager m_GameMgr;
    private CameraFilterManager m_FilteMgr;
    private RecorderManager m_RecorderMgr;
    private AudioPlayerManager m_AudioMgr;
    private AnimatorSpineManager m_AnimSpineMgr;
    private UISceneManager m_UISceneMgr;
	private ResourceLuaManager m_ResLuaMgr;
	private FxManager m_FxManager;
    private Main m_GameMain;

    private UICameraFilterRender UI_FilterMgr;
    private CameraFilterEyeFace m_FilteEyeFace;
    /// <summary>
    /// 注册消息
    /// </summary>
    /// <param name="view"></param>
    /// <param name="messages"></param>
    protected void RegisterMessage(IView view, List<string> messages)
    {
        if (messages == null || messages.Count == 0)
            return;
        Controller.Instance.RegisterViewCommand(view, messages.ToArray());
    }

    /// <summary>
    /// 移除消息
    /// </summary>
    /// <param name="view"></param>
    /// <param name="messages"></param>
    protected void RemoveMessage(IView view, List<string> messages)
    {
        if (messages == null || messages.Count == 0)
            return;
        Controller.Instance.RemoveViewCommand(view, messages.ToArray());
    }

    protected AppFacade facade
    {
        get
        {
            if (m_Facade == null)
            {
                m_Facade = AppFacade.Instance;
            }
            return m_Facade;
        }
    }

    /// <summary>
    /// 摄像头类
    /// </summary>
    protected WebCamManager WebCamMgr
    {
        get
        {
            if (m_WebCamMgr == null)
            {
                m_WebCamMgr = facade.GetManager<WebCamManager>(ManagerName.WebCam);
            }
            return m_WebCamMgr;
        }
    }

    protected MaterialsManager MaterialsMgr
    {
        get
        {
            if (m_MaterialsMgr == null)
            {
                m_MaterialsMgr = facade.GetManager<MaterialsManager>(ManagerName.Materials);
            }
            return m_MaterialsMgr;
        }
    }

	protected ResourceLuaManager ResLuaMgr
    {
        get
        {
			if (m_ResLuaMgr == null)
            {
				m_ResLuaMgr = facade.GetManager<ResourceLuaManager>(ManagerName.ResourceLua);
            }
			return m_ResLuaMgr;
        }
    }

    protected SendPlatformManager PlatformMgr
    {
        get
        {
            if (m_PlatformMgr == null)
            {
                m_PlatformMgr = facade.GetManager<SendPlatformManager>(ManagerName.Platform);
            }
            return m_PlatformMgr;
        }
    }

    protected ReceiveMgsManager ReceiveMgr
    {
        get
        {
            if (m_ReceiveMgr == null)
            {
                m_ReceiveMgr = facade.GetManager<ReceiveMgsManager>(ManagerName.ReceiveMgs);
            }
            return m_ReceiveMgr;
        }
    }

    protected GameManager GameMgr
    {
        get
        {
            if (m_GameMgr == null)
            {
                m_GameMgr = facade.GetManager<GameManager>(ManagerName.Game);
            }
            return m_GameMgr;
        }
    }

    protected CameraFilterManager FilteMgr
    {
        get
        {
            if (m_FilteMgr == null)
            {
                m_FilteMgr = facade.GetManager<CameraFilterManager>(ManagerName.Filter);
            }
            return m_FilteMgr;
        }
    }
    protected CameraFilterEyeFace FilteEyeFace
    {
        get
        {
            if (m_FilteEyeFace == null)
            {
                m_FilteEyeFace = facade.GetManager<CameraFilterEyeFace>(ManagerName.FileterEyeFace);
            }
            return m_FilteEyeFace;
        }
    }
    protected UICameraFilterRender UIFilterMgr
    {
        get
        {
            if (UI_FilterMgr == null)
            {
                UI_FilterMgr = facade.GetManager<UICameraFilterRender>(ManagerName.UICameraFilterMgs);
            }
            return UI_FilterMgr;
        }
    }
    protected RecorderManager RecorderMgr
    {
        get
        {
            if (m_RecorderMgr == null)
            {
                m_RecorderMgr = facade.GetManager<RecorderManager>(ManagerName.Recorder);
            }
            return m_RecorderMgr;
        }
    }

    protected AudioPlayerManager AudioMgr
    {
        get
        {
            if (m_AudioMgr == null)
            {
                m_AudioMgr = facade.GetManager<AudioPlayerManager>(ManagerName.Audio);
            }
            return m_AudioMgr;
        }
    }

    protected AnimatorSpineManager AnimSpineMgr
    {
        get
        {
            if (m_AnimSpineMgr == null)
            {
                m_AnimSpineMgr = facade.GetManager<AnimatorSpineManager>(ManagerName.AnimSpine);
            }
            return m_AnimSpineMgr;
        }
    }

    protected UISceneManager UISceneMgr
    {
        get
        {
            if (m_UISceneMgr == null)
            {
                m_UISceneMgr = facade.GetManager<UISceneManager>(ManagerName.UIScene);
            }
            return m_UISceneMgr;
        }
    }

	protected FxManager FxMgr
	{
		get
		{
			if (m_FxManager == null)
			{
				m_FxManager = facade.GetManager<FxManager>(ManagerName.FxEffect);
			}
			return m_FxManager;
		}
	}

    protected Main GameMain
    {
        get
        {
            if (m_GameMain == null)
            {
                m_GameMain = facade.GetManager<Main>(ManagerName.GameMain);
            }
            return m_GameMain;
        }
    }
}
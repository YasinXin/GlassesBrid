using UnityEngine;
public class StartUpCommand : ControllerCommand
{
    public override void Execute(IMessage message)
    {
        //-----------------初始化管理器-----------------------
        AppFacade.Instance.AddManager<UISceneManager>(ManagerName.UIScene);
        AppFacade.Instance.AddManager<WebCamManager>(ManagerName.WebCam);
		AppFacade.Instance.AddManager<ResourceLuaManager>(ManagerName.ResourceLua);
        AppFacade.Instance.AddManager<MaterialsManager>(ManagerName.Materials);
        AppFacade.Instance.AddManager<ReceiveMgsManager>(ManagerName.ReceiveMgs);
        AppFacade.Instance.AddManager<CameraFilterManager>(ManagerName.Filter);
        AppFacade.Instance.AddManager<RecorderManager>(ManagerName.Recorder);
        AppFacade.Instance.AddManager<SendPlatformManager>(ManagerName.Platform);
        AppFacade.Instance.AddManager<AudioPlayerManager>(ManagerName.Audio);
		AppFacade.Instance.AddManager<FxManager>(ManagerName.FxEffect);
        AppFacade.Instance.AddManager<AnimatorSpineManager>(ManagerName.AnimSpine);
        AppFacade.Instance.AddManager<GameManager>(ManagerName.Game);
        AppFacade.Instance.AddManager<UICameraFilterRender>(ManagerName.UICameraFilterMgs);
        
    }
}
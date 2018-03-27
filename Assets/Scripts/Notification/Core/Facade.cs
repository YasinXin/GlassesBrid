/* 
   LidxFramework Code By kaikai
*/

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件命令
/// </summary>
public class ControllerCommand :Base, ICommand {
    public virtual void Execute(IMessage message) {
    }
}

public class Facade {
    protected MonitorEvent m_monitor;
    protected IController m_controller;
    static GameObject m_GameManager;
    static Dictionary<string, object> m_Managers = new Dictionary<string, object>();

    GameObject AppGameManager {
        get {
            if (m_GameManager == null) {
                m_GameManager = GameObject.Find("GameManager");
            }
            return m_GameManager;
        }
    }

    protected Facade() {
        InitFramework();
    }
    protected virtual void InitFramework() {
        if (m_controller != null) return;
        m_controller = Controller.Instance;

        if (m_monitor != null) return;
        m_monitor = MonitorEvent.Instance;
    }

    /// <summary>
    /// 注册事件命令
    /// </summary>
    /// <param name="commandName"></param>
    /// <param name="listener"></param>
    public void RegisterEventCommand(string commandName, EventListenerDelegate listener) {
        m_monitor.AddEventListener(commandName, listener);
    }

    /// <summary>
	/// 移除单个事件命令
	/// </summary>
	/// <param name="commandName">Command name.</param>
    public virtual void RemoveEventCommand(string commandName, EventListenerDelegate listener)
    {
        m_monitor.RemoveEventListener(commandName,listener);
    }

    /// <summary>
	/// 移除所有事件命令
	/// </summary>
	/// <param name="commandName">Command name.</param>
    public virtual void RemoveAllEventCommand()
    {
        m_monitor.RemoveAll();
    }

    /// <summary>
    /// 执行事件命令
    /// </summary>
    /// <param name="evt"></param>
    public void SendEventCommand(UEvent evt)
    {
        m_monitor.DispatchEvent(evt);
    }

    /// <summary>
    /// 注册多个事件消息命令
    /// </summary>
    /// <param name="commandType"></param>
    /// <param name="commandType"></param>
    public void RegisterMultiEventCommand(params Type[] commandType)
    {
        int count = commandType.Length;
        for (int i = 0; i < count; i++)
        {
            RegisterCommand(commandType[i].Name.ToString(), commandType[i]);
        }
    }

    /// <summary>
    /// 移除多个事件消息命令
    /// </summary>
    /// <param name="commandName"></param>
    public void RemoveMultiEventCommand(params Type[] commandType)
    {
        int count = commandType.Length;
        for (int i = 0; i < count; i++)
        {
            RemoveCommand(commandType[i].Name.ToString());
        }
    }

    /// <summary>
    /// 发送多个事件消息命令
    /// </summary>
    /// <param name="message"></param>
    /// <param name="commandType"></param>
    public void SendMultiEventCommand(params Type[] commandType)
    {
        int count = commandType.Length;
        for (int i = 0; i < count; i++)
        {
            m_controller.ExecuteCommand(new Message(commandType[i].Name.ToString(), null));
        }
    }

    /// <summary>
    /// 注册命令
    /// </summary>
    /// <param name="commandName">Command name.</param>
    /// <param name="commandType">Command type.</param>
    public virtual void RegisterCommand(string commandName, Type commandType) {
        m_controller.RegisterCommand(commandName, commandType);
    }

	/// <summary>
	/// 移除命令
	/// </summary>
	/// <param name="commandName">Command name.</param>
    public virtual void RemoveCommand(string commandName) {
        m_controller.RemoveCommand(commandName);
    }

    public virtual bool HasCommand(string commandName) {
        return m_controller.HasCommand(commandName);
    }

    /// <summary>
    /// 注册多个消息命令
    /// </summary>
    /// <param name="commandType"></param>
    /// <param name="commandNames"></param>
    public void RegisterMultiCommand(Type commandType, params string[] commandNames) {
        int count = commandNames.Length;
        for (int i = 0; i < count; i++) {
            RegisterCommand(commandNames[i], commandType);
        }
    }

    /// <summary>
    /// 移除多个消息命令
    /// </summary>
    /// <param name="commandName"></param>
    public void RemoveMultiCommand(params string[] commandName) {
        int count = commandName.Length;
        for (int i = 0; i < count; i++) {
            RemoveCommand(commandName[i]);
        }
    }

    /// <summary>
    /// 发送消息命令
    /// </summary>
    /// <param name="message"></param>
    /// <param name="body"></param>
    public void SendMessageCommand(string message, object body = null) {
        m_controller.ExecuteCommand(new Message(message, body));
    }

    /// <summary>
    /// 添加管理器
    /// </summary>
    public void AddManager(string typeName, object obj) {
        if (!m_Managers.ContainsKey(typeName)) {
            m_Managers.Add(typeName, obj);
        }
    }

    /// <summary>
    /// 添加Unity对象
    /// </summary>
    public T AddManager<T>(string typeName) where T : Component {
        object result = null;
        m_Managers.TryGetValue(typeName, out result);
        if (result != null) {
            return (T)result;
        }
        Component c = AppGameManager.AddComponent<T>();
        m_Managers.Add(typeName, c);
        return default(T);
    }

    /// <summary>
    /// 获取系统管理器
    /// </summary>
    public T GetManager<T>(string typeName) where T : class {
        if (!m_Managers.ContainsKey(typeName)) {
            return default(T);
        }
        object manager = null;
        m_Managers.TryGetValue(typeName, out manager);
        return (T)manager;
    }

    /// <summary>
    /// 删除管理器
    /// </summary>
    public void RemoveManager(string typeName) {
        if (!m_Managers.ContainsKey(typeName)) {
            return;
        }
        object manager = null;
        m_Managers.TryGetValue(typeName, out manager);
        Type type = manager.GetType();
        if (type.IsSubclassOf(typeof(MonoBehaviour))) {
            GameObject.Destroy((Component)manager);
        }
        m_Managers.Remove(typeName);
    }
}

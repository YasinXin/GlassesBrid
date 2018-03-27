using System;
using System.Collections;
using System.Collections.Generic;

public class UEvent
{
	/// <summary>
	/// 事件类别
	/// </summary>
	public string eventType;

	/// <summary>
	/// 参数
	/// </summary>
	public object eventParams;

	/// <summary>
	/// 事件抛出者
	/// </summary>
	public object target;

	public UEvent (string eventType, object eventParams = null, object target = null)
	{
		this.eventType = eventType;
		this.eventParams = eventParams;
		this.target = target;
	}
}

public delegate void EventListenerDelegate (UEvent evt);
public class MonitorEvent
{
	private static MonitorEvent _instance;
	private Hashtable listeners = new Hashtable ();

	public static MonitorEvent Instance
	{
		get{
			if (_instance == null) {
				_instance = new MonitorEvent();
			}
			return _instance;
		}
	}

	/// <summary>
	/// 侦听事件
	/// </summary>
	/// <param name="eventType">事件类别</param>
	/// <param name="callback">回调函数</param>
	public void AddEventListener (string eventType, EventListenerDelegate listener)
	{
		EventListenerDelegate ceventListenerDelegate = this.listeners [eventType] as EventListenerDelegate;
		ceventListenerDelegate = (EventListenerDelegate)Delegate.Combine (ceventListenerDelegate, listener);
		this.listeners [eventType] = ceventListenerDelegate;
	}

	/// <summary>
	/// 移除事件
	/// </summary>
	/// <param name="eventType">事件类别</param>
	/// <param name="callback">回调函数</param>
	public void RemoveEventListener (string eventType,EventListenerDelegate listener)
	{
		EventListenerDelegate ceventListenerDelegate = this.listeners [eventType] as EventListenerDelegate;
		if (ceventListenerDelegate != null) {
			ceventListenerDelegate = (EventListenerDelegate)Delegate.Remove (ceventListenerDelegate, listener);
		}
		this.listeners [eventType] = ceventListenerDelegate;
	}

	/// <summary>
	/// 发送事件
	/// </summary>
	/// <param name="evt">Evt.</param>
	/// <param name="gameObject">Game object.</param>
	public void DispatchEvent (UEvent evt)
	{
		EventListenerDelegate ceventListenerDelegate = this.listeners [evt.eventType] as EventListenerDelegate;
		if (ceventListenerDelegate != null) {
			try {
				ceventListenerDelegate (evt);
			} catch (Exception ex) {
				throw new Exception (string.Concat (new string[] {
					"Error dispatching event ",
					evt.eventType.ToString (),
					": ",
					ex.Message,
					" ",
					ex.StackTrace
				}), ex);
			}
		}
	}

	public void RemoveAll ()
	{
		this.listeners.Clear ();
	}
}
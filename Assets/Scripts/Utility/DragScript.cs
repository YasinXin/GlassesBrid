 using UnityEngine;

public class DragScript : MonoBehaviour {
    public static GameObject tagObj;
	public GameObject goDrag;
	static bool bol=false;
    void Start()
    {
        if(goDrag!=null)
        {
            EventTriggerListener.Get(goDrag).onDrag = OnDrag;
        }
	}

	/// <summary>
	/// 模型旋转事件
	/// </summary>
	/// <param name="go"></param>
	/// <param name="delta"></param>
	private void OnDrag(GameObject go, Vector2 delta)
	{
		    tagObj=GameObject.Find("face");
			if (tagObj != null)
			{
				if (tagObj.activeSelf)
				{
					UguiRotaionObject(tagObj);
				}
			}
	}

	/// <summary>
	/// 游戏人物对象旋转
	/// </summary>
	/// <param name="obj"></param>
	public static void UguiRotaionObject (GameObject obj)
	{
		float y = 0;
		y -= Input.GetAxis ("Mouse X") * 120 * Time.deltaTime*10;
		if (bol) {
			obj.transform.Rotate (new Vector3 (y, 0, 0));
		} else {
			obj.transform.Rotate (new Vector3 (0, y, 0));
		}
	}
}

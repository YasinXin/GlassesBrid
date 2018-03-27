using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour //因为继承MonoBehaviour的物体不能使用new()来构造实例 所以这里只能用MonoBehaviour
{
    private static T instance; //实例目标对象

    protected virtual void Awake()
    {
        this.CheckInstance(); //觉醒检查
    }

    protected bool CheckInstance()//是否唯一，不是就销毁多余的目标，防止目标有多个挂载在其他gameobject上。
    {
        if (this == Singleton<T>.Instance)
        {
            return true;
        }
        UnityEngine.Object.Destroy(this);
        return false;
    }

    public static T Instance
    {
        get
        {
            if (Singleton<T>.instance == null)
            {
                Singleton<T>.instance = (T)UnityEngine.Object.FindObjectOfType(typeof(T));// 如果对象没实例化，则为空
                if (Singleton<T>.instance == null)
                {
                    Debug.LogError(typeof(T) + " was no attached GameObject");
                }
            }
            return Singleton<T>.instance;
        }
    }
}

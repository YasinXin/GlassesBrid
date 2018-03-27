using UnityEngine;
using System.Collections;

public class TouchTest : MonoBehaviour {

    void OnEnable()
    {
        TouchEvent.SingleClick += OnSingleClick;
        TouchEvent.SingleLongPress += OnSingleLongPress;
        TouchEvent.MouseRightCilck += OnMouseRightClick;
    }

    void OnSingleClick()
    {
        Debug.Log("click");
    }

    void OnSingleLongPress()
    {
        //Test.myText2.text = "longPress";
        Debug.Log("longpress");
    }

    void OnMouseRightClick()
    {
        Debug.Log("click~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
    }
}

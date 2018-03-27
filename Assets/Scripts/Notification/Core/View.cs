using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class View : Base, IView {
    public virtual void OnMessage(IMessage message) {
    }
}

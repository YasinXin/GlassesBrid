using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvent : Manager {

//	// Use this for initialization
//	void Start () {
//		
//	}
//	
//	// Update is called once per frame
//	void Update () {
//		
//	}

	public void PlaySound(string _name)
	{
		//Debug.Log ("ANIM CALLBACK PlaySound = " + _name);
		if (_name == null) 
		{
			return;
		}

		AudioMgr.AudioPlay (_name);
	}

	public void StopSound(string _name)
	{
		//Debug.Log ("ANIM CALLBACK StopSound = " + _name);
		if (_name == null) 
		{
			return;
		}

		AudioMgr.AudioStop(_name);
	}

	public void PlayFx(string _name)
	{
		//Debug.Log ("ANIM CALLBACK PlayFx = " + _name);
		if (_name == null) 
		{
			return;
		}
		FxMgr.PlayFx (_name);
	}

	public void StopFx(string _name)
	{
		//Debug.Log ("ANIM CALLBACK StopFx = " + _name);
		if (_name == null) 
		{
			return;
		}
		FxMgr.StopFx (_name);
	}

	public void PauseFx(string _name)
	{
		//Debug.Log ("ANIM CALLBACK PauseFx = " + _name);
		if (_name == null) 
		{
			return;
		}
		FxMgr.PauseFx (_name);
	}

	public void AnimComplete(string _name)
	{
		//Debug.Log ("触发动画结束回调 AnimComplete value = " + _name);

		int count = MaterialsMgr.m_material3DBean.model.Count;

		//遍历所有模型获得有动画控制器的模型
		for (int i = 0; i < count; i++) 
		{
			//如果该模型有动画控制器，继续操作
			if (MaterialsMgr.m_material3DBean.anim != null) {
				//判断该动画片段中是否有触发
				if (true) {
					//如果有动画有触发操作，遍历动画集合获取触发条件名称
					for (int j = 0; j < MaterialsMgr.m_material3DBean.model [i].animations.Count; j++) {
						//获取动画条件名称
						string conditionsName = MaterialsMgr.m_material3DBean.model [i].animations [j].animConditions [0].conditionsName;
						string conditionsType = MaterialsMgr.m_material3DBean.model [i].animations [j].animConditions [0].conditionsType;
						//根据条件名称与传入的值设置动画状态机属性
						switch (conditionsType)
						{
							case "trigger":
								MaterialsMgr.m_material3DBean.anim [i].SetTrigger (conditionsName);
								//mouthTrigger = false;
								break;

							case "bool":
								MaterialsMgr.m_material3DBean.anim [i].SetBool (conditionsName, bool.Parse (_name));

								//							Debug.Log ("SET BOOL NAME = " + MaterialsMgr.m_material3DBean.model [i].animations [j].animConditions [n].conditionsName + 
								//							"    VALUE = " +  bool.Parse (MaterialsMgr.m_material3DBean.model [i].animations [j].animConditions [n].conditionsValue));
								break;

							case "int":
								MaterialsMgr.m_material3DBean.anim [i].SetInteger (conditionsName, int.Parse (_name));

//															Debug.Log ("SET INT NAME = " + MaterialsMgr.m_material3DBean.model [i].animations [j].animConditions [n].conditionsName + 
//															"    VALUE = " + int.Parse(MaterialsMgr.m_material3DBean.model [i].animations [j].animConditions [n].conditionsValue));
								break;

							case "float":

								break;
						}
					}
				}

			} else {
				Debug.Log ("model[ " + i + " ] = " + MaterialsMgr.m_material3DBean.model[i].name + " not anim component countinue this loop ! ");
				continue;
			}

		}

		//MaterialsMgr.m_material3DBean.anim [0].SetBool (MaterialsMgr.m_material3DBean.model [0].animations [0].animConditions[0].conditionsName, bool.Parse(_name));

//		string name = MaterialsMgr.m_material3DBean.model [0].animations [0].animConfigs [3].functionParameValue;
//
//		int count = MaterialsMgr.m_material3DBean.model [0].animations [0].animConfigs.Count;
//		Debug.Log ("count = " + count);
//
//		for (int i = 0; i < count; i++) {
//
//			Debug.Log (" AnimComplete name = " + MaterialsMgr.m_material3DBean.model [0].animations [0].animConfigs[i].functionParameValue);
//
//		}
//
//		Debug.Log ("===============  " + MaterialsMgr.m_material3DBean.model [0].animations [0].animConfigs[4].functionParameValue);
//


		//根据重置动画条件
		//		if (_value == "animConditions") 
		//		{

		//default value
		//int = -1;
		//float = -1.0
		//bool = false;
		//
		//			//模型数量
		//			int modelCount = MaterialsMgr.m_material3DBean.model.Count;
		//			for (int i = 0; i < modelCount; i++) 
		//			{
		//				//动画数量
		//				int animationCount = MaterialsMgr.m_material3DBean.model[i].animations.Count;
		//				for (int j = 0; j < animationCount; j++)
		//				{
		//					//条件数量
		//					int conditionsCount = MaterialsMgr.m_material3DBean.model [i].animations [j].animConditions.Count;
		//					for (int n = 0; n < conditionsCount; n++)
		//					{
		//						string type = MaterialsMgr.m_material3DBean.model [i].animations [j].animConditions [n].conditionsType;
		//
		//						switch(type)
		//						{
		//						case "trigger":
		//							MaterialsMgr.m_material3DBean.anim [i].SetTrigger (MaterialsMgr.m_material3DBean.model [i].animations [j].animConditions [n].conditionsName);
		//							//mouthTrigger = false;
		//							break;
		//
		//						case "bool":
		//							MaterialsMgr.m_material3DBean.anim [i].SetBool (MaterialsMgr.m_material3DBean.model [i].animations [j].animConditions [n].conditionsName,
		//								bool.Parse (MaterialsMgr.m_material3DBean.model [i].animations [j].animConditions [n].conditionsValue));
		//
		//							//							Debug.Log ("SET BOOL NAME = " + MaterialsMgr.m_material3DBean.model [i].animations [j].animConditions [n].conditionsName + 
		//							//							"    VALUE = " +  bool.Parse (MaterialsMgr.m_material3DBean.model [i].animations [j].animConditions [n].conditionsValue));
		//							break;
		//
		//						case "int":
		//							MaterialsMgr.m_material3DBean.anim [i].SetInteger (MaterialsMgr.m_material3DBean.model [i].animations [j].animConditions [n].conditionsName,
		//							int.Parse(_value));
		//
		//														Debug.Log ("SET INT NAME = " + MaterialsMgr.m_material3DBean.model [i].animations [j].animConditions [n].conditionsName + 
		//														"    VALUE = " + int.Parse(MaterialsMgr.m_material3DBean.model [i].animations [j].animConditions [n].conditionsValue));
		//							break;
		//
		//						case "float":
		//
		//							break;
		//						}
		//					}
		//				}
		//			}
		//}
	}

		void Update()
		{
	
			//DownTest
			if (Input.GetKeyDown(KeyCode.O))
			{
				Debug.Log ("GetKeyDown - O");
	
			MaterialsMgr.m_material3DBean.anim [0].SetBool (MaterialsMgr.m_material3DBean.model [0].animations [0].animConditions [0].conditionsName,
				bool.Parse(MaterialsMgr.m_material3DBean.model [0].animations [0].animConditions [0].conditionsValue));

//			 Debug.Log ("SET INT NAME = " + MaterialsMgr.m_material3DBean.model [0].animations [0].animConditions [0].conditionsName + 
//				 "    VALUE = " + int.Parse(MaterialsMgr.m_material3DBean.model [0].animations [0].animConditions [0].conditionsValue));

//				MaterialsMgr.m_material3DBean.anim [0].SetBool (MaterialsMgr.m_material3DBean.model [0].animations [0].animConditions [0].conditionsName,
//													bool.Parse (MaterialsMgr.m_material3DBean.model [0].animations [0].animConditions [0].conditionsValue));
//							    Debug.Log ("SET BOOL NAME = " + MaterialsMgr.m_material3DBean.model [0].animations [0].animConditions [0].conditionsName + 
//							  	    "  VALUE = " +  bool.Parse (MaterialsMgr.m_material3DBean.model [0].animations [0].animConditions [0].conditionsValue));
	
			}
	
			//UpTest
			if (Input.GetKeyDown(KeyCode.P))
			{
				Debug.Log ("GetKeyDown - P");

			MaterialsMgr.m_material3DBean.anim [0].SetBool (MaterialsMgr.m_material3DBean.model [0].animations [1].animConditions [0].conditionsName,
				bool.Parse(MaterialsMgr.m_material3DBean.model [0].animations [1].animConditions [0].conditionsValue));

//			Debug.Log ("SET INT NAME = " + MaterialsMgr.m_material3DBean.model [0].animations [1].animConditions [0].conditionsName + 
//				"    VALUE = " + int.Parse(MaterialsMgr.m_material3DBean.model [0].animations [1].animConditions [0].conditionsValue));
//			

//			    MaterialsMgr.m_material3DBean.anim [0].SetBool (MaterialsMgr.m_material3DBean.model [0].animations [1].animConditions [0].conditionsName,
// 													bool.Parse (MaterialsMgr.m_material3DBean.model [0].animations [1].animConditions [0].conditionsValue));
//								Debug.Log ("SET BOOL NAME = " + MaterialsMgr.m_material3DBean.model [0].animations [1].animConditions [0].conditionsName + 
//									"  VALUE = " +  bool.Parse (MaterialsMgr.m_material3DBean.model [0].animations [1].animConditions [0].conditionsValue));
			}

		if (Input.GetKeyDown(KeyCode.U)) {
			//StopSound ("fire");
			PlaySound("bgm_default");
		}

		}
}

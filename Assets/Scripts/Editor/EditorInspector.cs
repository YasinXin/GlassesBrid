using System.Collections.Generic;
using UnityEngine;
using UnityEditor; 

public class EditorInspector : CustomMaterialEditor
{
	protected override void CreateToggleList()
	{
		Toggles.Add(new FeatureEditor("Normal Enabled","normal","NORMALMAP_ON","NORMALMAP_OFF"));
		Toggles.Add(new FeatureEditor("Specular Enabled","specular","SPECULAR_ON","SPECULAR_OFF"));
		Toggles.Add(new FeatureEditor("Fresnel Enabled","fresnel","FRESNEL_ON","FRESNEL_OFF"));
		Toggles.Add(new FeatureEditor("Rim Light Enabled","rim","RIMLIGHT_ON","RIMLIGHT_OFF"));
	}
}
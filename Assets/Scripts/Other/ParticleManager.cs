using UnityEngine;
using System.Collections;

public class ParticleManager : MonoBehaviour {
    public GameObject[] particleObj;
    private GameObject showParticle;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnParticleClick(int index)
    {
        ParticleControl(index);
    }

    public void ParticleControl(int index)
    {
        if(showParticle != null)
        {
            showParticle.SetActive(false);
        }
        if(index > 0)
        {
            showParticle = particleObj[index - 1];
            showParticle.SetActive(true);
        }
    }
}

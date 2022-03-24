using UnityEngine;
using System.Collections;

public class Torchelight : MonoBehaviour {	
	public GameObject TorchLight;
	public GameObject MainFlame;
	public GameObject BaseFlame;
	public GameObject Etincelles;
	public GameObject Fumee;
	public float MaxLightIntensity;
	public float IntensityLight;
    Light m_light;
    ParticleSystem.EmissionModule mainFlame;
    ParticleSystem.EmissionModule baseFlame;
    ParticleSystem.EmissionModule etincelles;
    ParticleSystem.EmissionModule fumee;
    void Start ()
    {
        m_light = TorchLight.GetComponent<Light>();
        m_light.intensity=IntensityLight;
        mainFlame = MainFlame.GetComponent<ParticleSystem>().emission;
        baseFlame = BaseFlame.GetComponent<ParticleSystem>().emission;
        etincelles = Etincelles.GetComponent<ParticleSystem>().emission;
        fumee = Fumee.GetComponent<ParticleSystem>().emission;
        mainFlame.rateOverTime = new ParticleSystem.MinMaxCurve(IntensityLight * 20f);
        baseFlame.rateOverTime = new ParticleSystem.MinMaxCurve(IntensityLight * 15f);
        etincelles.rateOverTime = new ParticleSystem.MinMaxCurve(IntensityLight * 7f);
        fumee.rateOverTime = new ParticleSystem.MinMaxCurve(IntensityLight * 12f);
    }	
	void Update ()
    {
		if (IntensityLight<0) IntensityLight=0;
		if (IntensityLight>MaxLightIntensity) IntensityLight=MaxLightIntensity;		
		m_light.intensity=IntensityLight/2f+Mathf.Lerp(IntensityLight-0.1f,IntensityLight+0.1f,Mathf.Cos(Time.time*30));
		m_light.color=new Color(Mathf.Min(IntensityLight/1.5f,1f),Mathf.Min(IntensityLight/2f,1f),0f);
        mainFlame.rateOverTime = new ParticleSystem.MinMaxCurve(IntensityLight * 20f);
        baseFlame.rateOverTime = new ParticleSystem.MinMaxCurve(IntensityLight * 15f);
        etincelles.rateOverTime = new ParticleSystem.MinMaxCurve(IntensityLight * 7f);
        fumee.rateOverTime = new ParticleSystem.MinMaxCurve(IntensityLight * 12f);
    }
}

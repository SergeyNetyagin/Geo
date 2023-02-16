using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightContol : MonoBehaviour {

    [Space( 10 )]
    public AnimationCurve light_curve;

    private Light vehicle_light;

    private float light_ratio = 1f;

	// Use this for initialization
	private void Start() {
		
        vehicle_light = GetComponent<Light>();

        light_ratio = 1f / 86400f;
	}
	
	// Update is called once per frame
	private void Update() {
		
        //vehicle_light.intensity = light_curve.Evaluate( OneDaySystem.seconds * light_ratio );
	}
}

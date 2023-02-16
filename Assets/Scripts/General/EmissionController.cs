using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionController : MonoBehaviour
{
    public Material material;

    [Space( 10 )]
    // Lights OFF: 22000 / 86400
    // Lights ON: 61000 / 86400
    public AnimationCurve colorCurve = new AnimationCurve( new Keyframe( 0f, 1f ), new Keyframe( 1f, 1f) );

    private Color startEmissionColor;

    private float emission_ratio = 1f;

    // Use this for initialization
    void Start() {

        startEmissionColor = material.GetColor( "_EmissionColor" );

        emission_ratio = 1f / 86400f;
    }

    // Update is called once per frame
    void Update() {

        //material.SetColor( "_EmissionColor", startEmissionColor * colorCurve.Evaluate( OneDaySystem.seconds * emission_ratio ) );
    }

    private void OnDisable() {

        material.SetColor( "_EmissionColor", startEmissionColor );
    }
}

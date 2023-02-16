using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightBlinker : MonoBehaviour
{
    public AnimationCurve blinkCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
    public Gradient colorGradient = new Gradient();
    public float speed = 1f;

    private Light lightComponent;

    // Use this for initialization
    void Start()
    {
        lightComponent = GetComponent<Light>();
        if (lightComponent == null)
        {
            Destroy(this);

            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float currentTime = Mathf.Repeat(Time.time * speed, 1f);
        lightComponent.intensity = blinkCurve.Evaluate(currentTime);
        lightComponent.color =  colorGradient.Evaluate(currentTime);
    }
}
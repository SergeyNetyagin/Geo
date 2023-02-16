using UnityEngine;
using System.Collections;

public class VolumeToEmission : MonoBehaviour
{ 
    [Header("Source")]
    public AudioSource audioSource;
    public int qSamples = 1024;

    [Header("Material")]
    [SerializeField]
    int materialIndex;
    public string colorPropertyName;

    [Header("Color options")]
    [ColorUsageAttribute(true, true, 0f, 8f, 0.125f, 3f)]
    public Color quietColor;
    [ColorUsageAttribute(true, true, 0f, 8f, 0.125f, 3f)]
    public Color loudColor;

    [Range(0f, 60f)]
    public float multiper = 2f;
    [Range(0f, 60f)]
    public float risingRoughness = 2f;
    [Range(0f, 60f)]
    public float fallingRoughness = 2f;

    //private  
    Material currentMaterial;
    AudioClip lastPlayedClip;
    float emissionStrength;
    float[] sampleData = new float[0];

    void Start()
    {
        currentMaterial = GetComponent<Renderer>().materials[materialIndex];
        currentMaterial.EnableKeyword("_EMISSION");     //in case of standard / standard (specular setup) shader
    }

    void Update()
    {
        float newStrength = audioSource.isPlaying ? GetCurrentSample() : 0f;

        if(newStrength > emissionStrength)                                              //if sound is rising
            emissionStrength = Mathf.Lerp(emissionStrength, newStrength, risingRoughness * Time.deltaTime);
        else
            emissionStrength = Mathf.Lerp(emissionStrength, newStrength, fallingRoughness * Time.deltaTime);
        
        currentMaterial.SetColor(colorPropertyName, Color.Lerp(quietColor, loudColor, Mathf.Clamp01(emissionStrength * multiper)));
    }

    float GetCurrentSample()                                        //returns values between 0-1
    {
        if(lastPlayedClip != audioSource.clip)                      //clip changed
        {
            lastPlayedClip = audioSource.clip;
            sampleData = new float[audioSource.clip.samples];       
            audioSource.clip.GetData(sampleData, 1);                //extract sample data from new clip to array 
        }

        try
        {
            //return Mathf.Abs((sampleData[audioSource.timeSamples]));  //это сложно объяснить. В случае чего есть catch
            float sum = 0;                                          
            for(int i = 0; i<qSamples; i++)
                sum += Mathf.Abs(sampleData[audioSource.timeSamples + i]);

            return sum / qSamples;
        }
        catch(System.IndexOutOfRangeException exc)                  //PlayOneShot() exception - я не смог додуматься, как получить клип, который проигрывается с помощью PlayOneShot()
        {
            return 0f;
        }       
    }
}


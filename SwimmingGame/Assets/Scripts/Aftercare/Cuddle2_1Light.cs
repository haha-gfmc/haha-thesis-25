using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cuddle2_1Light : MonoBehaviour
{
    public Light directionalLight;
    public List<Color> colorList; // List of colors for the nightclub effect
    public float lightPulseInterval = 2f; // Interval for light sine wave
    //public float fogPulseInterval = 3f; // Interval for fog sine wave
    public float lightChangeInterval = 1f; // Interval for changing light color
    //public float fogChangeInterval = 1f; // Interval for changing fog color
    //public Color fogOffColor = Color.black;
    public Color lightOffColor = Color.black;


    private Color currentLightColor;
    private Color currentFogColor;
    private float lightPulseTimer = 0f;
    //private float fogPulseTimer = 0f;
    private float lightChangeTimer = 0f;
    private float fogChangeTimer = 0f;

    void Start()
    {
        if (directionalLight != null)
        {
            currentLightColor = directionalLight.color;
        }
        currentFogColor = RenderSettings.fogColor;
    }

    void Update()
    {
        
        //Update light pulse
        if (directionalLight != null)
        {
            lightPulseTimer += Time.deltaTime;
            float lightFrequency = (Mathf.PI * 2) / lightPulseInterval; // Frequency based on interval
            float lightIntensity = Mathf.Sin(lightPulseTimer * lightFrequency) * 0.5f + 0.5f; // Sine wave between 0 and 1
            directionalLight.color = Color.Lerp(lightOffColor, currentLightColor, lightIntensity);

            // Change light color at intervals
            lightChangeTimer += Time.deltaTime;
            if (lightChangeTimer >= lightChangeInterval)
            {
                lightChangeTimer = 0f;
                currentLightColor = GetRandomColor(currentLightColor);
            }
        }

        // Update fog pulse
        /*
        fogPulseTimer += Time.deltaTime;
        float fogFrequency = (Mathf.PI * 2) / fogPulseInterval; // Frequency based on interval
        float fogIntensity = Mathf.Sin(fogPulseTimer * fogFrequency) * 0.5f + 0.5f; // Sine wave between 0 and 1
        RenderSettings.fogColor = Color.Lerp(fogOffColor, currentFogColor, fogIntensity);
        

        // Change fog color at intervals
        fogChangeTimer += Time.deltaTime;
        if (fogChangeTimer >= fogChangeInterval)
        {
            fogChangeTimer = 0f;
            currentFogColor = GetRandomColor(currentFogColor);
        }
        */
    }

    private Color GetRandomColor(Color excludeColor)
    {
        if (colorList == null || colorList.Count == 0)
        {
            return excludeColor; // Return the same color if the list is empty
        }

        Color newColor = colorList[Random.Range(0, colorList.Count)];
        if (newColor == excludeColor)
        {
            newColor = colorList[Random.Range(0, colorList.Count)];
        }

        return newColor;
    }
}

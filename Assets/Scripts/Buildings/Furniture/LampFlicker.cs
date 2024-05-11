using System.Collections.Generic;
using UnityEngine;

// https://gist.github.com/sinbad/4a9ded6b00cf6063c36a4837b15df969
// Adjusted from above example to make flickering frame rate independent
// ...and of course added day/night cycle
[RequireComponent(typeof(Light))]
public class LampFlicker : MonoBehaviour
{
    [SerializeField] private int smoothingQueueLength = 5;
    [SerializeField] private float flickerFrequency = 0.1f;
    
    private Queue<float> smoothingQueue;
    private Light theLight;
    private float lastSum = 0;
    private float maxIntensity;
    private float minIntensity;
    private float lastFlickered = float.MinValue;
    private float minLightLevel;

    // Start is called before the first frame update
    void Start()
    {
        theLight = GetComponent<Light>();
        smoothingQueue = new(smoothingQueueLength);
        maxIntensity = theLight.intensity * 1.2f;
        minIntensity = theLight.intensity / 2f;

        // Assign a random light level where this lamp will light - so they don't all come on at once!
        minLightLevel = Random.Range(0.4f, 0.6f);
    }

    // Update is called once per frame
    void Update()
    {
        if(DayNightCycleController.Instance.LightLevel > minLightLevel)
        {
            theLight.intensity = 0f;
            return;
        }

        if(Time.time - lastFlickered < flickerFrequency)
            return;

        lastFlickered = Time.time;

        while(smoothingQueue.Count > smoothingQueueLength)
            lastSum -= smoothingQueue.Dequeue();

        float newValue = Random.Range(minIntensity, maxIntensity);
        smoothingQueue.Enqueue(newValue);
        lastSum += newValue;

        theLight.intensity = lastSum / smoothingQueue.Count;
    }
}

using UnityEngine;

public class Intravel_Sound : MonoBehaviour 
{

    public AudioSource intervalAudioSource; // assign in Inspector
    public AudioClip intervalClip; // assign your sound
    private float intervalTime = 60f; // 0 seconds


    private float intervalTimer;


    public AudioSource intervalAudioSource_2;
    public AudioClip intervalClip_2;
    private float intervalTime_2 = 0f; // 0 seconds

    private float intervalTimer_2;

    void Start()
    {
        intervalAudioSource = gameObject.AddComponent<AudioSource>();
        
        // Assign your audio clip in the Inspector or load it here
        // intervalClip = Resources.Load<AudioClip>("YourClipName");
        intervalAudioSource.clip = intervalClip;
        intervalAudioSource.loop = false; // Play once per interval
        intervalTimer = intervalTime; // Initialize timer

        intervalAudioSource_2 = gameObject.AddComponent<AudioSource>();
        intervalAudioSource_2.clip = intervalClip_2;
        intervalAudioSource_2.loop = false; // Play once per interval
        intervalTimer_2 = intervalTime_2; // Initialize timer
    }
    void Update()
    {
        intervalTimer -= Time.deltaTime;
        if (intervalTimer <= 0f)
        {
            Debug.Log("Interval sound played");
            PlayIntervalSound();
            intervalTimer = intervalTime; // Reset timer
        }
        intervalTimer_2 += Time.deltaTime;
        if (intervalTimer_2 >= 180f)
        {
            Debug.Log("Sourounding  sound played");
            PlayIntervalSound_2();
            intervalTimer_2 = intervalTime_2; // Reset timer
        }
    }
    private void PlayIntervalSound()
    {
        if (intervalClip != null && intervalAudioSource != null)
        {
            intervalAudioSource.PlayOneShot(intervalClip);
        }
    }
    private void PlayIntervalSound_2()
    {
        if (intervalClip_2 != null && intervalAudioSource_2 != null)
        {
            intervalAudioSource_2.PlayOneShot(intervalClip_2);
        }
    }


}

using UnityEngine;
using UnityEngine.SceneManagement; // needed for scene loading

public class ControlManager : MonoBehaviour
{
    public static ControlManager Instance;
    public bool useGyroscope = false;

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    public void SetClassicMobile()
    {
        useGyroscope = false;
        LoadFPSScene();
    }

    public void SetGyroscope()
    {
        if (SystemInfo.supportsGyroscope)
        {
            useGyroscope = true;
            Input.gyro.enabled = true;
        }
        else
        {
            Debug.Log("Gyroscope not supported, falling back to Classic Mobile");
            useGyroscope = false;
        }

        LoadFPSScene();
    }

    private void LoadFPSScene()
    {
        SceneManager.LoadScene("Chenimalai"); // Make sure this matches your scene name
    }
    public enum ControlMode
    {
        Classic,
        Gyro
    }
    public ControlMode controlMode = ControlMode.Classic;
}

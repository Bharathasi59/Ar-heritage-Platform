using UnityEngine;

public class SceneOrientation : MonoBehaviour
{
    public enum OrientationType { Portrait, Landscape }
    public OrientationType orientation;

    void Start()
    {
        if (orientation == OrientationType.Portrait)
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }
        else if (orientation == OrientationType.Landscape)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft; // or LandscapeRight
        }
    }
}

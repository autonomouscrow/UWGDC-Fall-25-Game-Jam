using UnityEngine;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
    public static EndingManager Instance;

    [Header("UI Elements")]
    public GameObject EndingParent;
    public Image jailEnd;
    public Image stabEnd;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        EndingParent.SetActive(false);
            stabEnd.enabled = false;
    }

    public void SetEnding(string e)
    {
        InputManager.endMode = true;
        if (e == "jail")
        {

        }
        else if (e == "stab")
        {
            EndingParent.SetActive(true);
            stabEnd.enabled = true;
        }
        else if (e == "darryl")
        {

        }
        else if (e == "gone")
        {

        }
    }
}

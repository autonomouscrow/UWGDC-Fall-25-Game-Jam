using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
    public static Vector2 Movement;
    public static bool Talk;
    public static bool Doors;

    public static bool endMode = false;

    public PlayerInput playerInput;
    public InputAction moveAction;
    public InputAction talkAction;
    public InputAction doorsAction;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        talkAction = playerInput.actions["Talk"];
        doorsAction = playerInput.actions["Doors"];
    }

    // Update is called once per frame
    void Update()
    {
        if (endMode)
        {
            Movement = Vector2.zero;
            Talk = false;
            Doors = false;
        }
        else
        {
            Movement = moveAction.ReadValue<Vector2>();
            Talk = talkAction.triggered && talkAction.ReadValue<float>() > 0;
            Doors = doorsAction.triggered && doorsAction.ReadValue<float>() > 0;
        }
    }
}

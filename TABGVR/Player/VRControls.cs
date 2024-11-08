using UnityEngine;
using UnityEngine.XR;

namespace TABGVR.Player;
    
public class VRControls : MonoBehaviour
{
    private WeaponHandler weaponHandler;
    private MovementHandler movementHandler;
    private InputHandler inputHandler;
    private global::Player player;
    private Transform rotationX;

    private static Vector3 movementVector;

    private float triggerDeadZone = 0.7f;
    private void Start()
    {
        weaponHandler = GetComponent<WeaponHandler>();
        movementHandler = GetComponent<MovementHandler>();
        inputHandler = GetComponent<InputHandler>();
        player = GetComponent<global::Player>();
        rotationX = gameObject.GetComponentInChildren<RotationTarget>().transform.parent;

        inputHandler.enabled = false;
    }
    
    private void Update()
    {
        Controllers.LeftHandXR.TryGetFeatureValue(CommonUsages.primary2DAxis, out var leftJoystick);
        Controllers.RightHandXR.TryGetFeatureValue(CommonUsages.primary2DAxis, out var rightJoystick);
        
        Controllers.LeftHandXR.TryGetFeatureValue(CommonUsages.trigger, out var leftTrigger);
        Controllers.RightHandXR.TryGetFeatureValue(CommonUsages.trigger, out var rightTrigger);

        if (rightTrigger > triggerDeadZone)
        {
            weaponHandler.HoldAttack(true, false);
        }
        
        movementVector = rotationX.rotation * new Vector3(leftJoystick.x, 0.0f, leftJoystick.y);
        
        inputHandler.inputMovementDirection = movementVector;
    }
}
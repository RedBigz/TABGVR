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

    internal const float TriggerDeadZone = 0.7f;

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
        if (movementHandler.death.dead) return;

        Controllers.RightHandXR.TryGetFeatureValue(CommonUsages.primary2DAxis, out var rightJoystick);
        Controllers.LeftHandXR.TryGetFeatureValue(CommonUsages.primary2DAxis, out var leftJoystick);

        Controllers.RightHandXR.TryGetFeatureValue(CommonUsages.trigger, out var rightTrigger);
        Controllers.LeftHandXR.TryGetFeatureValue(CommonUsages.trigger, out var leftTrigger);

        Controllers.RightHandXR.TryGetFeatureValue(CommonUsages.primaryButton, out var aButton);
        Controllers.LeftHandXR.TryGetFeatureValue(CommonUsages.primaryButton, out var xButton);
        
        Controllers.RightHandXR.TryGetFeatureValue(CommonUsages.secondaryButton, out var bButton);
        Controllers.LeftHandXR.TryGetFeatureValue(CommonUsages.secondaryButton, out var yButton);

        if (rightTrigger > TriggerDeadZone)
        {
            weaponHandler.HoldAttack(true, false);
        }
        
        var movementVector = new Vector3(leftJoystick.x, 0.0f, leftJoystick.y);

        inputHandler.inputMovementDirection = rotationX.rotation * movementVector;
    }
}
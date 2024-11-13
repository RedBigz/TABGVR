using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.XR;

namespace TABGVR.Player;

public class VRControls : MonoBehaviour
{
    private WeaponHandler weaponHandler;
    private MovementHandler movementHandler;
    private InputHandler inputHandler;
    private InteractionHandler interactionHandler;
    private global::Player player;
    private Transform rotationX;
    private HaxInput haxInput;

    [CanBeNull] private Pickup currentPickup;

    internal const float TriggerDeadZone = 0.7f;

    private void Start()
    {
        weaponHandler = GetComponent<WeaponHandler>();
        movementHandler = GetComponent<MovementHandler>();
        inputHandler = GetComponent<InputHandler>();
        interactionHandler = GetComponent<InteractionHandler>();
        player = GetComponent<global::Player>();
        rotationX = gameObject.GetComponentInChildren<RotationTarget>().transform.parent;

        haxInput = HaxInput.Instance;

        interactionHandler.canPickUpAction = pickup => currentPickup = pickup;

        inputHandler.enabled = false;
    }

    private bool _rightTriggered;
    private bool _leftTriggered;

    private bool _aButtonPressed;
    private bool _bButtonPressed;
    private bool _xButtonPressed;
    private bool _yButtonPressed;
    
    /// <summary>
    /// Picks up selected <see cref="Pickup"/>.
    /// </summary>
    private void PickupInteract()
    {
        if (currentPickup && !currentPickup.hasBeenPickedUp && interactionHandler.pickupCor == null)
            interactionHandler.StartCoroutine(interactionHandler.StartPickup(currentPickup,
                currentPickup.weaponType == global::Pickup.WeaponType.Weapon &&
                !player.m_inventory.HasSpaceForWeapon(currentPickup)));
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
        
        Controllers.RightHandXR.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out var rightClick);

        if (rightTrigger > TriggerDeadZone)
        {
            if (!_rightTriggered)
            {
                if (weaponHandler.rightWeapon) weaponHandler.PressAttack(true, false);
                else PickupInteract();
            }
            else if (weaponHandler.rightWeapon) weaponHandler.HoldAttack(true, false);
        }

        if (leftTrigger > TriggerDeadZone)
        {
            if (!_leftTriggered)
            {
                if (weaponHandler.leftWeapon) weaponHandler.PressAttack(false, false);
                else PickupInteract();
            }
            else if (weaponHandler.leftWeapon) weaponHandler.HoldAttack(false, false);
        }

        _rightTriggered = rightTrigger > TriggerDeadZone;
        _leftTriggered = leftTrigger > TriggerDeadZone;

        if (rightClick)
            weaponHandler.CurrentWeapon = Pickup.EquipSlots.None;

        var movementVector = new Vector3(leftJoystick.x, 0.0f, leftJoystick.y);

        inputHandler.inputMovementDirection = rotationX.rotation * movementVector;

        if (aButton && !_aButtonPressed) movementHandler.Jump();
        if (bButton && !_bButtonPressed)
        {
            weaponHandler.rightWeapon?.gun.ReloadGun();
            weaponHandler.leftWeapon?.gun.ReloadGun();
        }

        _aButtonPressed = aButton;
        _bButtonPressed = bButton;
        _xButtonPressed = xButton;
        _yButtonPressed = yButton;
    }
}
using JetBrains.Annotations;
using Landfall.Network;
using TABGVR.Util;
using UnityEngine;
using UnityEngine.XR;

namespace TABGVR.Player;

public class VRControls : MonoBehaviour
{
    internal const float TriggerDeadZone = 0.7f;
    internal const float StopSprintingThreshold = 0.1f;
    internal const float SwapWeaponThreshold = 0.8f;

    internal bool _aButtonPressed;
    internal bool _bButtonPressed;
    internal bool _xButtonPressed;
    internal bool _yButtonPressed;

    internal bool _leftTriggered;
    internal bool _rightTriggered;

    public static bool SomethingTriggered;
    public static bool GetSomethingTriggered() => SomethingTriggered;

    internal bool _menuButtonPressed;

    internal bool _weaponUpPressed;
    internal bool _weaponDownPressed;

    internal bool _snapRightPressed;
    internal bool _snapLeftPressed;

    [CanBeNull] private Pickup currentPickup;
    private HaxInput haxInput;
    private InputHandler inputHandler;
    private InteractionHandler interactionHandler;
    private MovementHandler movementHandler;
    private global::Player player;
    private Transform rotationX;
    private WeaponHandler weaponHandler;

    private MenuTransitions menuTransitions;
    private MapHandler mapHandler;

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
        interactionHandler.canNotPickUpAction = () => currentPickup = null;

        inputHandler.enabled = false;

        menuTransitions = InventoryUI.instance.gameObject.GetComponent<MenuTransitions>();
        mapHandler = InventoryUI.instance.gameObject.GetComponentInChildren<MapHandler>();
    }

    private void SwapWeaponViaOffset(int offset)
    {
        if (weaponHandler.CurrentWeapon == Pickup.EquipSlots.ThrowableSlot) return; // TODO: Implement grenades

        if (weaponHandler.CurrentWeapon == Pickup.EquipSlots.None)
        {
            weaponHandler.CurrentWeapon = offset >= 0
                ? Pickup.EquipSlots.WeaponSlot01
                : Pickup.EquipSlots.WeaponSlot03;

            return;
        }

        for (var i = 0; i < MathUtil.CanonicalMod(offset, 3); i++)
        {
            weaponHandler.CurrentWeapon = weaponHandler.CurrentWeapon switch
            {
                Pickup.EquipSlots.WeaponSlot01 => Pickup.EquipSlots.WeaponSlot02,
                Pickup.EquipSlots.WeaponSlot02 => Pickup.EquipSlots.WeaponSlot03,
                Pickup.EquipSlots.WeaponSlot03 => Pickup.EquipSlots.WeaponSlot01,
            };
        }
    }

    private void Update()
    {
        // update interactor visibility
        if (global::Player.usingInterface != UIPorter.InteractorVisuals)
            UIPorter.InteractorVisuals = global::Player.usingInterface;

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
        Controllers.LeftHandXR.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out var leftClick);

        Controllers.LeftHandXR.TryGetFeatureValue(CommonUsages.menuButton, out var menuButtonPressed);

        // Menu
        if (menuButtonPressed && !_menuButtonPressed)
        {
            switch (MenuState.CurrentMenuState)
            {
                case MenuState.TABGMenuState.Escape:
                case MenuState.TABGMenuState.Options:
                    menuTransitions.GoToMain();
                    break;
                default:
                    menuTransitions.GoToEscape();
                    break;
            }
        }

        _menuButtonPressed = menuButtonPressed;

        // Sprinting
        if (leftClick && !inputHandler.isSpringting)
            inputHandler.isSpringting = true;

        inputHandler.isSpringting &= leftJoystick.magnitude > StopSprintingThreshold;

        // Right Trigger
        if (rightTrigger > TriggerDeadZone)
        {
            if (!_rightTriggered)
            {
                if (weaponHandler.rightWeapon) weaponHandler.PressAttack(true, false);
                else PickupInteract();
            }
            else if (weaponHandler.rightWeapon)
            {
                weaponHandler.HoldAttack(true, false);
            }
        }

        // Left Trigger
        if (leftTrigger > TriggerDeadZone)
        {
            if (!_leftTriggered)
            {
                if (weaponHandler.leftWeapon) weaponHandler.PressAttack(false, false);
                else PickupInteract();
            }
            else if (weaponHandler.leftWeapon)
            {
                weaponHandler.HoldAttack(false, false);
            }
        }

        _rightTriggered = rightTrigger > TriggerDeadZone;
        _leftTriggered = leftTrigger > TriggerDeadZone;

        SomethingTriggered = rightTrigger > 0.1 || leftTrigger > 0.1;

        // Right Click
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

        if (yButton && !_yButtonPressed) InventoryUI.ToggleInventoryState();
        if (xButton && !_xButtonPressed)
        {
            var activeSelf = mapHandler.images.activeSelf;

            MapHandler.isMiniMap = !activeSelf;
            mapHandler.images.SetActive(!activeSelf);
        }

        _aButtonPressed = aButton;
        _bButtonPressed = bButton;
        _xButtonPressed = xButton;
        _yButtonPressed = yButton;

        // Weapon Swapping

        var weaponUpPressed = rightJoystick.y >= SwapWeaponThreshold;
        var weaponDownPressed = rightJoystick.y <= -SwapWeaponThreshold;

        if (weaponUpPressed && !_weaponUpPressed) SwapWeaponViaOffset(-1);
        if (weaponDownPressed && !_weaponDownPressed) SwapWeaponViaOffset(1);

        _weaponUpPressed = weaponUpPressed;
        _weaponDownPressed = weaponDownPressed;

        // Snap Turning

        var snapRightPressed = rightJoystick.x >= SwapWeaponThreshold;
        var snapLeftPressed = rightJoystick.x <= -SwapWeaponThreshold;

        if (snapRightPressed && !_snapRightPressed) SnapTurn(1);
        if (snapLeftPressed && !_snapLeftPressed) SnapTurn(-1);

        _snapRightPressed = snapRightPressed;
        _snapLeftPressed = snapLeftPressed;
    }

    private void SnapTurn(int direction)
    {
        Controllers.SnapTurnParent.transform.Rotate(Vector3.up, direction * 45);
        player.m_cameraMovement.transform.Rotate(Vector3.up, direction * 45);
    }

    /// <summary>
    ///     Picks up selected <see cref="Pickup" />.
    /// </summary>
    private void PickupInteract()
    {
        if (currentPickup && !currentPickup.hasBeenPickedUp && interactionHandler.pickupCor == null)
            interactionHandler.StartCoroutine(interactionHandler.StartPickup(currentPickup,
                currentPickup.weaponType == Pickup.WeaponType.Weapon &&
                !player.m_inventory.HasSpaceForWeapon(currentPickup)));
    }
}
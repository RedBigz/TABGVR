using JetBrains.Annotations;
using Landfall.Network;
using TABGVR.Player.Mundanities;
using TABGVR.Util;
using UnityEngine;
using UnityEngine.XR;

namespace TABGVR.Player;

public class VRControls : MonoBehaviour
{
    internal const float TriggerDeadZone = 0.7f;
    internal const float StopSprintingThreshold = 0.1f;
    internal const float SwapWeaponThreshold = 0.8f;

    internal bool AButtonPressed;
    internal bool BButtonPressed;
    internal bool XButtonPressed;
    internal bool YButtonPressed;

    internal bool LeftTriggered;
    internal bool RightTriggered;

    public static bool SomethingTriggered;
    public static bool GetSomethingTriggered() => SomethingTriggered;

    internal bool MenuButtonPressed;

    internal bool WeaponUpPressed;
    internal bool WeaponDownPressed;

    internal bool SnapRightPressed;
    internal bool SnapLeftPressed;
    
    internal bool RightClickPressed;

    [CanBeNull] private Pickup currentPickup;
    private HaxInput haxInput;
    private InputHandler inputHandler;
    private InteractionHandler interactionHandler;
    private MovementHandler movementHandler;
    private global::Player player;
    private Transform rotationX;
    private WeaponHandler weaponHandler;
    private Inventory inventory;

    private MenuTransitions menuTransitions;
    private MapHandler mapHandler;

    private void Start()
    {
        weaponHandler = GetComponent<WeaponHandler>();
        movementHandler = GetComponent<MovementHandler>();
        inputHandler = GetComponent<InputHandler>();
        interactionHandler = GetComponent<InteractionHandler>();
        player = GetComponent<global::Player>();
        inventory = player.m_inventory;
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
        if (weaponHandler.CurrentWeapon is Pickup.EquipSlots.None or Pickup.EquipSlots.ThrowableSlot)
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
        if (menuButtonPressed && !MenuButtonPressed)
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

        MenuButtonPressed = menuButtonPressed;

        // Sprinting
        if (leftClick && !inputHandler.isSpringting)
            inputHandler.isSpringting = true;

        inputHandler.isSpringting &= leftJoystick.magnitude > StopSprintingThreshold;

        if (!global::Player.usingInterface)
        {
            // Right Trigger
            if (rightTrigger > TriggerDeadZone)
            {
                if (!RightTriggered)
                {
                    if (Grenades.SelectedGrenade && interactionHandler.sinceThrow > 3f && !interactionHandler.isThrowing) interactionHandler.StartCoroutine(interactionHandler.Throwing());
                    else if (weaponHandler.rightWeapon) weaponHandler.PressAttack(true, false);
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
                if (!LeftTriggered)
                {
                    if (weaponHandler.leftWeapon) weaponHandler.PressAttack(false, false);
                    else PickupInteract();
                }
                else if (weaponHandler.leftWeapon)
                {
                    weaponHandler.HoldAttack(false, false);
                }
            }

            RightTriggered = rightTrigger > TriggerDeadZone;
            LeftTriggered = leftTrigger > TriggerDeadZone;
        }

        SomethingTriggered = rightTrigger > 0.1 || leftTrigger > 0.1;

        // Right Click
        if (rightClick && !RightClickPressed)
        {
            if (weaponHandler.CurrentWeapon == Pickup.EquipSlots.ThrowableSlot)
            {
                foreach (var unused in inventory.GrenadeSlots)
                {
                    inventory.CurrentlySelectedGrenadeSlot =
                        (inventory.CurrentlySelectedGrenadeSlot + 1) % inventory.GrenadeSlots.Length;

                    if (inventory.GetGrenadeInSelectedSlot().Pickup) break;
                }
            }
            
            weaponHandler.CurrentWeapon = Pickup.EquipSlots.ThrowableSlot;
        }
        
        RightClickPressed = rightClick;

        var movementVector = new Vector3(leftJoystick.x, 0.0f, leftJoystick.y);

        inputHandler.inputMovementDirection = rotationX.rotation * movementVector;

        if (aButton && !AButtonPressed) movementHandler.Jump();
        if (bButton && !BButtonPressed)
        {
            weaponHandler.rightWeapon?.gun.ReloadGun();
            weaponHandler.leftWeapon?.gun.ReloadGun();
        }

        if (yButton && !YButtonPressed) InventoryUI.ToggleInventoryState();
        if (xButton && !XButtonPressed)
        {
            var activeSelf = mapHandler.images.activeSelf;

            MapHandler.isMiniMap = !activeSelf;
            mapHandler.images.SetActive(!activeSelf);
        }

        AButtonPressed = aButton;
        BButtonPressed = bButton;
        XButtonPressed = xButton;
        YButtonPressed = yButton;

        // Weapon Swapping

        var weaponUpPressed = rightJoystick.y >= SwapWeaponThreshold;
        var weaponDownPressed = rightJoystick.y <= -SwapWeaponThreshold;

        if (weaponUpPressed && !WeaponUpPressed) SwapWeaponViaOffset(-1);
        if (weaponDownPressed && !WeaponDownPressed) SwapWeaponViaOffset(1);

        WeaponUpPressed = weaponUpPressed;
        WeaponDownPressed = weaponDownPressed;

        // Snap Turning

        var snapRightPressed = rightJoystick.x >= SwapWeaponThreshold;
        var snapLeftPressed = rightJoystick.x <= -SwapWeaponThreshold;

        if (snapRightPressed && !SnapRightPressed) SnapTurn(1);
        if (snapLeftPressed && !SnapLeftPressed) SnapTurn(-1);

        SnapRightPressed = snapRightPressed;
        SnapLeftPressed = snapLeftPressed;
    }

    private void SnapTurn(int direction)
    {
        UIPorter.UISnapTurnBase?.transform.Rotate(Vector3.up, direction * 45);
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
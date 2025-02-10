using JetBrains.Annotations;
using Landfall.Network;
using TABGVR.Player;
using TABGVR.Player.Mundanities;
using TABGVR.Util;
using UnityEngine;
using UnityEngine.XR;

namespace TABGVR.Input;

public class VRControls : MonoBehaviour
{
    public const float TriggerDeadZone = 0.7f;
    public const float StopSprintingThreshold = 0.1f;
    public const float SwapWeaponThreshold = 0.8f;

    public readonly ControlList ControlList = new();

    public static bool SomethingTriggered;
    public static bool GetSomethingTriggered() => SomethingTriggered; // For SwapWeaponPatch

    // Components

    private global::Player player;
    private InputHandler inputHandler;
    private InteractionHandler interactionHandler;
    private MovementHandler movementHandler;
    private WeaponHandler weaponHandler;
    [CanBeNull] private Pickup currentPickup;

    private Transform rotationX; // For movement direction

    // UI Components
    private Inventory inventory;
    private MapHandler mapHandler;
    private MenuTransitions menuTransitions;

    private void Start()
    {
        weaponHandler = GetComponent<WeaponHandler>();
        movementHandler = GetComponent<MovementHandler>();
        inputHandler = GetComponent<InputHandler>();
        interactionHandler = GetComponent<InteractionHandler>();
        player = GetComponent<global::Player>();
        inventory = player.m_inventory;
        rotationX = gameObject.GetComponentInChildren<RotationTarget>().transform.parent;

        interactionHandler.canPickUpAction = pickup => currentPickup = pickup;
        interactionHandler.canNotPickUpAction = () => currentPickup = null;

        inputHandler.enabled = false;
        inputHandler.cameraMovement.ADS = false;

        menuTransitions = InventoryUI.instance.gameObject.GetComponent<MenuTransitions>();
        mapHandler = InventoryUI.instance.gameObject.GetComponentInChildren<MapHandler>();
    }

    /// <summary>
    /// Swaps weapon based on offset.
    /// </summary>
    /// <param name="offset">Offset the swap weapons by (e.g. -1 means weapon above & 1 means weapon below)</param>
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

    /// <summary>
    /// Updates a <see cref="VRInputBinary"/>'s value to <paramref name="value"/>.
    /// </summary>
    /// <param name="input">An input</param>
    /// <param name="value">A boolean value</param>
    private void UpdateBinaryInputsInternal(VRInputBinary input, bool value)
    {
        // Plugin.Logger.LogInfo($"Updating binary inputs: {input} <- {value}");
        
        input.JustPressed = false;
        input.JustReleased = false;

        if (value)
            if (!input.HoldingDown) input.JustPressed = true;
            else input.JustPressed = false;
        else if (input.HoldingDown) input.JustReleased = true;
        else input.JustReleased = false;

        input.HoldingDown = value;
    }

    /// <summary>
    /// Updates a <see cref="VRInputBinary"/> to a <see cref="InputFeatureUsage{Boolean}"/> on an <see cref="InputDevice"/>.
    /// </summary>
    /// <param name="hand">The hand the feature will be searched for</param>
    /// <param name="input">The <see cref="VRInputBinary"/> to update</param>
    /// <param name="feature">The feature to search for</param>
    /// <returns>The feature's value (for other purposes)</returns>
    private bool UpdateBinaryInputs(InputDevice hand, VRInputBinary input, InputFeatureUsage<bool> feature)
    {
        hand.TryGetFeatureValue(feature, out var value);
        UpdateBinaryInputsInternal(input, value);
        return value;
    }

    /// <summary>
    /// Updates a <see cref="VRInputBinary"/> to a <see cref="InputFeatureUsage{Float}"/> on an <see cref="InputDevice"/>.
    /// </summary>
    /// <param name="hand">The hand the feature will be searched for</param>
    /// <param name="input">The <see cref="VRInputBinary"/> to update</param>
    /// <param name="feature">The feature to search for</param>
    /// <returns>The feature's value (for other purposes)</returns>
    private float UpdateBinaryInputs(InputDevice hand, VRInputBinary input, InputFeatureUsage<float> feature)
    {
        hand.TryGetFeatureValue(feature, out var value);
        UpdateBinaryInputsInternal(input, value > TriggerDeadZone);
        return value;
    }

    /// <summary>
    /// Updates a <see cref="VRInput1D"/>'s X value using a float value.
    /// </summary>
    /// <param name="input">The <see cref="VRInput1D"/> to set</param>
    /// <param name="value">The value to set <paramref name="input"/> to</param>
    private void Update1DInputs(VRInput1D input, float value)
    {
        input.LastFrameDigital = input.Digital;
        input.X = value;
    }

    private void Update()
    {
        // update interactor visibility
        if (global::Player.usingInterface != UIPorter.InteractorVisuals)
            UIPorter.InteractorVisuals = global::Player.usingInterface;

        if (movementHandler.death.dead) return;

        Controllers.RightHandXR.TryGetFeatureValue(CommonUsages.primary2DAxis, out var rightJoystick);
        Controllers.LeftHandXR.TryGetFeatureValue(CommonUsages.primary2DAxis, out var leftJoystick);

        var leftTrigger = UpdateBinaryInputs(Controllers.LeftHandXR, ControlList.LeftTriggerBinary, CommonUsages.trigger);
        var rightTrigger = UpdateBinaryInputs(Controllers.RightHandXR, ControlList.RightTriggerBinary, CommonUsages.trigger);

        UpdateBinaryInputs(Controllers.RightHandXR, ControlList.AButton, CommonUsages.primaryButton);
        UpdateBinaryInputs(Controllers.RightHandXR, ControlList.BButton, CommonUsages.secondaryButton);
        UpdateBinaryInputs(Controllers.LeftHandXR, ControlList.XButton, CommonUsages.primaryButton);
        UpdateBinaryInputs(Controllers.LeftHandXR, ControlList.YButton, CommonUsages.secondaryButton);

        UpdateBinaryInputs(Controllers.LeftHandXR, ControlList.LeftStick, CommonUsages.primary2DAxisClick);
        UpdateBinaryInputs(Controllers.RightHandXR, ControlList.RightStick, CommonUsages.primary2DAxisClick);

        UpdateBinaryInputs(Controllers.LeftHandXR, ControlList.MenuButton, CommonUsages.menuButton);

        UpdateBinaryInputsInternal(ControlList.WeaponSwitchUp, rightJoystick.y >= SwapWeaponThreshold);
        UpdateBinaryInputsInternal(ControlList.WeaponSwitchDown, rightJoystick.y <= -SwapWeaponThreshold);
        
        Update1DInputs(ControlList.Turn, rightJoystick.x);

        // Menu
        if (ControlList.MenuButton.JustPressed)
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

        // Sprinting
        if (ControlList.LeftStick.JustPressed && !inputHandler.isSpringting)
            inputHandler.isSpringting = true;

        inputHandler.isSpringting &= leftJoystick.magnitude > StopSprintingThreshold;

        if (!global::Player.usingInterface)
        {
            // Right Trigger
            if (ControlList.RightTriggerBinary.HoldingDown)
            {
                if (ControlList.RightTriggerBinary.JustPressed)
                {
                    if (Grenades.SelectedGrenade && interactionHandler.sinceThrow > 3f &&
                        !interactionHandler.isThrowing)
                        interactionHandler.StartCoroutine(interactionHandler.Throwing());
                    else if (weaponHandler.rightWeapon) weaponHandler.PressAttack(true, false);
                    else PickupInteract();
                }
                else if (weaponHandler.rightWeapon)
                {
                    weaponHandler.HoldAttack(true, false);
                }
            }

            // Left Trigger
            if (ControlList.LeftTriggerBinary.HoldingDown)
            {
                if (ControlList.LeftTriggerBinary.JustPressed)
                {
                    if (weaponHandler.leftWeapon) weaponHandler.PressAttack(false, false);
                    else PickupInteract();
                }
                else if (weaponHandler.leftWeapon)
                {
                    weaponHandler.HoldAttack(false, false);
                }
            }
        }

        SomethingTriggered = leftTrigger > 0.1 || rightTrigger > 0.1;

        // Right Click
        if (ControlList.RightStick.JustPressed)
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

        var movementVector = new Vector3(leftJoystick.x, 0.0f, leftJoystick.y);

        inputHandler.inputMovementDirection = rotationX.rotation * movementVector;

        if (ControlList.AButton.JustPressed) movementHandler.Jump();
        if (ControlList.BButton.JustPressed)
        {
            weaponHandler.rightWeapon?.gun.ReloadGun();
            weaponHandler.leftWeapon?.gun.ReloadGun();
        }

        if (ControlList.XButton.JustPressed)
        {
            var activeSelf = mapHandler.images.activeSelf;

            MapHandler.isMiniMap = !activeSelf;
            mapHandler.images.SetActive(!activeSelf);
        }

        if (ControlList.YButton.JustPressed) InventoryUI.ToggleInventoryState();

        // Weapon Swapping

        if (ControlList.WeaponSwitchUp.JustPressed) SwapWeaponViaOffset(-1);
        if (ControlList.WeaponSwitchDown.JustPressed) SwapWeaponViaOffset(1);

        // Turning
        if (Plugin.SnapTurnEnabled.Value)
        {
            if (ControlList.Turn.Digital != ControlList.Turn.LastFrameDigital) SnapTurn(45 * ControlList.Turn.Digital);
        }
        else
        {
            SnapTurn(Time.deltaTime * ControlList.Turn.Digital * 200);
        }
    }

    /// <summary>
    /// Turns player by an angle.
    /// </summary>
    /// <param name="direction">Angle in degrees</param>
    private void SnapTurn(float direction)
    {
        UIPorter.UISnapTurnBase?.transform.Rotate(Vector3.up, direction);
        Controllers.SnapTurnParent.transform.Rotate(Vector3.up, direction);
        player.m_cameraMovement.transform.Rotate(Vector3.up, direction);
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
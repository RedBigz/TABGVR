using JetBrains.Annotations;
using Landfall.TABG.UI;
using TABGVR.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace TABGVR.Player;

public class VRControls : MonoBehaviour
{
    internal const float TriggerDeadZone = 0.7f;
    internal const float StopSprintingThreshold = 0.1f;

    private bool _aButtonPressed;
    private bool _bButtonPressed;
    private bool _xButtonPressed;
    private bool _yButtonPressed;

    private bool _leftTriggered;
    private bool _rightTriggered;

    private bool _menuButtonPressed;

    [CanBeNull] private Pickup currentPickup;
    private HaxInput haxInput;
    private InputHandler inputHandler;
    private InteractionHandler interactionHandler;
    private MovementHandler movementHandler;
    private global::Player player;
    private Transform rotationX;
    private WeaponHandler weaponHandler;

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

        if (menuButtonPressed && !_menuButtonPressed)
        {
            var menuTransitions = InventoryUI.instance.gameObject.GetComponent<MenuTransitions>();

            switch (MenuState.CurrentMenuState)
            {
                case MenuState.TABGMenuState.Main:
                    menuTransitions.GoToEscape();
                    break;
                case MenuState.TABGMenuState.Escape:
                case MenuState.TABGMenuState.Options:
                    menuTransitions.GoToMain();
                    break;
            }
        }

        _menuButtonPressed = menuButtonPressed;

        if (leftClick && !inputHandler.isSpringting)
            inputHandler.isSpringting = true;

        inputHandler.isSpringting &= leftJoystick.magnitude > StopSprintingThreshold;

        if (rightTrigger > TriggerDeadZone)
        {
            if (!_rightTriggered)
            {
                if (UIPorter.UIRightHand.GetComponent<XRRayInteractor>()
                    .TryGetCurrentUIRaycastResult(out var uiRaycast))
                {
                    var handler = uiRaycast.gameObject.GetComponent<IPointerClickHandler>();
                    handler?.OnPointerClick(new PointerEventData(EventSystem.current));
                }
                else if (weaponHandler.rightWeapon) weaponHandler.PressAttack(true, false);
                else PickupInteract();
            }
            else if (weaponHandler.rightWeapon)
            {
                weaponHandler.HoldAttack(true, false);
            }
        }

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

        _aButtonPressed = aButton;
        _bButtonPressed = bButton;
        _xButtonPressed = xButton;
        _yButtonPressed = yButton;
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
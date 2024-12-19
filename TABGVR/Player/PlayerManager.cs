using UnityEngine;

namespace TABGVR.Player;

/// <summary>
/// A manager used for referencing <see cref="GameObject"/>s and components related to the player.
/// </summary>
public class PlayerManager
{
    public static PlayerManager LocalPlayer;
    public Camera CameraComponent;

    public GameObject CameraObject;
    public global::Player Player;

    public bool PlayerIsClient;
    public GameObject PlayerRoot;

    public PlayerManager(GameObject root)
    {
        PlayerRoot = root;

        Player = root.GetComponent<global::Player>();
        PlayerIsClient = Player == global::Player.localPlayer; // i think this works

        CameraObject = PlayerRoot.transform.FindChildRecursive("Main Camera").gameObject;
        CameraComponent = CameraObject.GetComponent<Camera>();

        if (PlayerIsClient) LocalPlayer = this;
    }

    /// <summary>
    ///     Creates PlayerManager from game camera.
    /// </summary>
    /// <param name="camera">Player's <see cref="Camera" /></param>
    /// <returns>
    ///     <see cref="PlayerManager" />
    /// </returns>
    public static PlayerManager FromCamera(Camera camera)
    {
        return new PlayerManager(camera.transform
            .parent // Pivot
            .parent // PositionShake
            .parent // RotationShake
            .parent // CameraPhysics
            .parent // CameraRotationY
            .parent // CameraRotationX
            .parent // CameraMovement
            .parent // Player
            .gameObject);
    }
}
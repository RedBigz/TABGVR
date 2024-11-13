using UnityEngine;

namespace TABGVR.Player;

public class PlayerManager
{
    public static PlayerManager LocalPlayer;
    public Camera cameraComponent;

    public GameObject cameraObject;
    public global::Player player;

    public bool playerIsClient;
    public GameObject playerRoot;

    public PlayerManager(GameObject root)
    {
        playerRoot = root;

        player = root.GetComponent<global::Player>();
        playerIsClient = player == global::Player.localPlayer; // i think this works

        cameraObject = playerRoot.transform.FindChildRecursive("Main Camera").gameObject;
        cameraComponent = cameraObject.GetComponent<Camera>();

        if (playerIsClient) LocalPlayer = this;
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
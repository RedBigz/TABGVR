using UnityEngine;

namespace TABGVR.Player;

public class PlayerManager
{
    public GameObject playerRoot;
    public global::Player player;

    public GameObject cameraObject;
    public Camera cameraComponent;

    public bool playerIsClient;
    
    public static PlayerManager LocalPlayer;

    public PlayerManager(GameObject root)
    {
        playerRoot = root;
        
        player = root.GetComponent<global::Player>();
        playerIsClient = player == global::Player.localPlayer; // i think this works
        
        cameraObject = playerRoot.transform.FindChildRecursive("Main Camera").gameObject;
        cameraComponent = cameraObject.GetComponent<Camera>();

        if (playerIsClient) LocalPlayer = this;
    }

    public static PlayerManager FromCamera(Camera camera) => new(camera.transform
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
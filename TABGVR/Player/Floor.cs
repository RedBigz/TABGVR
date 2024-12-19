using UnityEngine;
using UnityEngine.SceneManagement;

namespace TABGVR.Player;

/// <summary>
/// A component that sets the object's position to the relative floor of the player.
/// </summary>
public class Floor : MonoBehaviour
{
    private bool stationary = false;
    
    private void Start()
    {
        stationary = SceneManager.GetActiveScene().name == "MainMenu";
    }

    private void Update()
    {
        if (Camera.current is null) return;
        
        transform.localPosition = stationary ? Vector3.zero : Camera.current.transform.position - Controllers.Head.transform.position;
    }
}
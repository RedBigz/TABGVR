using UnityEngine;
using UnityEngine.SceneManagement;

namespace TABGVR.Player;

public class Floor : MonoBehaviour
{
    private bool _stationary = false;
    
    private void Start()
    {
        _stationary = SceneManager.GetActiveScene().name == "MainMenu";
    }

    private void Update()
    {
        if (Camera.current is null) return;
        
        transform.localPosition = _stationary ? Vector3.zero : Camera.current.transform.position - Controllers.Head.transform.position;
    }
}
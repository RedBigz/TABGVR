using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace TABGVR.Player;

public class Floor : MonoBehaviour
{
    public void Update()
    {
        transform.position = Camera.current.transform.position - Controllers.Head.transform.position;
    }
}
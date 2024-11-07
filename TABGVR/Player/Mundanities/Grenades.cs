using JetBrains.Annotations;

namespace TABGVR.Player.Mundanities;

public class Grenades
{
    [CanBeNull]
    public static Grenade SelectedGrenade =>
        PlayerManager.LocalPlayer.player.GetComponent<Holding>().rightHand.GetComponentInChildren<Grenade>();
}
using JetBrains.Annotations;

namespace TABGVR.Player.Mundanities;

public class Grenades
{
    /// <summary>
    ///     Selected Grenade.
    /// </summary>
    [CanBeNull]
    public static Grenade SelectedGrenade =>
        PlayerManager.LocalPlayer?.Player.m_holding.rightHand.GetComponentInChildren<Grenade>();
}
using HarmonyLib;
using Landfall.TABG.UI.MainMenu;
using TABGVR.PatchAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TABGVR.Patches.UI;

[HarmonyPatch(typeof(Toggle), nameof(Toggle.Start))]
[VRPatch, FlatscreenPatch]
public static class MainMenuDonateButtonPatch
{
    public static void Postfix(MainMenuManager __instance)
    {
        if (GameObject.Find("DonateButton")) return;

        var lowerLeftButtons = GameObject.Find("LowerLeftButtons").transform;

        var donateButton = Object.Instantiate(lowerLeftButtons.Find("Quit").gameObject, lowerLeftButtons);
        donateButton.transform.SetAsFirstSibling();

        donateButton.name = "DonateButton";

        donateButton.transform.Find("TextMeshPro Text").GetComponent<TextMeshProUGUI>().text = "DONATE";

        var donateIconTexture = AssetBundle.Bundle0.LoadAsset<Texture2D>("DonateIcon");
        
        var icon = donateButton.transform.Find("Icon (3)");
        icon.transform.rotation = Quaternion.identity;
        
        icon.GetComponent<Image>().sprite = Sprite.Create(donateIconTexture,
            new Rect(0, 0, donateIconTexture.width, donateIconTexture.height), new Vector2(0.5f, 0.5f));

        Object.Destroy(donateButton.GetComponent<QuitGame>()); // Stop game from closing lmao

        var button = donateButton.GetComponent<Button>();
        button.onClick.AddListener(() => Application.OpenURL("https://liberapay.com/redbigz"));
    }
}
using UnityEngine;
using UnityEngine.Assertions;

public static class HotspotUtils
{
    private static string HotspotGroupName = "Hotspot CanvasGroup";

    public static void InitCanvasElement(GameObject hotspotPrefab, out HotspotBehaviour uiHotspot)
    {
        var hotspotGroup = GameObject.Find(HotspotGroupName); // TODO
        if (hotspotGroup == null)
        {
            uiHotspot = null;
            return;
        }

        Assert.IsNotNull(hotspotPrefab, "Hotspot prefab not found");
        var uiElement = GameObject.Instantiate(hotspotPrefab);
        uiElement.transform.SetParent(hotspotGroup.transform);
        uiElement.transform.localScale = Vector3.one;
        //StartCoroutine(ChangeCanvasElementNameInNextFrame(uiElement, canvasElementPrefab));

        uiHotspot = uiElement.GetComponent<HotspotBehaviour>();
        Assert.IsNotNull(uiHotspot, $"Could not find {nameof(HotspotBehaviour)} on {uiElement.name}");
    }
}

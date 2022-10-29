using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

[ExecuteAlways]
public class HotspotWorldPositionAnchor : MonoBehaviour
{
    [SerializeField]
    private HotspotBehaviour uiHotspot;

    [SerializeField]
    private MeshRenderer dummyMeshRenderer;

    [Header("Debug Only")]
    [SerializeField]
    private GameObject canvasElementPrefab;

    private void Awake()
    {
        if (uiHotspot == null)
        {
            InitCanvasElement();
        }

        if (dummyMeshRenderer == null)
        {
            InitRemainingFields();
        }
    }

    private void Start()
    {
        dummyMeshRenderer.enabled = false;
    }

    private void Update()
    {
        if (transform.hasChanged && uiHotspot != null)
        {
            transform.hasChanged = false;
            uiHotspot.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        }
    }

    private void InitCanvasElement ()
    {
        var hotspotGroup = GameObject.Find("Hotspot CanvasGroup"); // TODO
        Assert.IsNotNull(hotspotGroup, "Hotspotgroup not found");

        Assert.IsNotNull(canvasElementPrefab, "Hotspot prefab not found");
        var uiElement = Instantiate(canvasElementPrefab);
        uiElement.transform.SetParent(hotspotGroup.transform);
        uiElement.transform.localScale = Vector3.one;
        StartCoroutine(ChangeCanvasElementNameInNextFrame(uiElement, canvasElementPrefab));
        
        uiHotspot = uiElement.GetComponent<HotspotBehaviour>();
        Assert.IsNotNull(uiHotspot, $"Could not find {nameof(HotspotBehaviour)} on {uiElement.name}");
        uiHotspot.SetOwner(this);
    }

    private IEnumerator ChangeCanvasElementNameInNextFrame(GameObject target, GameObject prefab)
    {
        yield return null;
        target.name = $"{prefab.name} -> [{gameObject.name}]";
    }

    private void InitRemainingFields()
    {
        var childName = "HotspotAnchor Visual Helper";
        var helper = gameObject.FindChildByName(childName);
        Assert.IsNotNull(helper, $"Could not find child {childName}");

        dummyMeshRenderer = helper.GetComponent<MeshRenderer>();
        Assert.IsNotNull(dummyMeshRenderer, $"Could not find MeshRenderer component on {dummyMeshRenderer.name}");
    }
}
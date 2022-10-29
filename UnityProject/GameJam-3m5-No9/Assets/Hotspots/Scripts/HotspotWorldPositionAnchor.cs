using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class HotspotWorldPositionAnchor : MonoBehaviour
{
    [SerializeField]
    private HotspotBehaviour uiHotspot;

    [SerializeField]
    private MeshRenderer dummyMeshRenderer;

    private void Awake()
    {
        if (uiHotspot == null)
        {
            SetupUiHotspotElement();
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
            Debug.Log(uiHotspot.transform.position);
        }
    }

    private void SetupUiHotspotElement ()
    {
        var hotspotGroup = GameObject.Find("Hotspot CanvasGroup"); // TODO
        Assert.IsNotNull(hotspotGroup, "Hotspotgroup not found");

#if UNITY_EDITOR
        var prefab = (GameObject) AssetDatabase.LoadAssetAtPath("Assets/Hotspots/Prefabs/Hotspot.prefab", typeof(GameObject));
#endif
        Assert.IsNotNull(prefab, "Hotspot prefab not found");

        var uiElement = Instantiate(prefab);
        uiElement.transform.SetParent(hotspotGroup.transform);
        StartCoroutine(UpdateName(uiElement, prefab));
        

        uiHotspot = uiElement.GetComponent<HotspotBehaviour>();
        Assert.IsNotNull(uiHotspot, $"Could not find {nameof(HotspotBehaviour)} on {uiElement.name}");
        uiHotspot.SetOwner(this);

        var childName = "HotspotAnchor Visual Helper";
        var helper = gameObject.FindChildByName(childName);
        Assert.IsNotNull(helper, $"Could not find child {childName}");

        dummyMeshRenderer = helper.GetComponent<MeshRenderer>();
        Assert.IsNotNull(dummyMeshRenderer, $"Could not find MeshRenderer component on {dummyMeshRenderer.name}");
    }

    private IEnumerator UpdateName(GameObject target, GameObject prefab)
    {
        yield return null;
        target.name = $"{prefab.name} of {gameObject.name}";
    }
}
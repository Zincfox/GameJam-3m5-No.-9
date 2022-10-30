using System.Collections;
using UnityEngine;

// combattants

[ExecuteAlways]
public class EntityBehaviour : MonoBehaviour
{
    [SerializeField]
    protected GameObject hotspotPrefab;

    [SerializeField]
    protected HotspotBehaviour hotspot;

    [SerializeField]
    protected MeshRenderer meshRenderer;

    [SerializeField]
    private AvatarIcons avatarIcons;

    [SerializeField]
    protected Icon icon;

    [SerializeField]
    private GameObjectConnection hotspotConnection;

    public MapMangler.Entities.Entity Entity { get; protected set; }

    protected virtual void Awake()
    {
        if (!hotspotConnection) {
            InitHotspot();
        }
    }



    protected virtual void Start()
    {
        HideHelperObject();
    }

    protected virtual void Update() 
    {
        if (transform.hasChanged && hotspot != null)
        {
            transform.hasChanged = false;
            hotspot.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        }
    }

    protected virtual void InitHotspot()
    {
        HotspotUtils.InitCanvasElement(hotspotPrefab, out hotspot);
        hotspot.SetAvatarIcon(avatarIcons.SelectSprite(icon));

        var script = GetComponent<GameObjectConnection>();
        hotspotConnection = hotspot.GetComponent<GameObjectConnection>();
        hotspotConnection.other = script;
        script.other = hotspotConnection;

        gameObject.BindComponent(out meshRenderer);
    }

    private void HideHelperObject()
    {
        if(meshRenderer) meshRenderer.enabled = false;
    }

    private void OnValidate()
    {
        if (hotspot != null) hotspot.SetAvatarIcon(avatarIcons.SelectSprite(icon));
    }
}

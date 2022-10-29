using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

public class HotspotBehaviour : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;

#if UNITY_EDITOR
    [Header("Debug Only")]

    [SerializeField]
    private HotspotWorldPositionAnchor owner;
#endif

    private void Reset()
    {
        text = GetComponentInChildren<TMP_Text>();
    }

#if UNITY_EDITOR
    public void SetOwner(HotspotWorldPositionAnchor anchor)
    {
        owner = anchor;
    }
#endif
}

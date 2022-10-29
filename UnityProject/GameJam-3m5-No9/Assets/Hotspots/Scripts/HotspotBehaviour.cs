using UnityEngine;
using TMPro;

public class HotspotBehaviour : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;

    private void Reset()
    {
        text = GetComponentInChildren<TMP_Text>();
    }

    private void Came(Transform target)
    {
        //Vector3 screenPos = cam.WorldToScreenPoint(target.position);
    }
}

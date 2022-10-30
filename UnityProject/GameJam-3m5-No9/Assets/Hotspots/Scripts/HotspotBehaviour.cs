using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteAlways]
public class HotspotBehaviour : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;

    [SerializeField]
    private Image avatarIcon;

    public void SetAvatarIcon(Sprite sprite)
    {
        if (sprite != null) avatarIcon.sprite = sprite;
    }
}

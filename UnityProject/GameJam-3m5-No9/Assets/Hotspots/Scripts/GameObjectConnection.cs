using UnityEngine;

[ExecuteAlways]
public class GameObjectConnection : MonoBehaviour
{
    public GameObjectConnection other;

    private void OnDestroy()
    {
        if(other != null) {
            other.other = null;
            DestroyImmediate(other.gameObject);
        }
    }
}

using UnityEngine;
using System;

public class LevelClickHandler : MonoBehaviour
{
    private static float Max_Distance = 100f;

    public event Action AreaClicked;

    private int layerMask;

    public Transform LastClicked { get; set; }

    private void Start(){
        layerMask = LayerMask.GetMask("ClickableArea");
    }

    void Update()
    {
        //Check for mouse click 
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit raycastHit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, Max_Distance, layerMask))
            {
                if (raycastHit.transform != null)
                {
                    LastClicked = raycastHit.transform;
                    OnAreaClicked();
                }
            }
        }
    }

    private void OnAreaClicked()
    {
        AreaClicked?.Invoke();
    }
}

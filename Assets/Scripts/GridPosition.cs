using UnityEngine;

public class GridPosition : MonoBehaviour
{
    [SerializeField] private int x;
    [SerializeField] private int y;

    [SerializeField] private GameManager gameManager;
    private void OnMouseDown()
    {
        Debug.Log("Grid position clicked: " + gameObject.name);

        Debug.Log($"Grid Position clicked is : {x} {y}");

        //gameManager.OnBeforeTransformParentChanged(x, y);

        GameManager.instance.OnBeforeTransformParentChangedRpc(x, y , GameManager.instance.GetLocalPlayerType());
    }
}

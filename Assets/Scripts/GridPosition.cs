using UnityEngine;

public class GridPosition : MonoBehaviour
{
    [SerializeField] private int x;
    [SerializeField] private int y;
    private void OnMouseDown()
    {
        Debug.Log("Grid position clicked: " + gameObject.name);

        Debug.Log($"Grid Position clicked is : {x} {y}");
    }
}

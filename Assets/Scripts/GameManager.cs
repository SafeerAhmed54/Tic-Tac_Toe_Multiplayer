using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance { get;  private set; }

    public event EventHandler <OnClickedOnGridPositionEventArgs> OnClickedOnGridPosition;

    public class OnClickedOnGridPositionEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void OnBeforeTransformParentChanged(int x , int y)
    {
        Debug.Log("ClickedOnGridPosition" + x + ", " +  y);
        OnClickedOnGridPosition?.Invoke(this, new OnClickedOnGridPositionEventArgs
        {
            x = x,
            y = y
        });
    }
}


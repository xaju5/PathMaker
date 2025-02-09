using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private bool enableDebugShowPathfindig = true;

    public bool GetEnableDebugShowPathfindig(){
        return enableDebugShowPathfindig;
    }

    void Awake(){
        Instance = this;
    }
}

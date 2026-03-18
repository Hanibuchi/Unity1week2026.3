using UnityEngine;
using UnityEngine.Tilemaps;

public class GroundManager : MonoBehaviour
{
    public static GroundManager Instance { get; private set; }

    [SerializeField] private Tilemap groundTilemap;

    public Tilemap GroundTilemap => groundTilemap;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
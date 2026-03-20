using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Code.MainGame.Map
{
    public class MapPainter : MonoBehaviour
    {
        [SerializeField] private Tilemap groundTilemap;
        [SerializeField] private Tilemap obstacleTilemap;
        [SerializeField] private Tilemap spawnZoneTilemap;
        
        public Tilemap GroundTilemap => groundTilemap;
        public Tilemap ObstacleTilemap => obstacleTilemap;
        public Tilemap SpawnZoneTilemap => spawnZoneTilemap;
    }
}

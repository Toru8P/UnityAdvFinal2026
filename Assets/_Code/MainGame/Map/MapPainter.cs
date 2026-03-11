using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Code.MainGame.Map
{
    public class MapPainter : MonoBehaviour
    {
        [SerializeField] private Tilemap groundTilemap;
        [SerializeField] private Tilemap obstacleTilemap;

        public Tilemap GroundTilemap => groundTilemap;
        public Tilemap ObstacleTilemap => obstacleTilemap;
    }
}

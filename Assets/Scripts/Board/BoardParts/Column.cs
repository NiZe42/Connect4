using System.Collections.Generic;
using UnityEngine;

public class Column : MonoBehaviour {
    public List<Tile> tiles = new List<Tile>();

    public void GetTilesFromChildren() {
        tiles = new List<Tile>();
        for (int i = 0; i < transform.childCount; i++) {
            tiles.Add(transform.GetChild(i).gameObject.GetComponent<Tile>());
        }

    }

    public Tile GetTileByIndex(int index) {
        return tiles[index];
    }

    public int GetIndexByTile(Tile targetTile) {
        return tiles.IndexOf(targetTile);
    }

    public void Print() {
        Debug.Log($"{name}:\n");
        foreach (Tile tile in tiles) {
            Debug.Log($"{tile.transform.localPosition}, \n");
        }
    }



}

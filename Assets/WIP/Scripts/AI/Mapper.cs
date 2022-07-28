using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Creates "maps" over AI options, as in what areas opponents can attack,
// what is attackable (yes, that is a word!) and keeps track at who leads:

public class Mapper : MonoBehaviour
{
    GridControl gc;

    GameManager gm;
    GameObject gameManager;
    // Start is called before the first frame update
    void Awake()
    {
        gc = GridControl.GridControlSingleTon;

        gameManager = GameObject.FindGameObjectWithTag("GameController");
        gm = gameManager.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Vector2Int> GetPossibleAttacks(string team)
    {
        List<Vector2Int> temp = new List<Vector2Int>();
        foreach (KeyValuePair<Vector3Int, GameObject> pair in gc.pieceLocations)
        {
            if (!pair.Value.tag.Equals(team))
            {
                Vector3Int v3 = pair.Key;
                Vector2Int v2 = gc.GetCoordinates(new Vector2Int(v3.x, v3.z));
                AddPossAttackLocations(v2.x, v2.y, temp); 
            }
        }
        return temp;
    }
    
    public List<Vector2Int> AddPossAttackLocations(int row, int col, List<Vector2Int> addToList)
    {
        int maxrow = row + 2;
        int minrow = row - 2;
        int maxcol = col + 2;
        int mincol = col - 2;

        for (int i = minrow; i <= maxrow; i++)
        {
            for (int j = mincol; j <= maxcol; j++)
            {
                if (i < 17 && j < 17 && i >= 0 && j >= 0)
                {
                    Vector2Int v2 = new Vector2Int((int)gc.grids2D[i, j].transform.position.x, (int)gc.grids2D[i, j].transform.position.z);
                    
                    foreach (GridControl.GridSystem g in GridControl.GridClassList)
                    {
                        if ((g.position == v2 && !g.teamName.Equals(tag)) || (g.position == v2 && !g.teamName.Equals(tag) &&
                            (g.settlement == GridControl.Settlement.City || g.settlement == GridControl.Settlement.Village)))
                        {
                            if (!addToList.Contains(v2))
                            {
                                // sv Instantiate(gm.archer, gc.grids2D[i, j].transform.position, Quaternion.identity);
                                addToList.Add(v2);
                            }
                        }
                    }
                }
            }
        }
        return addToList;
    }

    public List<Vector2Int> GetPossibleTargets(string team)
    {
        List<Vector2Int> temp = new List<Vector2Int>();
        foreach (KeyValuePair<Vector3Int, GameObject> pair in gc.pieceLocations)
        {
            if (pair.Value.tag.Equals(team))
            {
                //Debug.Log("Hipp" + pair.Key);
                Vector3Int v3 = pair.Key;
                temp.Add(new Vector2Int(v3.x, v3.z));
            }
        }
        return temp;
    }
}

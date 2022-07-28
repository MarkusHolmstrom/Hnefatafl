using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourManager : MonoBehaviour
{
    public Material oden, frigg, tor, frej, freja, loke, balder, heimdall, tyr, idun;
    const int MAX = 17;

    GridControl gridC;

    // Start is called before the first frame update
    void Awake()
    {
        gridC = GridControl.GridControlSingleTon;
    }

    int nr = 0;
    // Update is called once per frame
    void Update()
    {
        //if (nr < 0 || nr >= MAX)
        //{
        //    nr = 0;
        //}
    }

    int radius = 0;

    public void ColourCity(Vector2 coordinates, bool capital, Material god, bool attacked) //v2: 0 -> 16
    {
        if (capital)
        {
            radius = 2;
        }
        else
        {
            radius = 1;
        }
        for (int i = (int)coordinates.x - radius; i <= (int)coordinates.x + radius; i++)
        {
            for (int j = (int)coordinates.y - radius; j <= (int)coordinates.y + radius; j++)
            {
                if (i < MAX && j < MAX && i >= 0 && j >= 0) 
                {
                    bool noVillageHere = false;
                    if ((!gridC.CheckIfVillageAtPosition(new Vector2Int(i, j)) && capital) || coordinates == new Vector2Int(i, j))
                    {
                        noVillageHere = true;
                    }
                    if((attacked && noVillageHere) || !capital || !attacked)
                    {
                        gridC.grids2D[i, j].GetComponent<Renderer>().material = god;
                        Vector3 pos = gridC.grids2D[i, j].transform.position;
                        gridC.ChangeMaterialGrid(Vector3Int.FloorToInt(pos), god);
                    }
                    
                }
            }
        }
    }



    public int CorrectNumber(int nr)
    {
        return nr;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public bool clearBoard = false;
    readonly string attacks = "Attacks";

    public bool charge = false;

    Vector3 startDamLocation;
    GridControl gridControl;

    Village village;
    Capital capital;

    ColourManager colourManager;
    GameManager gameManager;
    // Start is called before the first frame update
    void Awake()
    {
        gridControl = GridControl.GridControlSingleTon;
        startDamLocation = gridControl.damagePrefab.transform.position;
        colourManager = GetComponent<ColourManager>();
        movement = GetComponent<Movement>();
        gameManager = GetComponent<GameManager>();
    }

    float timer = 0;
    readonly float limit = 1.1f;

    // Update is called once per frame
    void Update()
    {
        if (clearBoard)
        {
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(attacks);
            foreach (GameObject item in gameObjects)
            {
                Destroy(item);
            }
            clearBoard = false;
        }

        if (damageStarted && timer <= limit)
        {
            timer += Time.deltaTime * 2;
        }
        else if (timer > limit)
        {
            gridControl.damagePrefab.transform.position = startDamLocation;
            timer = 0;
            damageStarted = false;
        }
    }
    string victimName;
    float dam;
    bool marauder = false;
    bool godAttack = false;
    GameObject soldier;

    public List<Vector2Int> PossAttackLocations(GameObject goPiece, int row, int col, string tag, GridControl.Piece piece)
    {
        soldier = goPiece;
        dam = goPiece.GetComponent<PieceMover>().damage;
        int maxrow, minrow, maxcol, mincol;
        List<Vector2Int> possAttacks = new List<Vector2Int>();
        if (piece.NamePiece == GridControl.NamePiece.God)
        {
            godAttack = true;
        }
        else
        {
            godAttack = false;
        }
        if (piece.NamePiece == GridControl.NamePiece.Marauder)
        {
            marauder = true;
            maxrow = row + 1;
            minrow = row - 1;
            maxcol = col + 1;
            mincol = col - 1;
        }
        else
        {
            marauder = false;
            maxrow = row + 2;
            minrow = row - 2;
            maxcol = col + 2;
            mincol = col - 2;
        }

        for (int i = minrow; i <= maxrow; i++)
        {
            for (int j = mincol; j <= maxcol; j++)
            {
                if (i < 17 && j < 17 && i >= 0 && j >= 0)
                {
                    Vector2Int v2 = new Vector2Int((int)gridControl.grids2D[i, j].transform.position.x,
                                            (int)gridControl.grids2D[i, j].transform.position.z);

                    foreach (GridControl.GridSystem g in GridControl.GridClassList)
                    {
                        if ((g.position == v2 && g.place == GridControl.Place.Occupied && !g.teamName.Equals(tag)) ||
                            (g.position == v2  && !g.teamName.Equals(tag) && !g.teamName.Equals("") &&
                            (g.settlement == GridControl.Settlement.City || g.settlement == GridControl.Settlement.Village)))
                        {
                            victimName = g.teamName;
                            possAttacks.Add(Vector2Int.FloorToInt(v2));
                        }
                    }
                }
            }
        }

        return possAttacks;
    }

    PieceMover pm;
    bool damageStarted = false;
    int attackDecider;
    
    public void AttackOpponent(Vector3Int v3)
    {
        gridControl.ChangeRemainingMovesText(1);
        gridControl.haloKeeper.transform.position = gridControl.startHalo;
        clearBoard = true;
        soldier.GetComponent<PieceMover>().doneForTurn = true;
        if (marauder)
        {
            soldier.GetComponent<PieceMover>().health -= 1;
        }
        attackDecider = 0;
        GetCapitalVillageInfo(); //ersätt denna i startmenyn senare på nå sätt!! elr?
        GameObject temp; // Just a null filler
        foreach (KeyValuePair<Vector3Int, GameObject> item in gridControl.pieceLocations)
        {
            if (item.Key.x == v3.x && item.Key.z == v3.z)
            {
                gridControl.damagePrefab.transform.position = v3;
                damageStarted = true;
                temp = item.Value;
                if (temp.tag != "Village" && temp.tag != "Capital")
                {
                    attackDecider = 1;
                    //
                    pm = temp.GetComponent<PieceMover>();
                    pm.health -= dam;
                    if (pm.health <= 0 && pm.god)
                    {
                        RemoveDestroyedOpp(temp.tag);
                        ChangeCapitalsVillages(soldier.tag, temp.tag);
                    }
                }
                else if (temp.tag == "Capital" && attackDecider == 0)
                {
                    attackDecider = 2;
                    //Debug.LogWarning(temp.tag + "barabahhiliddy capuitala");
                    capital = temp.GetComponent<Capital>();
                    capital.health -= dam;
                    if ((godAttack || marauder) && capital.health <= 0)
                    {
                        capital.SetVariables(capital.gridPosition, soldier.tag);
                    }
                }
                else if (temp.tag == "Village" && attackDecider == 0)
                {
                    attackDecider = 3;
                    //Debug.LogWarning(temp.tag + "karabahhiliddy villagia");
                    village = temp.GetComponent<Village>();
                    village.health -= dam;
                    if ((godAttack || marauder) && village.health <= 0)
                    {
                        village.SetVariables(village.gridPosition, soldier.tag);
                    }
                }
            }
        }
        /*if (attackDecider == 1 && (godAttack || marauder) && pm != null && pm.health <= 0) ska mara flytta på sig efter dödsattack?
        {
            MarauderAdvance(soldier, v3, "Opp"); //Nothing special happens because of this default string
        }
        else */
        if (attackDecider == 2 && (godAttack || marauder) && capital.health <= 0 && capital != null)
        {
            MarauderAdvance(soldier, v3 /*temp.transform.position*/, "Capital");
            gridControl.ChangeGridName(v3, gameManager.GetCurrentTeam()); // Change grid name so the city can be attacked again?
            capital.health = 2;
        }
        else if (attackDecider == 3 && (godAttack || marauder) && village != null && village.health <= 0)
        {
            MarauderAdvance(soldier, v3, "Village");
            gridControl.ChangeGridName(v3, gameManager.GetCurrentTeam());
            village.health = 2;
        }
    }

    public void GetCapitalVillageInfo()
    {
        GameObject g = GameObject.FindGameObjectWithTag("Capital");
        capital = g.GetComponent<Capital>();

        GameObject v = GameObject.FindGameObjectWithTag("Village");
        if (v != null)
        {
            village = v.GetComponent<Village>();
        }
    }

    Vector2Int v2;
    Movement movement;

    void MarauderAdvance(GameObject mara, Vector3 newPos, string attackedName)
    {
        movement.moving = true;
        Material team = gridControl.FindTeamMaterial(mara.tag);
        foreach (KeyValuePair<Vector3Int, Vector2Int> item in gridControl.posToColRow)
        {
            if (item.Key.x == (int)newPos.x && item.Key.z == (int)newPos.z)
            {
                v2 = item.Value;
            }
        }
        if (attackedName.Equals("Village"))
        {
            colourManager.ColourCity(v2, false, team, true);
        }
        else if (attackedName.Equals("Capital"))
        {
            colourManager.ColourCity(v2, true, team, true);
        }

        movement.ChangeMoveablePiece(mara);
        movement.MoveToPosition(Vector3Int.FloorToInt(mara.transform.position), Vector3Int.FloorToInt(newPos));
        //Debug.Log(mara.tag + ":  till " + newPos + " från: " + mara.transform.position);
        //mara.transform.position = newPos;
        //gridControl.ChangePositionSoldier(mara, GridControl.Place.Occupied, mara.tag);
        //gridControl.MakeGridEmpty(Vector3Int.FloorToInt(mara.transform.position), GridControl.Place.Empty);
    }

    void RemoveDestroyedOpp(string oppName)
    {
        GameObject[] opps = GameObject.FindGameObjectsWithTag(oppName);
        foreach (GameObject opp in opps)
        {
            PieceMover pm = opp.GetComponent<PieceMover>();
            pm.health = 0;
            UniversalSettings.RemovePlayer(oppName);
        }
    }

    void ChangeCapitalsVillages(string teamName, string oppName)
    {
        foreach (GameObject village in gameManager.villages)
        {
            if (village.GetComponent<Village>().tagTeam == oppName)
            {
                Village vill = village.GetComponent<Village>();
                vill.SetVariables(vill.gridPosition, soldier.tag);
                colourManager.ColourCity(vill.gridPosition, false, gridControl.FindTeamMaterial(teamName), true);
            }
        }
        foreach (GameObject capital in gameManager.capitals)
        {
            if (capital.GetComponent<Capital>().tagTeam == oppName)
            {
                Capital cap = capital.GetComponent<Capital>();
                cap.SetVariables(cap.gridPosition, soldier.tag);
                colourManager.ColourCity(cap.gridPosition, true, gridControl.FindTeamMaterial(teamName), true);
            }
        }
    }
}

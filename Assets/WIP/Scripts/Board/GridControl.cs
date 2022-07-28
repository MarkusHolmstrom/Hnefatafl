using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridControl : MonoBehaviour
{
    public static GridControl _gridControl;
    public static GridControl GridControlSingleTon
    {
        get
        {
            if (_gridControl == null)
            {
                _gridControl = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridControl>();
                if (_gridControl == null)
                {
                    Debug.LogWarning("ada is null + gridcontrol");
                    GameObject b = new GameObject("Board");
                    _gridControl = b.AddComponent<GridControl>();
                }
            }
            return _gridControl;
        }
    }

    GameManager gameManager;
    ColourManager colourManager;

    public GameObject movePrefab, attackPrefab, damagePrefab;
    public bool destroyInPieces = true;

    public Text infoText;
    public GameObject infoImage;

    readonly string grid = "Grid";
    public GameObject[] grids;
    public GameObject[] grid0, grid1, grid2, grid3, grid4, grid5, grid6, grid7, grid8,
        grid9, grid10, grid11, grid12, grid13, grid14, grid15, grid16;
    public GameObject[,] grids2D = new GameObject[MAXROW, MAXCOL]; 

    public Vector2[,] board = new Vector2[MAXROW,MAXCOL];
    const int MAXROW = 17;
    const int MAXCOL = 17;

    public enum NamePiece {God, Marauder, Archer, Horsemen};

    public class Piece
    {
        public NamePiece NamePiece; public string team; public Vector2Int coordinates;
    }

    public enum Place {Empty, Occupied, Current};
    public enum Settlement {None, City, Village};

    public class GridSystem
    {
        public GameObject grid; public Vector2Int position; public Vector2Int coordinates; public Place place; 
        public Settlement settlement; public Material godMaterial; public string teamName;
    }

    public static List<GridSystem> GridClassList = new List<GridSystem>();

    Movement movement;
    Attack attack;
    public Dictionary<Vector3Int, Vector2Int> posToColRow = new Dictionary<Vector3Int, Vector2Int>();

    public Dictionary<Vector3Int, GameObject> pieceLocations = new Dictionary<Vector3Int, GameObject>();


    public GameObject haloKeeper;
    public Behaviour halo;
    public Vector3 startHalo;

    public GameObject turnPointer;
    public Vector3 turnStartHalo;


    // Start is called before the first frame update
    void Awake()
    {
        infoImage.SetActive(false);
        grid0 = GameObject.FindGameObjectsWithTag(grid);
        grid1 = GameObject.FindGameObjectsWithTag("Grid1");
        grid2 = GameObject.FindGameObjectsWithTag("Grid2");
        grid3 = GameObject.FindGameObjectsWithTag("Grid3");
        grid4 = GameObject.FindGameObjectsWithTag("Grid4");
        grid5 = GameObject.FindGameObjectsWithTag("Grid5");
        grid6 = GameObject.FindGameObjectsWithTag("Grid6");
        grid7 = GameObject.FindGameObjectsWithTag("Grid7");
        grid8 = GameObject.FindGameObjectsWithTag("Grid8");
        grid9 = GameObject.FindGameObjectsWithTag("Grid9");
        grid10 = GameObject.FindGameObjectsWithTag("Grid10");
        grid11 = GameObject.FindGameObjectsWithTag("Grid11");
        grid12 = GameObject.FindGameObjectsWithTag("Grid12");
        grid13 = GameObject.FindGameObjectsWithTag("Grid13");
        grid14 = GameObject.FindGameObjectsWithTag("Grid14");
        grid15 = GameObject.FindGameObjectsWithTag("Grid15");
        grid16 = GameObject.FindGameObjectsWithTag("Grid16");

        grids2D = GetGridArray(MAXROW);
        board = CreateBoard(MAXROW, MAXCOL, grids2D);
        foreach (GameObject go in grids2D)
        {
            GridSystem g = new GridSystem // Create default values for the in-script boards grids:
            {
                grid = go,
                position = new Vector2Int((int)go.transform.position.x, (int)go.transform.position.z),
                coordinates = GetCoordinates(go),
                godMaterial = defaultMat,
                place = Place.Empty,
                settlement = Settlement.None,
                teamName = ""
            };
            GridClassList.Add(g);
        }
        movement = GetComponent<Movement>();
        attack = GetComponent<Attack>();
        gameManager = GetComponent<GameManager>();
        colourManager = GetComponent<ColourManager>();
        villageScreen.SetActive(false);

        haloKeeper = GameObject.FindGameObjectWithTag("HaloKeeper");
        startHalo = haloKeeper.transform.position;
        halo = (Behaviour)haloKeeper.GetComponent("Halo");
        halo.enabled = false;
        textDoneTurnInfo.SetActive(false);
        turnStartHalo = turnPointer.transform.position;
    }

    public Vector2Int GetCoordinates(GameObject gop)
    {
        Vector2Int v;
        foreach (KeyValuePair<Vector3Int, Vector2Int> item in posToColRow)
        {
            if (item.Key.x == (int)gop.transform.position.x && item.Key.z == (int)gop.transform.position.z)
            {
                v = item.Value;
                return v;
            }
        }
        return Vector2Int.zero;
    }
    public Vector2Int GetCoordinates(Vector2Int v2)
    {
        Vector2Int v;
        foreach (KeyValuePair<Vector3Int, Vector2Int> item in posToColRow)
        {
            if (item.Key.x == v2.x && item.Key.z == v2.y)
            {
                v = item.Value;
                return v;
            }
        }
        return Vector2Int.zero;
    }

    public Vector3 GetPosition(Vector2Int coordinates)
    {
        Vector3 v;
        foreach (KeyValuePair<Vector3Int, Vector2Int> item in posToColRow)
        {
            if (item.Value == coordinates)
            {
                v = new Vector3(item.Key.x, GameManager.pieceHeightY, item.Key.z);
                return v;
            }
        }
        Debug.LogError("Wröng: felaktiga coordinatersigh: " + coordinates);
        return Vector3Int.zero;
    }

    int counter;

    GameObject[,] GetGridArray(int width)
    {
        GameObject[,] tempgrid = new GameObject[MAXROW, MAXCOL];
        for (int i = 0; i < width; i++)
        {
            counter = 0;
            tempgrid[i, counter] = grid0[i];
            posToColRow.Add(Vector3Int.FloorToInt(grid0[i].transform.position), new Vector2Int(i, counter));
            counter++;
            tempgrid[i, counter] = grid1[i];
            posToColRow.Add(Vector3Int.FloorToInt(grid1[i].transform.position), new Vector2Int(i, counter));
            counter++;
            tempgrid[i, counter] = grid2[i];
            posToColRow.Add(Vector3Int.FloorToInt(grid2[i].transform.position), new Vector2Int(i, counter));
            counter++;
            tempgrid[i, counter] = grid3[i];
            posToColRow.Add(Vector3Int.FloorToInt(grid3[i].transform.position), new Vector2Int(i, counter));
            counter++;
            tempgrid[i, counter] = grid4[i];
            posToColRow.Add(Vector3Int.FloorToInt(grid4[i].transform.position), new Vector2Int(i, counter));
            counter++;
            tempgrid[i, counter] = grid5[i];
            posToColRow.Add(Vector3Int.FloorToInt(grid5[i].transform.position), new Vector2Int(i, counter));
            counter++;
            tempgrid[i, counter] = grid6[i];
            posToColRow.Add(Vector3Int.FloorToInt(grid6[i].transform.position), new Vector2Int(i, counter));
            counter++;
            tempgrid[i, counter] = grid7[i];
            posToColRow.Add(Vector3Int.FloorToInt(grid7[i].transform.position), new Vector2Int(i, counter));
            counter++;
            tempgrid[i, counter] = grid8[i];
            posToColRow.Add(Vector3Int.FloorToInt(grid8[i].transform.position), new Vector2Int(i, counter));
            counter++;
            tempgrid[i, counter] = grid9[i];
            posToColRow.Add(Vector3Int.FloorToInt(grid9[i].transform.position), new Vector2Int(i, counter));
            counter++;
            tempgrid[i, counter] = grid10[i];
            posToColRow.Add(Vector3Int.FloorToInt(grid10[i].transform.position), new Vector2Int(i, counter));
            counter++;
            tempgrid[i, counter] = grid11[i];
            posToColRow.Add(Vector3Int.FloorToInt(grid11[i].transform.position), new Vector2Int(i, counter));
            counter++;
            tempgrid[i, counter] = grid12[i];
            posToColRow.Add(Vector3Int.FloorToInt(grid12[i].transform.position), new Vector2Int(i, counter));
            counter++;
            tempgrid[i, counter] = grid13[i];
            posToColRow.Add(Vector3Int.FloorToInt(grid13[i].transform.position), new Vector2Int(i, counter));
            counter++;
            tempgrid[i, counter] = grid14[i];
            posToColRow.Add(Vector3Int.FloorToInt(grid14[i].transform.position), new Vector2Int(i, counter));
            counter++;
            tempgrid[i, counter] = grid15[i];
            posToColRow.Add(Vector3Int.FloorToInt(grid15[i].transform.position), new Vector2Int(i, counter));
            counter++;
            tempgrid[i, counter] = grid16[i];
            posToColRow.Add(Vector3Int.FloorToInt(grid16[i].transform.position), new Vector2Int(i, counter));
        }
        return tempgrid;
    }

    Vector2[,] CreateBoard(int width, int depth, GameObject[,] gameObjects)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                board[i, j] = new Vector2(gameObjects[i, j].transform.position.x, gameObjects[i, j].transform.position.z);
            }
        }
        return board;
    }
    public bool turnDone = false;
    public GameObject textDoneTurnInfo;

    public bool moveAct = false, attackAct = false;
    public bool showMove = false, showAtt = false;
    public bool doneMove = false;

    public GameObject villageScreen;
    bool possVillage = false;
    public bool acceptedVillage = false;
    Vector2Int villCoord;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && turnDone) //
        {
            turnDone = false;
        }
        if (movesRemaining <= 0)
        {
            textDoneTurnInfo.SetActive(true);
            turnDone = true;
        }
        if (!turnDone)
        {
            if (Input.GetKeyDown(KeyCode.M) && !showMove && !doneMove)
            {
                moveAct = true;
                attackAct = false;
                attack.clearBoard = true;
                foreach (Vector2 m in moves)
                {
                    Instantiate(movePrefab, new Vector3(m.x, 0, m.y), Quaternion.identity);
                }
                showMove = true;
                showAtt = false;
            }
            else if (Input.GetKeyDown(KeyCode.M) && showMove)
            {
                //attackAct = false;
                movement.clearBoard = true;
                showMove = false;
            }
            if (Input.GetKeyDown(KeyCode.N) && !showAtt)
            {
                moveAct = false;
                attackAct = true;
                movement.clearBoard = true;
                foreach (Vector2 a in attacks)
                {
                    Instantiate(attackPrefab, new Vector3(a.x, 0, a.y), Quaternion.identity);
                }
                showAtt = true;
                showMove = false;
            }
            else if (Input.GetKeyDown(KeyCode.N) && showAtt)
            {
                //attackAct = false;
                attack.clearBoard = true;
                showAtt = false;
            }
            if (Input.GetKeyDown(KeyCode.V) && villagePossibility && tempTag != null)
            {
                villageScreen.SetActive(true);
                possVillage = true; //Debug.Log("village possible... " + possVillage);
            }
            if (possVillage && acceptedVillage) // undvik ny klick vid detta läge...
            {
                possHeal = false;
                moves.Clear();
                attacks.Clear();
                villagePossibility = false;
                villageScreen.SetActive(false);
                gameManager.BuildSettlement(villCoord.x, villCoord.y, Settlement.Village, false, curMaterial, tempPieces, tempTag);
                possVillage = false;
                acceptedVillage = false;
                PieceMover pm = curPiece.GetComponent<PieceMover>();
                pm.doneForTurn = true;
                pm.multiPlierHealth = 1.25f;
                ChangeRemainingMovesText(1);
                halo.enabled = false;
            }

            if (Input.GetKeyDown(KeyCode.H) && possHeal)
            {
                villagePossibility = false;
                moves.Clear();
                attacks.Clear();
                //Debug.Log("Heel!!");
                possHeal = false;
                PieceMover pm = curPiece.GetComponent<PieceMover>();
                pm.health += 1;
                pm.doneForTurn = true;
                ChangeRemainingMovesText(1);
                halo.enabled = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.V)){
            //Setthisshitout();
        }
    }

    void Setthisshitout()
    {
        foreach (GridSystem g in GridClassList)
        {
            if (g.position == new Vector2Int(13, 94))
            {
                Debug.Log(g.position + "match" + g.godMaterial + " " + g.place + " " + g.teamName + " " + g.settlement);
            }
        }
    }

    public Text movesLeftText;
    const string remainingMoves = " Moves Left";
    public int movesRemaining = 4;

    public void ChangeRemainingMovesText(int remain)
    {
        if (remain == 4)
        {
            movesRemaining = 4;
        }
        else
        {
            movesRemaining -= 1;
        }
        movesLeftText.text = movesRemaining + remainingMoves;
    }

    public void SettleCity(Vector3Int pos, Settlement what)
    {
        Vector2 v2 = new Vector2(pos.x, pos.z);
        foreach (GridSystem g in GridClassList)
        {
            if (g.position == v2)
            {
                //Debug.Log(g.position + "match" + v2);
                g.settlement = what;
            }
            //Debug.Log(g.position + "inte match" + v2);
        }
    }
    Vector3Int v3;

    public void ChangePositionSoldier(GameObject soldier, Place what, string name)
    {
        Vector3 soldierPos = soldier.transform.position;
        if (pieceLocations.ContainsValue(soldier))
        {
            foreach (KeyValuePair<Vector3Int, GameObject> item in pieceLocations)
            {
                if (item.Value == soldier)
                {
                    v3 = item.Key;
                    //GameObject g = pieceLocations[item.Key];
                }
            }
            pieceLocations.Remove(v3);
        }
        if (!pieceLocations.ContainsKey(Vector3Int.FloorToInt(soldierPos)))
        {
            pieceLocations.Add(Vector3Int.FloorToInt(soldierPos), soldier);
        }
        
        Vector2Int v2 = new Vector2Int((int)soldierPos.x, (int)soldierPos.z);
        foreach (GridSystem g in GridClassList)
        {
            if (g.position == v2)
            {
                if (g.settlement == Settlement.City)
                {
                    //Debug.Log("le capitalish" + soldier.name);
                    soldier.GetComponent<PieceMover>().multiPlierHealth = 1.5f;
                }
                else if (g.settlement == Settlement.Village)
                {
                    //Debug.Log("le villish" + soldier.name);
                    soldier.GetComponent<PieceMover>().multiPlierHealth = 1.25f;
                }
                else if (soldier.GetComponent<PieceMover>().multiPlierHealth != 1)
                {
                    soldier.GetComponent<PieceMover>().multiPlierHealth = 1;
                }
                //Debug.LogWarning(g.position + "match" + v2);
                g.place = what;
                g.teamName = name;
            }
        }
    }
    public void MakeGridEmpty(Vector3Int v3, Place what)
    {
        Vector2Int v2 = new Vector2Int(v3.x, v3.z);
        foreach (GridSystem g in GridClassList)
        {
            if (g.position == v2)
            {
                //Debug.LogWarning(g.position + "match" + v2);
                g.place = what;
                g.teamName = "";
            }
        }
    }

    public void ChangeMaterialGrid(Vector3Int pos, Material what)
    {
        Vector2Int v2 = new Vector2Int(pos.x, pos.z);
        foreach (GridSystem g in GridClassList)
        {
            if (g.position == v2)
            {
                //Debug.LogWarning(g.position + "match" + v2);
                g.godMaterial = what;
            }
        }
    }

    Vector2Int coordinates;
    public void ChangeGridName(Vector3Int pos, string tag)
    {
        Vector2Int v2 = new Vector2Int(pos.x, pos.z);
        foreach (GridSystem g in GridClassList)
        {
            if (g.position == v2)
            {
                g.teamName = tag;
                //Debug.LogWarning(v2 + " wupp " + g.teamName );
            }
        }
    }

    public List<Vector2Int> moves = new List<Vector2Int>();
    public List<Vector2Int> attacks = new List<Vector2Int>();

    public void RemoveAttackPos(Vector2Int attPos)
    {
        List<Vector2Int> tempAtts = new List<Vector2Int>();
        foreach (Vector2Int item in attacks)
        {
            if (item != attPos)
            {
                tempAtts.Add(item);
            }
        }
        attacks = tempAtts;
    }
    
    readonly Dictionary<Vector2Int, List<Vector2Int>> tempSavedPossMoveLocations = new Dictionary<Vector2Int, List<Vector2Int>>();
    public bool villagePossibility = false;
    public Material curMaterial;
    GameObject curPiece;
    public List<GameObject> tempPieces = new List<GameObject>();
    string tempTag;

    public GameObject uIAttack, uIMove, uIHeal, uIVillage;
    public GameObject[] uIItems;

    public void ViewOptions(GameObject goPiece, bool damaged, string tag, Piece piece, bool movedThisTurn)
    {
        foreach (GameObject item in uIItems)
        {
            item.SetActive(false);
        }
        doneMove = false;
        possHeal = false;
        curPiece = goPiece;
        curMaterial = FindTeamMaterial(tag); //fixar tempPieces också!
        tempTag = tag;
        villagePossibility = false;
        showAtt = false;
        showMove = false;
        movement.clearBoard = true;
        attack.clearBoard = true;
        foreach (KeyValuePair<Vector3Int, Vector2Int> item in posToColRow)
        {
            if (item.Key.x == (int)goPiece.transform.position.x && item.Key.z == (int)goPiece.transform.position.z)
            {
                coordinates = item.Value;
            }
        }
        if (coordinates.x >= 0 && coordinates.x < MAXCOL && coordinates.y >= 0 && coordinates.y < MAXROW)
        {
            if (!movedThisTurn)
            {
                moves = movement.PossMoveLocations(goPiece, coordinates.x, coordinates.y, tag, piece, false);
                if (moves.Count > 0)
                {
                    uIMove.SetActive(true);
                }
            }
            else
            {
                doneMove = true;
            }
            attacks = attack.PossAttackLocations(goPiece, coordinates.x, coordinates.y, tag, piece);
            if (attacks.Count > 0)
            {
                uIAttack.SetActive(true);
            }
        }
        villagePossibility = CheckVillagePoss(coordinates, goPiece.transform.position);
        if (villagePossibility)
        {
            villCoord = coordinates;
            uIVillage.SetActive(true);
        }
        if (damaged && PossibilityToHeal(Vector3Int.FloorToInt(goPiece.transform.position)))
        {
            possHeal = true;
            uIHeal.SetActive(true);
        }
        //UpdateBoard(goPiece, damaged, piece, movedThisTurn);
    }

    public void UpdateBoard(GameObject goPiece, bool damaged, Piece piece, bool movedThisTurn)
    {
        foreach (KeyValuePair<Vector3Int, Vector2Int> item in posToColRow)
        {
            if (item.Key.x == (int)goPiece.transform.position.x && item.Key.z == (int)goPiece.transform.position.z)
            {
                coordinates = item.Value;
            }
        }
        if (coordinates.x >= 0 && coordinates.x < MAXCOL && coordinates.y >= 0 && coordinates.y < MAXROW)
        {
            if (!movedThisTurn)
            {
                moves = movement.PossMoveLocations(goPiece, coordinates.x, coordinates.y, tag, piece, false);
                if (moves.Count > 0)
                {
                    uIMove.SetActive(true);
                }
            }
            else
            {
                doneMove = true;
            }
            attacks = attack.PossAttackLocations(goPiece, coordinates.x, coordinates.y, tag, piece);
            if (attacks.Count > 0)
            {
                uIAttack.SetActive(true);
            }
        }
        villagePossibility = CheckVillagePoss(coordinates, goPiece.transform.position);
        if (villagePossibility)
        {
            villCoord = coordinates;
            uIVillage.SetActive(true);
        }
        if (damaged && PossibilityToHeal(Vector3Int.FloorToInt(goPiece.transform.position)))
        {
            possHeal = true;
            uIHeal.SetActive(true);
        }
    }

    public bool possHeal = false;

    bool PossibilityToHeal(Vector3Int pos)
    {
        foreach (GridSystem g in GridClassList)
        {
            if (g.position.x == pos.x && g.position.y == pos.z && g.godMaterial == curMaterial && g.godMaterial != defaultMat)
            {
                //Debug.LogError(pos + " jaohwy ");
                return true;
            }
        }
        return false;
    }

    public Material FindTeamMaterial(string tag)
    {
        if (tag == gameManager.oden)
        {
            tempPieces = gameManager.odenPieces;
            return colourManager.oden;
        }
        else if (tag == gameManager.frigg)
        {
            tempPieces = gameManager.friggPieces;
            return colourManager.frigg;
        }
        else if (tag == gameManager.tor)
        {
            tempPieces = gameManager.torPieces;
            return colourManager.tor;
        }
        else if (tag == gameManager.frej)
        {
            tempPieces = gameManager.frejPieces;
            return colourManager.frej;
        }
        else if (tag == gameManager.freja)
        {
            tempPieces = gameManager.frejaPieces;
            return colourManager.freja;
        }
        else if (tag == gameManager.loke)
        {
            tempPieces = gameManager.frejaPieces;
            return colourManager.loke;
        }
        else if (tag == gameManager.balder)
        {
            tempPieces = gameManager.balderPieces;
            return colourManager.balder;
        }
        else if (tag == gameManager.heimdall)
        {
            tempPieces = gameManager.heimdallPieces;
            return colourManager.heimdall;
        }
        else if (tag == gameManager.tyr)
        {
            tempPieces = gameManager.tyrPieces;
            return colourManager.tyr;
        }
        else
        {
            tempPieces = gameManager.idunPieces;
            return colourManager.idun;
        }
    }
    
    public GameObject[] capitals;
    public GameObject[] villages;

    public void SetCapitalsAndVillageLists()
    {
        capitals = GameObject.FindGameObjectsWithTag("Capital");
        villages = GameObject.FindGameObjectsWithTag("Village");
    }
    List<Vector2Int> occupado = new List<Vector2Int>();

    public Material defaultMat;

    bool CheckVillagePoss(Vector2Int coord, Vector3 pos)
    {
        foreach (GridSystem g in GridClassList)
        {
            if (g.position.x == (int)pos.x && g.position.y == (int)pos.z && g.godMaterial != curMaterial && g.godMaterial != defaultMat)// g.teamName != tag)
            {
                //Debug.LogError(coord + " : " + g.godMaterial.ToString() + ", " + curMaterial.ToString());
                return false;
            }
        }
        if (occupado.Count > 0)
        {
            occupado.Clear();
        }

        SetCapitalsAndVillageLists();
        foreach (GameObject capital in capitals)
        {
            CheckPieceAndSettlementLocation(Vector3Int.FloorToInt(capital.transform.position), capital);
            foreach (KeyValuePair<Vector3Int, Vector2Int> pair in posToColRow)
            {
                if (pair.Key.x == (int)capital.transform.position.x && pair.Key.z == (int)capital.transform.position.z)
                {
                    Vector2Int v2 = pair.Value;
                    occupado.Add(v2);
                }
            }
        }
        foreach (GameObject village in villages)
        {
            CheckPieceAndSettlementLocation(Vector3Int.FloorToInt(village.transform.position), village);
            foreach (KeyValuePair<Vector3Int, Vector2Int> pair in posToColRow)
            {
                if (pair.Key.x == (int)village.transform.position.x && pair.Key.z == (int)village.transform.position.z)
                {
                    Vector2Int v2 = pair.Value;
                    occupado.Add(v2);
                }
            }
        }
        //Debug.Log(capitals.Length + ", occu: " + occupado.Count);
        foreach (Vector2Int item in occupado)
        {
            if ((coord.x <= (item.x + 1) && coord.x >= (item.x - 1)) && (coord.y <= (item.y + 1) && coord.y >= (item.y - 1)))
            {
                //Debug.LogWarning(item + ", " + coord);
                return false;
            }
        }
        return true;
    }

    void CheckPieceAndSettlementLocation(Vector3Int v3, GameObject settle)
    {
        foreach (GridSystem g in GridClassList)
        {
            //Debug.LogError(g.position + ", " + v3);// + ", " + settle.GetComponent<Village>().tagTeam);
            if (g.position.x == v3.x && g.position.y == v3.z)
            {
                if (settle.tag == "Village")
                {
                    //Debug.LogError(settle.tag);
                    g.teamName = settle.GetComponent<Village>().tagTeam;
                }
                else if (settle.tag == "Capital")
                {
                    //Debug.LogWarning(settle.tag);
                    g.teamName = settle.GetComponent<Capital>().tagTeam;
                }
            }
        }
        if (!pieceLocations.ContainsKey(v3))
        {
            //Debug.Log("soldat försvann från: " + v3);
            pieceLocations.Add(v3, settle);
            //settle.transform.position = new Vector3(settle.transform.position.x, 1, settle.transform.position.z);
        }
        else
        {

        }
    }

    public bool CheckIfVillageAtPosition(Vector2Int coordinates)
    {
        //Debug.Log(coordinates);
        foreach (GridSystem g in GridClassList)
        {
            if (g.coordinates == coordinates && g.settlement == Settlement.Village)
            {
                //Debug.LogError(g.coordinates);
                return true;
            }
        }
        return false;
    }

    public int GetNumberOfTiles(Material teamMat) //For score board and some winning scenarios
    {
        int score = 0;
        foreach (GridSystem g in GridClassList)
        {
            if (g.godMaterial == teamMat)
            {
                score++;
            }
        }
        return score;
    }

    //UI:
    public void AcceptVillage()
    {
        acceptedVillage = true;
    }

    public void CloseScreenOrImage(GameObject go)
    {
        go.SetActive(false);
    }
}

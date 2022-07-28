using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    ColourManager colourManager;
    public GameObject capitalCityPrefab, villagePrefab;

    public readonly string oden = "Oden", frigg = "Frigg", tor = "Tor", frej = "Frej", freja = "Freja", 
        loke = "Loke", balder = "Balder", heimdall = "Heimdall", tyr = "Tyr", idun = "Idun";
    public GameObject odenPrefab, friggPrefab, torPrefab, frejPrefab, frejaPrefab,
        lokePrefab, balderPrefab, heimdallPrefab, tyrPrefab, idunPrefab;
    public static readonly float pieceHeightY = 3.3f;

    public GameObject marauder, archer, horsemen; // Prefabs

    public List<GameObject> odenPieces = new List<GameObject>(), friggPieces = new List<GameObject>(),
        torPieces = new List<GameObject>(), frejPieces = new List<GameObject>(), frejaPieces = new List<GameObject>(),
        lokePieces = new List<GameObject>(), balderPieces = new List<GameObject>(), heimdallPieces = new List<GameObject>(),
        tyrPieces = new List<GameObject>(), idunPieces = new List<GameObject>();

    Attack att;
    Movement move;

    GridControl gridControl;

    public Text generationText;
    public const string soldierGen = "New Soldier Generation Process: ";
    public GameObject genScreen;
    public Text maximumRoundText;
    const string roundsLeft = " Rounds Remaining";

    Controller controller;
    GameObject ai;

    // Start is called before the first frame update
    void Awake()
    {
        genScreen.SetActive(false);
        colourManager = GetComponent<ColourManager>();
        att = GetComponent<Attack>();
        move = GetComponent<Movement>();
        gridControl = GridControl.GridControlSingleTon;
        generationText.text = "";
        maximumRoundText.text = "";
        ai = GameObject.FindGameObjectWithTag("AI");
        controller = ai.GetComponent<Controller>();
    }

    bool visLogos = true;
    bool initiateNewTurn = false;

    bool closeInput = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && visLogos && logos.Count > 0)
        {
            visLogos = false;
            foreach (GameObject logo in logos)
            {
                logo.SetActive(false);
            }
        }
        else if (Input.GetKeyDown(KeyCode.L) && !visLogos && logos.Count > 0)
        {
            visLogos = true;
            foreach (GameObject logo in logos)
            {
                logo.SetActive(true);
            }
        }
        if (!closeInput)
        {
            if (Input.GetKeyDown(KeyCode.Return) && !initiateNewTurn)
            {
                EndTeamTurn(currentTeam);
                NewTurnReset(false);
            }
        }

        if (initiateNewTurn)
        {
            initiateNewTurn = false;
            if (maxPlayers < turnOrder)
            {
                turnOrder = 1;   
            }
            NewTurn(turnOrder); 
        }

        if (soldierGeneration && mar) //Marauder
        {
            GenerateNewSoldier(marauder);
        }
        else if (soldierGeneration && arch) //Archer
        {
            GenerateNewSoldier(archer);
        }
        else if (soldierGeneration && horse) //Horsemen
        {
            GenerateNewSoldier(horsemen);
        }
    }

    public void NewTurnReset(bool AI)
    {
        att.clearBoard = true;
        move.clearBoard = true;
        gridControl.textDoneTurnInfo.SetActive(false);
        gridControl.haloKeeper.transform.position = gridControl.startHalo;
        gridControl.turnDone = false;
        gridControl.ChangeRemainingMovesText(4);
        gridControl.showAtt = true;
        gridControl.attacks.Clear();
        gridControl.showMove = true;
        gridControl.moves.Clear();
        gridControl.villagePossibility = false;
        gridControl.possHeal = false;
        foreach (GameObject s in spawnObjects)
        {
            Destroy(s);
        }
        initiateNewTurn = true;
        if (AI)
        {
            closeInput = false;
        }
    }

    void GenerateNewSoldier(GameObject soldier)
    {
        TeamTurn(currentTeam); //Starts turn before soldier gets created
        SpawnSoldierUnit(soldier, possSpawnCoordinates.x, possSpawnCoordinates.y, curMaterial, curPlayer.Team, Quaternion.Euler(0, 90, 0));
        soldierGeneration = false;
        horse = false;
        arch = false;
        mar = false;
        closeInput = false;
    }

    int turnOrder = 1;
    int maxPlayers;

    public void SetUpGame(int nrPl)
    {
        if (!UniversalSettings.limitedRounds)
        {
            round = -5;
            maximumRoundText.text = "";
        }
        maxPlayers = nrPl;
        foreach (Player p in UniversalSettings.players)
        {
            p.CapitalPosition = FindCorrectCapital(p.Team); // Always saves home adress that doesnt change during the game
            List<Vector2Int> temp = new List<Vector2Int>();
            p.Pieces = FindNamePieces(p.Team);
            //foreach (GameObject go in FindNamePieces(p.Team)) sv fixa player.positions, behövs det?
            //{
            //    Vector2Int v2int = 
            //}
            Vector2Int start = StartPositionGenerator(nrPl, p.Order);
            if (start.x == 0 && start.y == 0)
            {
                Debug.LogError("errör! wröng start position fönd!");
            }
            else
            {
                Material teamMat = gridControl.FindTeamMaterial(p.Team);
                BuildSettlement(start.x, start.y, GridControl.Settlement.City, true, teamMat, FindNamePieces(p.Team), p.Team);
            }
        }
        NewTurn(turnOrder); //Initiates a new turn and the start of the game
    }

    Vector2Int StartPositionGenerator(int nrPl, int order)
    {
        // Manually order of positions
        if (order == 1)
        {
            return new Vector2Int(2, 14);
        }
        else if (nrPl == 2 && order == 2)
        {
            return new Vector2Int(14, 2);
        }
        else if (nrPl == 3)
        {
            if (order == 2)
            {
                return new Vector2Int(14, 14);
            }
            else
            {
                return new Vector2Int(8, 2);
            }
        }
        else if (nrPl == 4)
        {
            if (order == 2)
            {
                return new Vector2Int(14, 14);
            }
            else if (order == 3)
            {
                return new Vector2Int(2, 2);
            }
            else
            {
                return new Vector2Int(14, 2);
            }
        }
        else if (nrPl == 5)
        {
            if (order == 2)
            {
                return new Vector2Int(14, 14);
            }
            else if (order == 3)
            {
                return new Vector2Int(8, 8);
            }
            else if (order == 4)
            {
                return new Vector2Int(2, 2);
            }
            else
            {
                return new Vector2Int(14, 2);
            }
        }
        else if (nrPl == 6)
        {
            if (order == 2)
            {
                return new Vector2Int(8, 14);
            }
            else if (order == 3)
            {
                return new Vector2Int(14, 14);
            }
            else if (order == 4)
            {
                return new Vector2Int(2, 2);
            }
            else if (order == 5)
            {
                return new Vector2Int(8, 2);
            }
            else
            {
                return new Vector2Int(14, 2);
            }
        }
        else if (nrPl == 7)
        {
            if (order == 2)
            {
                return new Vector2Int(8, 14);
            }
            else if (order == 3)
            {
                return new Vector2Int(14, 14);
            }
            else if (order == 4)
            {
                return new Vector2Int(8, 8);
            }
            else if (order == 5)
            {
                return new Vector2Int(2, 2);
            }
            else if (order == 6)
            {
                return new Vector2Int(8, 2);
            }
            else
            {
                return new Vector2Int(14, 2);
            }
        }
        else if (nrPl == 8)
        {
            if (order == 2)
            {
                return new Vector2Int(8, 14);
            }
            else if (order == 3)
            {
                return new Vector2Int(14, 14);
            }
            else if (order == 4)
            {
                return new Vector2Int(2, 8);
            }
            else if (order == 5)
            {
                return new Vector2Int(14, 8);
            }
            else if (order == 6)
            {
                return new Vector2Int(2, 2);
            }
            else if (order == 7)
            {
                return new Vector2Int(8, 2);
            }
            else
            {
                return new Vector2Int(14, 2);
            }
        }
        else if (nrPl == 9)
        {
            if (order == 2)
            {
                return new Vector2Int(8, 14);
            }
            else if (order == 3)
            {
                return new Vector2Int(14, 14);
            }
            else if (order == 4)
            {
                return new Vector2Int(2, 8);
            }
            else if (order == 5)
            {
                return new Vector2Int(8, 8);
            }
            else if (order == 6)
            {
                return new Vector2Int(14, 8);
            }
            else if (order == 7)
            {
                return new Vector2Int(2, 2);
            }
            else if (order == 8)
            {
                return new Vector2Int(8, 2);
            }
            else
            {
                return new Vector2Int(14, 2);
            }
        }
        else
        {
            return new Vector2Int(0, 0);
        }
    }

    public List<GameObject> FindNamePieces(string teamName)
    {
        if (teamName.Equals(oden))
        {
            return odenPieces;
        }
        else if (teamName.Equals(tor))
        {
            return torPieces;
        }
        else if (teamName.Equals(frigg))
        {
            return friggPieces;
        }
        else if (teamName.Equals(frej))
        {
            return frejPieces;
        }
        else if (teamName.Equals(freja))
        {
            return frejaPieces;
        }
        else if (teamName.Equals(loke))
        {
            return lokePieces;
        }
        else if (teamName.Equals(balder))
        {
            return balderPieces;
        }
        else if (teamName.Equals(heimdall))
        {
            return heimdallPieces;
        }
        else if (teamName.Equals(tyr))
        {
            return tyrPieces;
        }
        else
        {
            return idunPieces;
        }
    }


    public Text turnText;
    readonly string tTurn = "TURN: ";

    Material curMaterial;
    Vector2Int possSpawnCoordinates;
    Player curPlayer;
    List<GameObject> curPieces; //Behövs denna?

    string currentTeam;

    public string GetCurrentTeam()
    {
        return currentTeam;
    }

    float curDamage;

    public float SetCurDamage(float damage)
    {
        curDamage = damage;
        return curDamage;
    }
    public float GetCurDamage()
    {
        return curDamage;
    }
    public int round;
    //public int rateGeneration;
    bool firstRound = true;

    bool genRound = false;

    readonly int haloHeight = 32;

    void NewTurn(int turn)
    {
        if (UniversalSettings.limitedRounds)
        {
            if (turn == 1 && !firstRound)
            {
                round -= 1;
            }
            else if (firstRound)
            {
                firstRound = false;
            }
            maximumRoundText.text = round + roundsLeft;
            if (round == 0)
            {
                //Debug.LogWarning("Game over, but who won?");
            }
        }
        foreach (Player p in UniversalSettings.players)
        {
            curDamage = 0;
            curPlayer = p;
            if (curPlayer.Order == turn && !curPlayer.defeated)
            {
                curPlayer.Pieces = FindNamePieces(curPlayer.Team);
                curPlayer.GodCoordinates = FindGodCoordinates(curPlayer);
                gridControl.turnPointer.transform.position =
                    new Vector3(curPlayer.CapitalPosition.x, haloHeight, curPlayer.CapitalPosition.y);
                currentTeam = curPlayer.Team;
                curMaterial = gridControl.FindTeamMaterial(curPlayer.Team);
                curPieces = gridControl.tempPieces;

                if (UniversalSettings.regenaration)
                {
                    if (curPlayer.Regeneration < UniversalSettings.regenerationRate)
                    {
                        genRound = false;
                        curPlayer.Regeneration++;
                    }
                    if (curPlayer.Regeneration >= UniversalSettings.regenerationRate) // AI motsvarigheten happens in Controller
                    {
                        //if (!p.AI)
                        {
                            ShowSpawnPositions();
                        }
                        genRound = true;
                    }
                    generationText.text = soldierGen + curPlayer.Regeneration + "/" + UniversalSettings.regenerationRate;
                }
                
                turnText.text = tTurn + currentTeam;
                if (!genRound)
                {
                    TeamTurn(currentTeam);
                }
                //if (p.AI)
                //{
                //    closeInput = true; 
                //    controller.AITurn(p);
                //}
                
                turnOrder++;
                break;
            }
            else if (curPlayer.Order == turn && curPlayer.defeated)
            {
                turnOrder++;
                // sv behövs inte(?): EndTeamTurn(curPlayer.Team);
                initiateNewTurn = true;
                break;
            }
        }
    }

    Vector2Int FindGodCoordinates(Player p) // sv spara gudarna som gameobject ist när spelet startar?
    {
        foreach (GameObject g in p.Pieces)
        {
            if (g.GetComponent<PieceMover>().god)
            {
                return gridControl.GetCoordinates(g);
            }
        }
        return Vector2Int.left;
    }
    
    List<GameObject> spawnObjects = new List<GameObject>();
    public GameObject spawnPrefab;
    public List<GameObject> ShowSpawnPositions()
    {
        spawnObjects.Clear();
        foreach (GridControl.GridSystem g in GridControl.GridClassList)
        {
            if (g.godMaterial.Equals(curMaterial) && g.place == GridControl.Place.Empty)
            {
                GameObject s = Instantiate(spawnPrefab, g.grid.transform.position, Quaternion.identity);
                spawnObjects.Add(s);
            }
        }
        return spawnObjects;
    }

    public void SetNewSpawn(Vector3 pos)
    {
        foreach (GridControl.GridSystem g in GridControl.GridClassList)
        {
            if (g.position.x == (int)pos.x && g.position.y == (int)pos.z)
            {
                possSpawnCoordinates = g.coordinates;
                soldierGeneration = true;
                foreach (GameObject s in spawnObjects)
                {
                    Destroy(s);
                }

                genScreen.SetActive(true);
            }
        }
        closeInput = true;
        curPlayer.Regeneration = 0;
    }

    bool mar = false, arch = false, horse = false;
    bool soldierGeneration = false;

    public void NewSoldier(int choise) // Gets activated at button press during regeneration
    {
        mar = false;
        arch = false;
        horse = false;
        if (genScreen == null)
        {
            genScreen = GameObject.FindGameObjectWithTag("GenScreen");
        }
        genScreen.SetActive(false);
        soldierGeneration = true;
        if (choise == 0) //Marauder
        {
            mar = true;
        }
        else if (choise == 1) //Archer
        {
            arch = true;
        }
        else if (choise == 2) //Horsemen
        {
            horse = true;
        }
    }

    public Vector2Int FindCorrectCapital(string teamName)
    {
        GameObject[] capitalish = GameObject.FindGameObjectsWithTag("Capital");
        foreach (GameObject capital in capitalish)
        {
            if (capital.GetComponent<Capital>().tagTeam.Equals(teamName))
            {
                return new Vector2Int((int)capital.transform.position.x, (int)capital.transform.position.z);
            }
        }
        //Safeplay:
        return new Vector2Int((int)gridControl.turnStartHalo.x, (int)gridControl.turnStartHalo.y);
    }

    void TeamTurn(string team)
    {
        UpdateCapsAndVillages();
        CheckHealthVillagesCapitals(team);
        GameObject[] teamP = GameObject.FindGameObjectsWithTag(team);
        foreach (GameObject game in teamP)
        {
            PieceMover pm = game.GetComponent<PieceMover>();
            pm.movedThisTurn = false;
            pm.doneForTurn = false;
            pm.darkLogo.SetActive(false);
            pm.movesTaken = 0;
        }
    }

    public GameObject[] capitals = new GameObject[0];
    public GameObject[] villages = new GameObject[0];

    public void UpdateCapsAndVillages()
    {
        capitals = GameObject.FindGameObjectsWithTag("Capital");
        villages = GameObject.FindGameObjectsWithTag("Village");
    }

    void CheckHealthVillagesCapitals(string teamName)
    {
        foreach (GameObject capital in capitals)
        {
            Capital cap = capital.GetComponent<Capital>();
            if (cap.tagTeam == teamName && cap.health < 20)
            {
                cap.health += 1;
            }
        }
        foreach (GameObject village in villages)
        {
            Village vil = village.GetComponent<Village>();
            if (vil.tagTeam == teamName && vil.health < 10)
            {
                vil.health += 1;
            }
        }
    }

    public void EndTeamTurn(string team)
    {
        GameObject[] teamP = GameObject.FindGameObjectsWithTag(team);
        foreach (GameObject game in teamP)
        {
            PieceMover pm = game.GetComponent<PieceMover>();
            pm.doneForTurn = true;
        }
    }

    public List<GameObject> logos = new List<GameObject>();

    public void BuildSettlement(int row, int col, GridControl.Settlement settlement, bool capital, Material colour, List<GameObject> pieces, string tag)
    {
        GameObject gibby = gridControl.grids2D[row, col];
        gridControl.SettleCity(Vector3Int.FloorToInt(gibby.transform.position), settlement);
        colourManager.ColourCity(new Vector2Int(row, col), capital, colour, false);
        if (capital)
        {
            GameObject cap = Instantiate(capitalCityPrefab, new Vector3(gibby.transform.position.x, 0.2f, gibby.transform.position.z), Quaternion.identity);
            GameObject god = Instantiate(GetGod(colour), new Vector3(gibby.transform.position.x, pieceHeightY, gibby.transform.position.z), Quaternion.identity);
            SetLogoGod(god);
            gridControl.ChangePositionSoldier(god, GridControl.Place.Occupied, tag);
            Capital c = cap.GetComponent<Capital>();
            c.SetVariables(new Vector2Int(row, col), tag);
            gridControl.ChangeGridName(Vector3Int.FloorToInt(cap.transform.position), tag);
            SetUpSoldiers(row, col, colour, pieces, tag);
        }
        else // if  village:
        {
            GameObject village = Instantiate(villagePrefab, new Vector3(gibby.transform.position.x, 0.1f,
                gibby.transform.position.z), Quaternion.identity);
            Village v = village.GetComponent<Village>();
            v.SetVariables(new Vector2Int(row, col), tag);
            gridControl.ChangeGridName(Vector3Int.FloorToInt(village.transform.position), tag);
        }
    }

    GameObject GetGod(Material godColour)
    {
        if (godColour == colourManager.oden)
        {
            return odenPrefab;
        }
        if (godColour == colourManager.frigg)
        {
            return friggPrefab;
        }
        if (godColour == colourManager.tor)
        {
            return torPrefab;
        }
        if (godColour == colourManager.frej)
        {
            return frejPrefab;
        }
        if (godColour == colourManager.freja)
        {
            return frejaPrefab;
        }
        if (godColour == colourManager.loke)
        {
            return lokePrefab;
        }
        if (godColour == colourManager.balder)
        {
            return balderPrefab;
        }
        if (godColour == colourManager.heimdall)
        {
            return heimdallPrefab;
        }
        if (godColour == colourManager.tyr)
        {
            return tyrPrefab;
        }
        else
        {
            return idunPrefab;
        }
    }

    void SetLogoGod(GameObject god)
    {
        for (int i = 0; i < god.transform.childCount; i++)
        {
            Transform child = god.transform.GetChild(i);
            if (child.tag == "Helmet")
            {
                logos.Add(child.gameObject);
            }
            if (child.childCount > 0)
            {
                SetLogoGod(child.gameObject);
            }
        }
    }

    void SetUpSoldiers(int row, int col, Material team, List<GameObject> pieces, string tag)
    {
        int lowRow = row - 2;
        int highRow = row + 2;
        int lowCol = col - 2;
        int highCol = col + 2;

        SpawnSoldierUnit(marauder, lowRow, col, team, tag, Quaternion.Euler(0, 90, 0));
        SpawnSoldierUnit(marauder, highRow, col, team, tag, Quaternion.Euler(0, 270, 0));
        SpawnSoldierUnit(marauder, row, lowCol, team, tag, Quaternion.Euler(0, 0, 0));
        SpawnSoldierUnit(marauder, row, highCol, team, tag, Quaternion.Euler(0, 180, 0));

        int redRow = row - 1;
        int addRow = row + 1;
        int redCol = col - 1;
        int addCol = col + 1;

        SpawnSoldierUnit(archer, redRow, addCol, team, tag, Quaternion.Euler(0, 135, 0));
        SpawnSoldierUnit(archer, addRow, addCol, team, tag, Quaternion.Euler(0, 225, 0));
        SpawnSoldierUnit(archer, addRow, redCol, team, tag, Quaternion.Euler(0, 315, 0));
        SpawnSoldierUnit(archer, redRow, redCol, team, tag, Quaternion.Euler(0, 45, 0));

        SpawnSoldierUnit(horsemen, redRow, col, team, tag, Quaternion.Euler(0, 90, 0));
        SpawnSoldierUnit(horsemen, addRow, col, team, tag, Quaternion.Euler(0, 270, 0));
        SpawnSoldierUnit(horsemen, row, redCol, team, tag, Quaternion.Euler(0, 0, 0));
        SpawnSoldierUnit(horsemen, row, addCol, team, tag, Quaternion.Euler(0, 180, 0));

        SetPieces(pieces, tag);
    }

    readonly string teamColour = "Helmet";
    public GameObject SpawnSoldierUnit(GameObject soldier, int row, int col, Material team, string tag, Quaternion angle)
    {
        //Debug.Log(row + "," + col + " 1: " + tag);
        GameObject freshSoldier;
        if (gridControl == null)
        {
            gridControl = GridControl.GridControlSingleTon;
        }
        GameObject sold = gridControl.grids2D[row, col];
        freshSoldier = Instantiate(soldier, new Vector3(sold.transform.position.x, pieceHeightY, sold.transform.position.z), angle);
        List<GameObject> templist = TransformExtensions.FindSoldierPiecesToColour(freshSoldier.transform, teamColour, team);
        foreach (GameObject logo in templist)
        {
            if (logo.name.Equals("Logo"))
            {
                logos.Add(logo);
            }
        }
        freshSoldier.tag = tag;

        /*if (soldier.name.Equals("Marauder")) // sv onödigt?
        {
            GridControl.Piece piece = new GridControl.Piece
            {
                NamePiece = GridControl.NamePiece.Marauder,
                coordinates = new Vector2Int(row, col)
            };
        }
        else if (soldier.name.Equals("Archer"))
        {
            GridControl.Piece piece = new GridControl.Piece
            {
                NamePiece = GridControl.NamePiece.Archer,
                coordinates = new Vector2Int(row, col)
            };
        }
        else
        {
            GridControl.Piece piece = new GridControl.Piece
            {
                NamePiece = GridControl.NamePiece.Horsemen,
                coordinates = new Vector2Int(row, col)
            };
        }*/

        gridControl.ChangePositionSoldier(freshSoldier, GridControl.Place.Occupied, tag);
        return freshSoldier;
    }

    List<GameObject> SetPieces(List<GameObject> pieces, string tag)
    {
        GameObject[] temp = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject go in temp)
        {
            pieces.Add(go);
        }
        return pieces;
    }

}
public static class TransformExtensions
{
    public static List<GameObject> FindSoldierPiecesToColour(this Transform parent, string tag, Material team)
    {
        
        List<GameObject> taggedGameObjects = new List<GameObject>();

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.tag == tag)
            {
                taggedGameObjects.Add(child.gameObject);
                if (child.gameObject.name == "Logo")
                {
                    RawImage ri = child.gameObject.GetComponent<RawImage>();
                    ri.color = team.color;
                }
                else
                {
                    child.gameObject.GetComponent<Renderer>().material = team;
                }
            }
            if (child.childCount > 0)
            {
                taggedGameObjects.AddRange(FindSoldierPiecesToColour(child, tag, team));
            }
        }
        return taggedGameObjects;
    }
}

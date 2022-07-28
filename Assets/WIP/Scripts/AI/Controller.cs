using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls the AI as central area for other AI-scripts: 

public class Controller : MonoBehaviour
{
    public class AIAttack
    {
        public AIAttack(GridControl.Piece p, GridControl.Piece def, float score)
        {
            AttPiece = p;
            DefPiece = def;
            this.score = score;
        }
        private GridControl.Piece attPiece;
        private GridControl.Piece defPiece;
        private float score;

        public GridControl.Piece AttPiece { get => attPiece; set => attPiece = value; }
        public GridControl.Piece DefPiece { get => defPiece; set => defPiece = value; }
        public float Score { get => score; set => score = value; }
        public AIAttack(GridControl.Piece p, Village vill)
        {
            attPiece = p;
            defVill = vill;
            score = defVill.health;
        }
        private Village defVill;

        public AIAttack(GridControl.Piece p, Capital cap)
        {
            attPiece = p;
            defCap = cap;
            score = defCap.health;
        }
        private Capital defCap;
    }


    Guide guide;
    Mapper mapper;
    UserInfo userInfo;
    GridControl gc;
    GameObject gameManager;
    GameManager gm;

    Attack attack;

    // Start is called before the first frame update
    void Awake()
    {
        gc = GridControl.GridControlSingleTon;
        gameManager = GameObject.FindGameObjectWithTag("GameController");
        gm = gameManager.GetComponent<GameManager>();
        guide = GetComponent<Guide>();
        mapper = GetComponent<Mapper>();
        userInfo = GetComponent<UserInfo>();

        attack = gameManager.GetComponent<Attack>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AITurn(Player player)
    {
        List<Vector2Int> dangerZones = mapper.GetPossibleAttacks(player.Team);
        List<Vector2Int> possTargets = mapper.GetPossibleTargets(player.Team);
        if (UniversalSettings.regenaration)
        {
            if (player.Regeneration >= UniversalSettings.regenerationRate)
            {
                List<GameObject> possSpawns = gm.ShowSpawnPositions();
                Vector2Int v2int = GetSpawnCoordinates(possSpawns);
                if (possSpawn)
                {
                    GameObject soldier = GetBestUnit(gm.FindNamePieces(player.Team));
                    gm.SpawnSoldierUnit(soldier, v2int.x, v2int.y, gc.FindTeamMaterial(player.Team), player.Team, Quaternion.Euler(0, 90, 0));
                }
                player.Regeneration = 0;
            }
            
        }
        // Find close enemies etc and moves the units:
        guide.FindCLoseInformation(player);
        // Gather information about ev. god movement or attack:


        // Attack enemies (if possible):
        // Attack cities/villages:
        AttackDecider(player);

        // Create Village(s) (if possible):
        CheckVillageConstruction();

        //foreach (Guide.SpecScore s in guide.GetScoreList()) // ca 37 per motståndare + ev village poäng
        //{
        //    Debug.Log(s.Coordinates + " " + s.Spec + " " + s.Score);
        //}
        guide.ClearScores();

        gm.EndTeamTurn(player.Team);
        gm.NewTurnReset(true);
    }

    bool possSpawn = true;

    Vector2Int GetSpawnCoordinates(List<GameObject> objs)
    {
        possSpawn = true;
        Vector2Int temp = Vector2Int.zero;
        if (objs.Count == 0)
        {
            possSpawn = false;
            return temp;
        }
        foreach (GameObject game in objs)
        {
            temp = gc.GetCoordinates(new Vector2Int((int)game.transform.position.x, (int)game.transform.position.z));
            return temp;
        }
        return temp;
    }
    // Returns unit for spawn, marauder first 
    GameObject GetBestUnit(List<GameObject> units)
    {
        int mar = 0, arch = 0, horse = 0;
        foreach (GameObject g in units)
        {
            PieceMover pm = g.GetComponent<PieceMover>();
            if (g.name.Equals("Horsemen(Clone)"))
            {
                mar++;
            }
            else if (g.name.Equals("Marauder(Clone)"))
            {
                arch++;
            }
            else if (g.name.Equals("Archer(Clone)"))
            {
                horse++;
            }
        }
        if (mar > 0 && arch == 0)
        {
            return gm.archer;
        }
        else if (mar > 0 && horse == 0)
        {
            return gm.horsemen;
        }
        else
        {
            return gm.marauder;
        }
    }


    void AttackDecider(Player player)
    {
        List<AIAttack> possAllAtt = new List<AIAttack>();
        List<AIAttack> possMultiAtt = new List<AIAttack>();
        // attackera en unit som flera kan attackera i samma runda
        foreach (GameObject s in player.Pieces)
        {
            attacks = CheckAttackOptions(s);
            foreach (AIAttack item in attacks)
            {
                if (!possAllAtt.Contains(item))
                {
                    possAllAtt.Add(item);
                }
                else
                {
                    possMultiAtt.Add(item);
                }
            }
        }
        GetBestAttacks(UniversalSettings.nrUnitMoves, possAllAtt);

        // Debug.LogError(possAllAtt.Count + " " + possMultiAtt.Count);
        // attackera nära capital
        // att byar o främst capitals
    }

    List<AIAttack> attacks = new List<AIAttack>();

    List<AIAttack> CheckAttackOptions(GameObject sold) 
    {
        PieceMover pm = sold.GetComponent<PieceMover>();
        List<GameObject> enemies = new List<GameObject>();
        if (!pm.god) // God attacks is controlled from "Controller."
        {
            GridControl.Piece piece = pm.GetPiece();
            List<Vector2Int> possAttCoordinates = attack.PossAttackLocations(sold, piece.coordinates.x, piece.coordinates.y, sold.tag, piece);
            foreach (Vector2Int v in possAttCoordinates)
            {
                Vector3Int v3 = new Vector3Int(v.x, 3, v.y);
                if (gc.pieceLocations.ContainsKey(v3))
                {
                    GameObject enemy = gc.pieceLocations[v3];
                    PieceMover oppPM = enemy.GetComponent<PieceMover>();
                    AIAttack aI = new AIAttack(piece, oppPM.GetPiece(), oppPM.health);
                    attacks.Add(aI);
                    Debug.LogWarning("fiende hittad: " + enemy.tag);
                    enemies.Add(enemy);
                }
                else 
                {
                    gc.SetCapitalsAndVillageLists();
                    if (gc.villages.Length > 0)
                    {
                        foreach (GameObject village in gc.villages)
                        {
                            Village vill = village.GetComponent<Village>();
                        // sv Debug.LogError(gc.GetCoordinates(v) +  " vill hittad: " + vill.gridPosition);
                            if (vill.gridPosition == gc.GetCoordinates(v))
                            {
                                AIAttack aI = new AIAttack(piece, vill);
                                attacks.Add(aI);
                                Debug.Log("vill hittad: " + vill.gridPosition);
                            }
                        }
                    }
                    foreach (GameObject capital in gc.capitals)
                    {
                        Capital cap = capital.GetComponent<Capital>();
                        // sv Debug.LogError(gc.GetCoordinates(v) + "cap hittad: " + cap.gridPosition);
                        if (cap.gridPosition == gc.GetCoordinates(v))
                        {
                            AIAttack aI = new AIAttack(piece, cap);
                            attacks.Add(aI);
                            Debug.Log("cap hittad: " + cap.gridPosition);
                        }
                    }

                }
            }
        }
        return attacks;
    }

    List<AIAttack> GetBestAttacks(int nrAttacks, List<AIAttack> atts)
    {
        List<AIAttack> bestAtts = new List<AIAttack>();
        float max = float.MaxValue;
        foreach (AIAttack aI in atts)
        {
            if (aI.Score < max)
            {
                Debug.LogWarning(aI.Score + " av: " + max);
                bestAtts.Add(aI);
                max = aI.Score;
            }
        }
        List<AIAttack> sortBestAtts = new List<AIAttack>();
        for (int i = nrAttacks; i >= 0; i--)
        {
            if (bestAtts.Count > i)
            {
                sortBestAtts.Add(bestAtts[i]);
            }
        }
        Debug.Log(sortBestAtts.Count); 
        return sortBestAtts;
    }

    void CheckVillageConstruction()
    {

    }
}

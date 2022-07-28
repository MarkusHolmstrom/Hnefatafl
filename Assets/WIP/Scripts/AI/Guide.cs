using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Guides the AI regarding different options: 

public class Guide : MonoBehaviour
{
    public enum Direction { NorthEast, North, NorthWest, West, East, SouthEast, South, SouthWest, None };
    public enum Spec { Capital, Village, Enemy, God, Useless };


    GridControl gc;

    GameObject gameManager;
    GameManager gm;

    Movement movement;

    public class SpecScore 
    { 
        public SpecScore(float score, Vector2Int coordinates, Spec spec) 
        {
            this.Score = score;
            this.Coordinates = coordinates;
            this.Spec = spec;
        }
        public float Score { get; set; }  
        public Vector2Int Coordinates { get; set; }
        public Spec Spec { get; set; }
    };

    List<SpecScore> scores = new List<SpecScore>();

    public List<SpecScore> GetScoreList()
    {
        return scores;
    }
    public void ClearScores()
    {
        scores.Clear();
    }

    // Start is called before the first frame update
    void Awake()
    {
        gc = GridControl.GridControlSingleTon;
        gameManager = GameObject.FindGameObjectWithTag("GameController");
        gm = gameManager.GetComponent<GameManager>();
        movement = gameManager.GetComponent<Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FindCLoseInformation(Player player)
    {
        foreach (GameObject piece in player.Pieces)
        {
            List<Vector2Int> dangerCoordinates = new List<Vector2Int>();
            Vector2Int coordinates = gc.GetCoordinates(new Vector2Int((int)piece.transform.position.x, (int)piece.transform.position.z));
            SpecScore capScore = GetClosestOppCapital(coordinates, player);
            if (capScore.Coordinates != Vector2Int.left)
            {
                scores.Add(capScore);
                dangerCoordinates.Add(capScore.Coordinates);
            }

            if (gm.villages.Length > 0)
            {
                SpecScore villScore = GetClosestVillage(coordinates, player);
                if (villScore.Coordinates != Vector2Int.left)
                {
                    scores.Add(villScore);
                    dangerCoordinates.Add(villScore.Coordinates);
                }
            }

            SpecScore enemyScore = GetClosestEnemy(piece, player);
            if (enemyScore.Coordinates != Vector2Int.left)
            {
                scores.Add(enemyScore);
                dangerCoordinates.Add(enemyScore.Coordinates);
            }

            SpecScore godScore = GetClosestGod(player, coordinates);
            if(godScore.Spec == Spec.God)
            {
                scores.Add(godScore);
                dangerCoordinates.Add(godScore.Coordinates);
            }

            Direction dir = GetDirection(dangerCoordinates, coordinates); // sv de drar sig mot sw av ngn anledning? vädret?
            CheckMovePositions(piece, dir); // Also moves the AI piece somewhere
            //Debug.LogWarning("Lölz, ingen varning!! " + dangerCoordinates.Count);
        }
    }

    Vector2Int tempVector;

    SpecScore GetClosestOppCapital(Vector2Int coordinates, Player p)
    {
        List<SpecScore> temps = new List<SpecScore>();
        float prevScore = float.MaxValue;
        SpecScore score = new SpecScore(prevScore, Vector2Int.left, Spec.Capital);
        foreach (GameObject cap in gm.capitals)
        {
            float tempScore = GetCapitalScore(cap, coordinates);
            if (prevScore > tempScore && !cap.tag.Equals(p.Team))
            {
                score = new SpecScore(tempScore, tempVector, Spec.Capital);
                temps.Add(score);
                prevScore = tempScore;
            }
        }
        // sv Debug.LogError(prevScore);
        return score;
    }

    float GetCapitalScore(GameObject cap, Vector2Int pieceCoordinates)
    {
        Capital cappy = cap.GetComponent<Capital>();
        float score = cappy.health;
        tempVector = gc.GetCoordinates(cappy.gridPosition);
        score += Vector2Int.Distance(pieceCoordinates, tempVector);
        
        return score;
    }

    SpecScore GetClosestVillage(Vector2Int coordinates, Player p) // Can also find closest village to protect
    {
        List<SpecScore> temps = new List<SpecScore>();
        float prevScore = float.MaxValue;
        SpecScore score = new SpecScore(prevScore, Vector2Int.left, Spec.Capital);
        foreach (GameObject village in gm.villages)
        {
            float tempScore = GetVillageScore(village, coordinates);
            if (prevScore > tempScore && !village.tag.Equals(p.Team))
            {
                score = new SpecScore(tempScore, tempVector, Spec.Village);
                temps.Add(score);
                prevScore = tempScore;
            }
            else if (village.tag.Equals(p.Team))
            {
                prevScore = village.GetComponent<Village>().health;
                score = new SpecScore(prevScore, tempVector, Spec.Village);
            }
        }
        //Debug.LogWarning(prevScore);
        return score;
    }

    float GetVillageScore(GameObject vill, Vector2Int pieceCoordinates)
    {
        Village village = vill.GetComponent<Village>();
        float score = village.health;
        tempVector = gc.GetCoordinates(village.gridPosition);
        score += Vector2Int.Distance(pieceCoordinates, tempVector);

        return score;
    }

    SpecScore GetClosestEnemy(GameObject piece, Player p)
    {
        List<Vector2Int> temp = new List<Vector2Int>();
        Vector2Int v2 = LookForEnemies(gc.GetCoordinates(piece));
        float score = float.MaxValue;
        if (v2 != Vector2Int.left)
        {
            temp.Add(v2);
            foreach (KeyValuePair<Vector3Int, GameObject> pair in gc.pieceLocations)
            {
                if (!pair.Value.tag.Equals(p.Team) && pair.Key.x == v2.x && pair.Key.z == v2.y)
                {
                    score = GetEnemyScore(pair.Value, v2);
                }
            }
        }
        SpecScore s = new SpecScore(score, gc.GetCoordinates(v2), Spec.Enemy);
        return s;
    }

    Vector2Int LookForEnemies(Vector2Int pieceCoord)
    {
        int maxrow = pieceCoord.x + 2;
        int minrow = pieceCoord.x - 2;
        int maxcol = pieceCoord.y + 2;
        int mincol = pieceCoord.y - 2;

        for (int i = minrow; i <= maxrow; i++)
        {
            for (int j = mincol; j <= maxcol; j++)
            {
                if (i < 17 && j < 17 && i >= 0 && j >= 0)
                {
                    Vector2Int v2 = new Vector2Int((int)gc.grids2D[i, j].transform.position.x, (int)gc.grids2D[i, j].transform.position.z);

                    foreach (GridControl.GridSystem g in GridControl.GridClassList)
                    {
                        if (g.position == v2 && g.place == GridControl.Place.Occupied && !g.teamName.Equals(tag))
                        {
                            return v2;
                        }
                    }
                }
            }
        }
        return Vector2Int.left; // is off board
    }

    float GetEnemyScore(GameObject enemy, Vector2Int pieceCoordinates)
    {
        PieceMover pm = enemy.GetComponent<PieceMover>();
        if (pm != null)
        {
            float score = pm.health;
            score += Vector2Int.Distance(pieceCoordinates, gc.GetCoordinates(enemy));
            return score;
        }
        else
        {
            return float.MaxValue;
        }
        
    }

    SpecScore GetClosestGod(Player p, Vector2Int pieceCoordinates)
    {
        List<SpecScore> tempScores = new List<SpecScore>();
        List<Vector2Int> temp = new List<Vector2Int>();
        float max = float.MaxValue;
        float score = 0;
        foreach (Player player in UniversalSettings.players)
        {
            if (!p.Team.Equals(player.Team) && score < max)
            {
                temp.Add(player.GodCoordinates);
                score = Vector2Int.Distance(pieceCoordinates, player.GodCoordinates);
                max = score;
                SpecScore spec = new SpecScore(score, player.GodCoordinates, Spec.God);
                tempScores.Add(spec);
            }
        }
        if(tempScores.Count > 0)
        {
            return tempScores[0];
        }
        else
        {
            Debug.LogError("ärrör: Ingen gud hittad...");
            SpecScore bad = new SpecScore(score, Vector2Int.left, Spec.Useless);
            return bad;
        }
    }

    Direction GetDirection(List<Vector2Int> options, Vector2Int playerCoordinates)
    {
        float max = float.MaxValue;
        float cur;
        Direction dir = Direction.None;
        foreach (Vector2Int vector in options)
        {
            // sv Debug.LogError(dir + " " + playerCoordinates + " " + vector);
            cur = Vector2Int.Distance(playerCoordinates, vector);
            if (cur < max)
            {
                dir = CalculateDirection(playerCoordinates, vector);
                max = cur;
            }
        }
        return dir;
    }

    Direction CalculateDirection(Vector2Int playerCoord, Vector2Int oppCoord)
    {
        if (playerCoord.x < oppCoord.x && playerCoord.y == oppCoord.y)
        {
            return Direction.North;
        }
        else if (playerCoord.x < oppCoord.x && playerCoord.y < oppCoord.y)
        {
            return Direction.NorthEast;
        }
        else if (playerCoord.x == oppCoord.x && playerCoord.y < oppCoord.y)
        {
            return Direction.East;
        }
        else if (playerCoord.x > oppCoord.x && playerCoord.y < oppCoord.y)
        {
            return Direction.SouthEast;
        }
        else if (playerCoord.x > oppCoord.x && playerCoord.y == oppCoord.y)
        {
            return Direction.South;
        }
        else if (playerCoord.x > oppCoord.x && playerCoord.y > oppCoord.y)
        {
            return Direction.SouthWest;
        }
        else if (playerCoord.x == oppCoord.x && playerCoord.y > oppCoord.y)
        {
            return Direction.West;
        }
        else 
        {
            return Direction.NorthWest;
        }
    }

    
    void CheckMovePositions(GameObject soldier, Direction dir)
    {
        PieceMover pm = soldier.GetComponent<PieceMover>();
        if (!pm.god) // Gods movements is controlled from "Controller"
        {
            GridControl.Piece piece = pm.GetPiece();
            // 
            List<Vector2Int> possMovesCoordinates = movement.PossMoveLocations(soldier, piece.coordinates.x, piece.coordinates.y, soldier.tag, piece, true);
            Vector2Int bestMove = Vector2Int.left;
            //Debug.LogWarning(soldier.name + " moving..." + possMovesCoordinates.Count);
            foreach (Vector2Int possVector in possMovesCoordinates)
            {
                Vector2Int coord = gc.GetCoordinates(possVector);
                // sv Debug.Log(dir + ", calc: " + CalculateDirection(piece.coordinates, coord) + " " + coord + " " + piece.coordinates);
                if (CalculateDirection(piece.coordinates, coord) == dir ||
                    CalculateDirection(piece.coordinates, coord) == NextDirection(dir, true) ||
                    CalculateDirection(piece.coordinates, coord) == NextDirection(dir, false))
                {
                    //Debug.Log(NextDirection(dir, true) + " dir: " + CalculateDirection(piece.coordinates, possCoordinates) + " ned: " + NextDirection(dir, false));
                    bestMove = coord;
                }
            }
            if (bestMove != Vector2Int.left)
            {

                movement.ChangeMoveablePiece(soldier);
                MovePiece(Vector3Int.FloorToInt(soldier.transform.position), bestMove);
            }
        }
    }

    Direction NextDirection(Direction dir, bool clockWise)
    {
        switch (dir)
        {
            case Direction.NorthEast:
                if (clockWise)
                {
                    return Direction.North;
                }
                else
                {
                    return Direction.East;
                }
            case Direction.North:
                if (clockWise)
                {
                    return Direction.NorthWest;
                }
                else
                {
                    return Direction.NorthEast;
                }
            case Direction.NorthWest:
                if (clockWise)
                {
                    return Direction.West;
                }
                else
                {
                    return Direction.North;
                }
            case Direction.West:
                if (clockWise)
                {
                    return Direction.SouthWest;
                }
                else
                {
                    return Direction.NorthWest;
                }
            case Direction.East:
                if (clockWise)
                {
                    return Direction.NorthEast;
                }
                else
                {
                    return Direction.SouthEast;
                }
            case Direction.SouthEast:
                if (clockWise)
                {
                    return Direction.East;
                }
                else
                {
                    return Direction.South;
                }
            case Direction.South:
                if (clockWise)
                {
                    return Direction.SouthEast;
                }
                else
                {
                    return Direction.SouthWest;
                }
            case Direction.SouthWest:
                if (clockWise)
                {
                    return Direction.South;
                }
                else
                {
                    return Direction.West;
                }
            default:
                return Direction.None;
        }
    }

    void MovePiece(Vector3Int startVec, Vector2Int goalCoord)
    {
        // Vector2Int adjCoord = new Vector2Int(goalCoord.x - 1, goalCoord.y - 1);
        Vector3 goalVec = gc.GetPosition(goalCoord);

        // Debug.Log(startVec + " till: " + goalVec);
        movement.MoveAIPiece(startVec, goalVec);
    }
}

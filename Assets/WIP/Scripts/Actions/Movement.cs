using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public bool clearBoard = false;
    string moves = "Moves";
    GridControl gc;
    // Start is called before the first frame update
    void Awake()
    {
        gc = GridControl.GridControlSingleTon;
    }

    public bool animationMovingActive = true; // sv ändra till universalsettings?
    public bool moving = false;
    // Update is called once per frame
    void Update()
    {
        if (clearBoard)
        {
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(moves);
            foreach (GameObject item in gameObjects)
            {
                Destroy(item);
            }
            clearBoard = false;
        }
        if (moving)
        {
            clearBoard = true;
            MovingPiece(animationMovingActive);
        }
    }

    GameObject moveablePiece;

    public GameObject ChangeMoveablePiece(GameObject go)
    {
        moveablePiece = go;
        return moveablePiece;
    }
    Vector3 startPosition;

    

    Vector2 tempv2;
    public List<Vector2Int> PossMoveLocations(GameObject goPiece, int row, int col, string tag, GridControl.Piece piece, bool AI)
    {
        tempv2 = new Vector2(150, 99);
        moveablePiece = goPiece;
        startPosition = new Vector3((int)goPiece.transform.position.x, GameManager.pieceHeightY, (int)goPiece.transform.position.z);
        int maxrow, minrow, maxcol, mincol;
        List<Vector2Int> possMoves = new List<Vector2Int>();

        if (piece.NamePiece == GridControl.NamePiece.God || 
            (AI && (piece.NamePiece == GridControl.NamePiece.Archer || piece.NamePiece == GridControl.NamePiece.Marauder)))
        {
            maxrow = row + 1;
            minrow = row - 1;
            maxcol = col + 1;
            mincol = col - 1;
        }
        else if (piece.NamePiece == GridControl.NamePiece.Horsemen && !AI)
        {
            maxrow = row + 3;
            minrow = row - 3;
            maxcol = col + 3;
            mincol = col - 3;
        }
        else // if (!AI && (piece.NamePiece == GridControl.NamePiece.Archer || piece.NamePiece == GridControl.NamePiece.Marauder) || (AI && piece.NamePiece == GridControl.NamePiece.Horsemen))
        {
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
                    Vector2Int v2 = new Vector2Int((int)gc.grids2D[i, j].transform.position.x,
                                            (int)gc.grids2D[i, j].transform.position.z);
                    
                    foreach (GridControl.GridSystem g in GridControl.GridClassList)
                    {
                        if (g.position == v2 && g.place == GridControl.Place.Empty && 
                            (g.settlement == GridControl.Settlement.None || OwnCapital(v2, g.settlement, tag)))
                        {
                            possMoves.Add(Vector2Int.FloorToInt(v2));
                        }
                    }
                }
            }
        }

        return possMoves;
    }

    bool OwnCapital(Vector2Int loc, GridControl.Settlement settlement, string tag)
    {
        if (settlement == GridControl.Settlement.City)
        {
            foreach (GameObject c in gc.capitals)
            {
                if ((int)c.transform.position.x == loc.x && (int)c.transform.position.z == loc.y)
                {
                    Capital cap = c.GetComponent<Capital>();
                    if (cap.tagTeam == tag)
                    {
                        //Debug.LogWarning("true" + loc);
                        return true;
                    }
                }
            }
            
            return false;
        }
        else if (settlement == GridControl.Settlement.Village)
        {
            foreach (GameObject v in gc.villages)
            {
                //Debug.Log(v.transform.position + ", " + loc);
                if ((int)v.transform.position.x == loc.x && (int)v.transform.position.z == loc.y)
                {
                    Village vil = v.GetComponent<Village>();
                    if (vil.tagTeam == tag)
                    {
                        //Debug.LogWarning("true" + loc);
                        return true;
                    }
                }
            }
            return false;
        }
        //Debug.LogError("false" + loc);
        return false;
    }

    private Vector3 newPosition;
    // Time when the movement started.
    private float startTime;
    // Total distance between the markers.
    private float journeyLength;

    Vector2Int rowColDistance;
    Vector2Int startRowCol;
    Vector2Int moveRowCol;

    public List<GameObject> movedUnits = new List<GameObject>(); //Saves the moved units so they can be turned on again when a new turn starts

    // Without input on start position (from TouchGrid Script):
    public void MoveToPosition(Vector3Int goal)
    {
        newPosition = goal;

        foreach (KeyValuePair<Vector3Int, Vector2Int> item in gc.posToColRow)
        {
            if (item.Key.x == goal.x && item.Key.z == goal.z)
            {
                moveRowCol = item.Value;
            }
            else if (item.Key.x == startPosition.x && item.Key.z == startPosition.z)
            {
                startRowCol = item.Value;
            }
        }
        PieceMover pmy = moveablePiece.GetComponent<PieceMover>();
        pmy.movedThisTurn = true; //if full moves taken => doneturn
        movedUnits.Add(moveablePiece);

        rowColDistance = startRowCol - moveRowCol;
        int x = 0;
        int y = 0;
        if (rowColDistance.x <= rowColDistance.y && rowColDistance.y >= 0)
        {
            y = rowColDistance.y;
        }
        if (rowColDistance.x > rowColDistance.y && rowColDistance.x >= 0)
        {
            x = rowColDistance.x;
        }
        if (rowColDistance.x >= rowColDistance.y && rowColDistance.y < 0)
        {
            y = rowColDistance.y * -1;
        }
        if (rowColDistance.x < rowColDistance.y && rowColDistance.x < 0)
        {
            x = rowColDistance.x * -1;
        }

        if (x <= y)
        {
            pmy.movesTaken = y;
        }
        else
        {
            pmy.movesTaken = x;
        }

        startTime = Time.time;
        journeyLength = Vector3.Distance(startPosition, newPosition);
        moving = true;
    }
    public void MoveToPosition(Vector3Int start, Vector3 goal)
    {
        // Change values for this script instance for other methods etc:
        newPosition = goal; 
        startPosition = start;

        foreach (KeyValuePair<Vector3Int, Vector2Int> item in gc.posToColRow)
        {
            if (item.Key.x == goal.x && item.Key.z == goal.z)
            {
                moveRowCol = item.Value;
            }
            else if (item.Key.x == start.x && item.Key.z == start.z)
            {
                startRowCol = item.Value;
            }
        }
        PieceMover pmy = moveablePiece.GetComponent<PieceMover>();
        pmy.movedThisTurn = true; //if full moves taken => doneturn
        movedUnits.Add(moveablePiece);

        rowColDistance = startRowCol - moveRowCol;
        int x = 0;
        int y = 0;
        if (rowColDistance.x <= rowColDistance.y && rowColDistance.y >= 0)
        {
            y = rowColDistance.y;
        }
        if (rowColDistance.x > rowColDistance.y && rowColDistance.x >= 0)
        {
            x = rowColDistance.x;
        }
        if (rowColDistance.x >= rowColDistance.y && rowColDistance.y < 0)
        {
            y = rowColDistance.y * -1;
        }
        if (rowColDistance.x < rowColDistance.y && rowColDistance.x < 0)
        {
            x = rowColDistance.x * -1;
        }

        if (x <= y)
        {
            pmy.movesTaken = y;
        }
        else
        {
            pmy.movesTaken = x;
        }

        startTime = Time.time;
        journeyLength = Vector3.Distance(start, newPosition);
        moving = true;
    }

    readonly float speed = 10;

    void MovingPiece(bool animation)
    {
        if (animation)
        {
            // Distance moved equals elapsed time times speed..
            float distCovered = (Time.time - startTime) * speed;

            // Fraction of journey completed equals current distance divided by total distance.
            float fractionOfJourney = distCovered / journeyLength;
            // Set our position as a fraction of the distance between the markers.
            if (fractionOfJourney <= 1)
            {
                moveablePiece.transform.position = Vector3.Lerp(startPosition, newPosition, fractionOfJourney);
            }
            else
            {
                moveablePiece.transform.position = newPosition; //Final crucial push needed for future movement
                gc.ChangePositionSoldier(moveablePiece, GridControl.Place.Occupied, moveablePiece.tag);
                gc.MakeGridEmpty(Vector3Int.FloorToInt(startPosition), GridControl.Place.Empty);
                PieceMover pm = moveablePiece.GetComponent<PieceMover>();
                pm.GetPiece().coordinates = gc.GetCoordinates(moveablePiece);
                moving = false;
            }
        }
        else
        {
            moveablePiece.transform.position = newPosition; 
            gc.ChangePositionSoldier(moveablePiece, GridControl.Place.Occupied, moveablePiece.tag);
            gc.MakeGridEmpty(Vector3Int.FloorToInt(startPosition), GridControl.Place.Empty);
            PieceMover pm = moveablePiece.GetComponent<PieceMover>();
            pm.GetPiece().coordinates = gc.GetCoordinates(moveablePiece);
            //moving = false;
        }
    }

    public void MoveAIPiece(Vector3Int start, Vector3 goal)
    {
        // Debug.Log(moveablePiece.name + " till " + goal);
        moveablePiece.transform.position = goal;
        gc.ChangePositionSoldier(moveablePiece, GridControl.Place.Occupied, moveablePiece.tag);
        gc.MakeGridEmpty(start, GridControl.Place.Empty);
        PieceMover pm = moveablePiece.GetComponent<PieceMover>();
        pm.GetPiece().coordinates = gc.GetCoordinates(moveablePiece);
    }
}

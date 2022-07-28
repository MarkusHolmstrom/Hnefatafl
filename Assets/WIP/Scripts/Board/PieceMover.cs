using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieceMover : MonoBehaviour
{
    public GameObject darkLogo;

    public int movesTaken = 0;
    int maxMoves = 0; //Walking range
    public int GetMaxMoves()
    {
        return maxMoves;
    }
    readonly string grid = "IDGrid";

    public bool movePiece;
    bool chosen = false;
    

    GameManager manager;
    Movement movement;
    GameObject gameManager;

    public float health = 10;
    public float fullHealth;
    public float damage = 2;

    public float multiPlierHealth = 1;
    public float multiPlierDamage = 1;

    GridControl gridControl;
    public bool god = false;

    void Awake() //This needs to be Awake when loading a game, this information is needed for moving caps around
    {
        showOppInfo.SetActive(false);
        capLogo.SetActive(false);
        vilLogo.SetActive(false);
        gridControl = GridControl.GridControlSingleTon;
        healthbar.SetActive(false);

        gameManager = GameObject.FindGameObjectWithTag("GameController");
        manager = gameManager.GetComponent<GameManager>();
        movement = gameManager.GetComponent<Movement>();

        movePiece = true;
        FindPieceTypeAndSetDamageHealth(this.gameObject);
        cameraMain = GameObject.FindGameObjectWithTag("CamerHolder");
        //movesTaken = 0;

        if (name.Equals("Horsemen(Clone)") || name.Equals("Archer(Clone)"))
        {
            range = 2;
        }
        doneForTurn = true;
    }

    GameObject cameraMain;
    bool damaged = false;
    bool dead = false;

    public GameObject canvas, healthbar;
    public Slider healthSlider;

    public bool doneForTurn = true;

    public GameObject capLogo, vilLogo;
    private float villageHealth = 1.25f, capitalHealth = 1.5f;

    private void Update()
    {
        //if (transform.position == Vector3Int.zero)
        //{
        //    Debug.LogError("Här ska jag inte vara! " + name);
        //}
        //health = fullHealth * multiPlierHealth;
        //health *= multiPlierHealth; leder till oändlig gudhälsa....
        if (multiPlierHealth == capitalHealth)
        {
            capLogo.SetActive(true);
        }
        else if (multiPlierHealth == villageHealth)
        {
            vilLogo.SetActive(true);
        }
        else
        {
            capLogo.SetActive(false);
            vilLogo.SetActive(false);
        }

        canvas.transform.LookAt(canvas.transform.position + cameraMain.transform.rotation * Vector3.down, //down
                cameraMain.transform.rotation * Vector3.down); //down
        
        if (movement.moving && !dead)
        {
            chosen = false;
            gridControl.haloKeeper.transform.position = gridControl.startHalo;
        }
        if (chosen && !dead && !doneForTurn)
        {
            //Get the Screen positions of the object
            Vector2 positionOnScreen = Camera.main.WorldToViewportPoint(transform.position);
            //Get the Screen position of the mouse
            Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);
            //Get the angle between the points
            float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);
            transform.rotation = Quaternion.Euler(new Vector3(0f, -angle + 100, 0f));
        }
        if (health < fullHealth)
        {
            damaged = true;
            healthbar.SetActive(true);
            healthSlider.value = CalculateHealth(health, fullHealth);
        }
        else if (health >= fullHealth && damaged)
        {
            damaged = false;
            health = fullHealth;
            healthbar.SetActive(false);
        }
        if (health <= 0 && !dead)
        {
            DeathScene(gridControl.destroyInPieces);
        }
        if (!doneForTurn && maxMoves <= movesTaken) 
        {
            gridControl.ChangeRemainingMovesText(1);
            doneForTurn = true;
        }
        if (doneForTurn)
        {
            darkLogo.SetActive(true);
        }
    }

    float CalculateHealth(float curHealth, float maxHealth)
    {
        return curHealth / maxHealth;
    }
    void DeathScene(bool piecesInPieces)
    {
        gridControl.MakeGridEmpty(Vector3Int.FloorToInt(transform.position), GridControl.Place.Empty);
        if (gridControl.pieceLocations.ContainsKey(Vector3Int.FloorToInt(transform.position)))
        {
            gridControl.pieceLocations.Remove(Vector3Int.FloorToInt(transform.position));
        }
        //gridControl.UpdateBoard(gameObject, false, piece, true);
        
        if (piecesInPieces)
        {
            dead = true;
            Transform[] allChildren = GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                child.gameObject.transform.SetParent(null);// parent = null;
                Rigidbody rob = child.gameObject.AddComponent<Rigidbody>();
                rob.useGravity = true;
            }
        }
        else
        {
            if (transform.position.y < 100)
            {
                transform.position += new Vector3(0, 10 * Time.deltaTime, 0); // sv "Far till Valhöll!"
            }
            else
            {
                dead = true;
            }
        }
        gridControl.RemoveAttackPos(new Vector2Int((int)transform.position.x, (int)transform.position.z)); // sv funkar detta, rengör så stad kan attackeras efter pjäs där dött?
    }

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    private Vector3 startPos;
    private Vector2 startVec2;
    private Vector3 screenPoint;
    private Vector3 offset;
    private Vector3 curScreenPoint;
    public Vector3 curPosition;

    public bool movedThisTurn = false;
    public void OnMouseDown()
    {
        if (!movement.moving/* && !GridControl.AdaLovelace.attackAct*/ && !dead && !doneForTurn)
        {
            gridControl.turnPointer.transform.position = gridControl.turnStartHalo;
            //Debug.LogWarning("Aja baja!");
            startPos = transform.position;
            startVec2.x = startPos.x;
            startVec2.y = startPos.z;

            GetPosition(startVec2);
            gridControl.ViewOptions(this.gameObject, damaged, this.gameObject.tag, piece, movedThisTurn);
            gridControl.haloKeeper.transform.position = transform.position;
            gridControl.halo.enabled = true;

            screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
            ShowHealth();
            manager.SetCurDamage(damage);
            chosen = true;
        }
        //else 
        //{
        //    Debug.Log(movement.moving + " gridc: " + GridControl.AdaLovelace.attackAct +" död: "+ dead + " döne?: " + doneForTurn +"Aj! ja e klar!");
        //}
    }
    GridControl.Piece piece = new GridControl.Piece();
    GridControl.Piece FindPieceTypeAndSetDamageHealth(GameObject game)
    {
        piece.team = tag;
        piece.coordinates = gridControl.GetCoordinates(game); 
        if (game.name.Equals("Horsemen(Clone)"))
        {
            maxMoves = 3;
            health = UniversalSettings.horseHealth * multiPlierHealth;
            fullHealth = health;
            damage = UniversalSettings.horseDamage * multiPlierDamage;
            piece.NamePiece = GridControl.NamePiece.Horsemen;
            return piece;
        }
        else if (game.name.Equals("Marauder(Clone)"))
        {
            maxMoves = 2;
            health = UniversalSettings.marHealth * multiPlierHealth;
            fullHealth = health;
            damage = UniversalSettings.marDamage * multiPlierDamage;
            piece.NamePiece = GridControl.NamePiece.Marauder;
            return piece;
        }
        else if (game.name.Equals("Archer(Clone)"))
        {
            maxMoves = 2;
            health = UniversalSettings.archHealth * multiPlierHealth;
            fullHealth = health;
            damage = UniversalSettings.archDamage * multiPlierDamage;
            piece.NamePiece = GridControl.NamePiece.Archer;
            return piece;
        }
        else //if (god == true)
        {
            god = true;
            maxMoves = 1;
            health = UniversalSettings.godHealth * multiPlierHealth;
            fullHealth = health;
            damage = UniversalSettings.godDamage * multiPlierDamage;
            piece.NamePiece = GridControl.NamePiece.God;
            return piece;
        }
    }

    public GridControl.Piece GetPiece()
    {
        return piece;
    }

    public Vector2Int SetNewCoordinates()
    {
        piece.coordinates = gridControl.GetCoordinates(this.gameObject);
        return piece.coordinates;
    }

    private Vector2 endPos;
    public bool goodMove = false;
    int range = 1;

    public void ShowHealth()
    {
        gridControl.infoImage.SetActive(true); 
        Text text = gridControl.infoText; 
        text.text = piece.NamePiece + " (" + tag + ") Damage: " + damage + " Range: " + range + " Health: " + health + "/" + fullHealth;
    }                           

    private GameObject closest;

    public GameObject FindClosestTarget()
    {
        GameObject[] grids = GameObject.FindGameObjectsWithTag(grid);

        closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject grid in grids)
        {
            Vector3 diff = grid.transform.position - position;
            float curDistance = diff.sqrMagnitude;

            if (curDistance < distance)
            {
                closest = grid;
                distance = curDistance;
            }
        }
        return closest;
    }
    private Vector2 startVec;
    Vector2 GetPosition(Vector2 vector)
    {
        startVec = vector;
        return startVec;
    }

    public GameObject showOppInfo;
    public Text oppText;
    
    void OnMouseOver()
    {
        if (!tag.Equals(manager.GetCurrentTeam()))
        {
            showOppInfo.SetActive(true);
            //showOppInfo.transform.localScale = new Vector3(2, 3, 1);
            oppText.text = "Damage: " + manager.GetCurDamage() + "/" + health;
        }
    }

    void OnMouseExit()
    {
        showOppInfo.SetActive(false);
    }
    
}

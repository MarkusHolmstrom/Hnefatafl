using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchGrid : MonoBehaviour
{
    Movement movement;
    Attack attack;
    GameObject board;

    bool accessable = true;
    bool attackPrefab;
    // Start is called before the first frame update
    void Awake()
    {
        board = GameObject.FindGameObjectWithTag("GameController");
        movement = board.GetComponent<Movement>();
        attack = board.GetComponent<Attack>();

        if (tag.Equals("Attacks"))
        {
            attackPrefab = true;
        }
        else if (tag.Equals("Moves"))
        {
            attackPrefab = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseDown()          
    {
        Vector3 temp = new Vector3(transform.position.x, 3, transform.position.z);
        if (accessable && !attackPrefab)
        {
            movement.MoveToPosition(Vector3Int.FloorToInt(temp));
        }
        else if (accessable && attackPrefab)
        {
            attack.AttackOpponent(Vector3Int.FloorToInt(temp));
        }
    }
}

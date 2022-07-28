using UnityEngine;

public class SpawnClick : MonoBehaviour
{
    GameObject board;
    GameManager gm;
    bool chosen = false;
    // Start is called before the first frame update
    void Awake()
    {
        board = GameObject.FindGameObjectWithTag("GameController");
        gm = board.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    string respawn = "Respawn";
    public void OnMouseDown()
    {
        gm.SetNewSpawn(this.transform.position);
        //GameObject[] allSpawns = GameObject.FindGameObjectsWithTag(respawn);
        //foreach (GameObject g in allSpawns)
        //{
        //    g.GetComponent<SpawnClick>().MakeFalse();
        //}
        chosen = true;
    }

}

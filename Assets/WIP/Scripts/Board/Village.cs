using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Village : MonoBehaviour
{
    public float health = UniversalSettings.villageMaxHealth;
    private readonly float fullHealth = UniversalSettings.villageMaxHealth;
    public Vector2Int gridPosition;
    public string tagTeam;

    public GameObject smoke;

    ColourManager colourManager;
    GameManager gm;
    GameObject board;

    public Text nameText;

    // Start is called before the first frame update
    void Awake()
    {
        board = GameObject.FindGameObjectWithTag("GameController");
        colourManager = board.GetComponent<ColourManager>();
        gm = board.GetComponent<GameManager>();
        cameraMain = GameObject.FindGameObjectWithTag("CamerHolder");
        smoke.SetActive(false);
        gm.logos.Add(logotype);
    }


    bool damaged = false;
    GameObject cameraMain;
    public GameObject canvas, healthbar;
    public Slider healthSlider;

    // Update is called once per frame
    void Update()
    {
        canvas.transform.LookAt(canvas.transform.position + cameraMain.transform.rotation * Vector3.down, cameraMain.transform.rotation * Vector3.forward);

        if (damaged)
        {
            smoke.SetActive(true);
            damaged = false;
        }
        if (health < 0)
        {
            health = 0;
        }

        if (health < fullHealth)
        {
            damaged = true;
            healthbar.SetActive(true);
            healthSlider.value = CalculateHealth(health, fullHealth);
        }
        else if (health >= fullHealth)
        {
            health = fullHealth;
            healthbar.SetActive(false);
        }
    }

    public GameObject logotype;

    public void SetVariables(Vector2Int v2, string team)
    {
        gridPosition = v2;
        tagTeam = team;
        logotype.GetComponent<RawImage>().color = ColorLogo(tagTeam);
        nameText.text = "Village of " + tagTeam;
    }

    Color ColorLogo(string team)
    {
        if (team == gm.oden)
        {
            return colourManager.oden.color;
        }
        else if (team == gm.frigg)
        {
            return colourManager.frigg.color;
        }
        else if (team == gm.freja)
        {
            return colourManager.freja.color;
        }
        else if (team == gm.frej)
        {
            return colourManager.frej.color;
        }
        else if (team == gm.tor)
        {
            return colourManager.tor.color;
        }
        else if (team == gm.loke)
        {
            return colourManager.loke.color;
        }
        else if (team == gm.balder)
        {
            return colourManager.balder.color;
        }
        else if (team == gm.heimdall)
        {
            return colourManager.heimdall.color;
        }
        else if (team == gm.tyr)
        {
            return colourManager.tyr.color;
        }
        else
        {
            return colourManager.idun.color;
        }
    }

    float CalculateHealth(float curHealth, float maxHealth)
    {
        return curHealth / maxHealth;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Capital : MonoBehaviour
{
    public float health = UniversalSettings.capitalMaxHealth;
    private readonly float fullHealth = UniversalSettings.capitalMaxHealth;
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
        smoke.SetActive(false);
        board = GameObject.FindGameObjectWithTag("GameController");
        colourManager = board.GetComponent<ColourManager>();
        gm = board.GetComponent<GameManager>();
        cameraMain = GameObject.FindGameObjectWithTag("CamerHolder");
        gm.logos.Add(logotype);
    }

    GameObject cameraMain;
    public GameObject canvas, healthbar;
    public Slider healthSlider;

    bool damaged = false;
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
        nameText.text = "Capital of " + tagTeam;
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

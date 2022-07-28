using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Canvas GO
public class UniversalSettings : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject gmGO;

    public static int nrofPlayers;

    public static List<Player> players = new List<Player>();

    public static void RemovePlayer(string team)
    {
        foreach (Player player in players)
        {
            if (player.Team == team)
            {
                player.defeated = true;
            }
        }
        //List<Player> newSetPlayers = new List<Player>();
        //if (players.Count > 0)
        //{
        //    foreach (Player player in players)
        //    {
        //        if (player.Team != team)
        //        {
        //            newSetPlayers.Add(player);
        //        }
        //    }
        //}
        //players = newSetPlayers;
    }

    [SerializeField] public static bool destroyedDeathScenes = false;
    [SerializeField] public static bool animatedMovement = true;
    [SerializeField] public static bool onlyCapitalDefeats = false;

    [SerializeField] public static bool villageConstruction = true;

    [SerializeField] public static bool limitedUnitMoves = true;
    [SerializeField] public static int nrUnitMoves = 4;

    [SerializeField] public static bool moveAndAttackSameTurn = true;

    [SerializeField] public static bool noAttackFirstRound = false;
    [SerializeField] public static bool regenaration = true;
    [SerializeField] public static int regenerationRate = 8;
    [SerializeField] public static bool limitedRounds = true;
    [SerializeField] public static int maxRounds = 100;


    // Piece information:
    public static int marDamage = 4;
    public static int archDamage = 3;
    public static int horseDamage = 3;
    public static int godDamage = 7;

    public static int marHealth = 10;
    public static int archHealth = 8;
    public static int horseHealth = 6;
    public static int godHealth = 15;

    // Village/capital info:
    public static int villageMaxHealth = 10;
    public static int capitalMaxHealth = 20;

    // Start is called before the first frame update
    void Awake()
    {
        gmGO = GameObject.FindGameObjectWithTag("GameController");
        gameManager = gmGO.GetComponent<GameManager>();
        gameManager.round = maxRounds;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetNumberOfPlayers(int nr)
    {
        nrofPlayers = nr;
    }

    public void DestroyDeathScenes()
    {
        destroyedDeathScenes = !destroyedDeathScenes;
    }

    public void AnimatedMovement()
    {
        animatedMovement = !animatedMovement;
    }

    public void OnlyCapitalDefeat()
    {
        onlyCapitalDefeats = !onlyCapitalDefeats;
    }

    public void PossVillageConstruction()
    {
        villageConstruction = !villageConstruction;
    }

    public void LimitMovement()
    {
        limitedUnitMoves = !limitedUnitMoves;
    }

    public void NoAttackFirst()
    {
        noAttackFirstRound = !noAttackFirstRound;
    }

    int formerRate = 8;
    public void Regeneration()
    {
        if (regenerationRate != 0)
        {
            formerRate = regenerationRate;
        }
        
        regenaration = !regenaration;
        if (!regenaration)
        {
            regenerationRate = 0;
        }
        else
        {
            regenerationRate = formerRate;
        }
    }

    public void LimitRounds()
    {
        limitedRounds = !limitedRounds;
    }

    public void NumberRounds(int setMaxRounds)
    {
        if (limitedRounds)
        {
            if (setMaxRounds == 0)
            {
                maxRounds = 0;
                limitedRounds = false;
            }
            else if (setMaxRounds == 1)
            {
                maxRounds = 50;
            }
            else if (setMaxRounds == 2)
            {
                maxRounds = 100;
            }
            else if (setMaxRounds == 3)
            {
                maxRounds = 250;
            }
            else if (setMaxRounds == 4)
            {
                maxRounds = 500;
            }
            else if (setMaxRounds == 5)
            {
                maxRounds = 1000;
            }
        }
        else
        {
            maxRounds = 0;
        }
        gameManager.round = maxRounds;
        //Debug.Log(maxRounds);
    }

    public void SolderGenerate(int rate)
    {
        if (regenaration)
        {
            if (rate == 0)
            {
                regenerationRate = 0;
                regenaration = false;
            }
            else if (rate == 1)
            {
                regenerationRate = 4;
            }
            else if (rate == 2)
            {
                regenerationRate = 6;
            }
            else if (rate == 3)
            {
                regenerationRate = 8;
            }
            else if (rate == 4)
            {
                regenerationRate = 10;
            }
            else if (rate == 5)
            {
                regenerationRate = 12;
            }
        }
        else
        {
            regenerationRate = 0;
        }
    }
}


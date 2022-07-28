using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IPlayer 
{
    string Team { get; set; }
    int Order { get; set; } //Starts from 1
    int Regeneration { get; set; }
    List<GameObject> Pieces { get; set; }
    Vector2Int GodCoordinates { get; set; }
    Vector2Int CapitalPosition { get; set; }
    //bool AI { get; set; }
    float Score { get; set; }
}

public class Player : IPlayer
{
    public string Team { get; set; }
    public int Order { get; set; }
    public int Regeneration { get; set; }
    public List<GameObject> Pieces { get; set; }
    public Vector2Int GodCoordinates { get; set; }
    public Vector2Int CapitalPosition { get; set; }
    //public bool AI { get; set; }
    public float Score { get; set; }
    public bool defeated = false;
}

public class StartGame : MonoBehaviour
{
    public static StartGame _ada;
    public static StartGame Ada
    {
        get
        {
            if (_ada == null)
            {
                _ada = GameObject.FindGameObjectWithTag("GameController").GetComponent<StartGame>();
                if (_ada == null)
                {
                    Debug.LogWarning("ada is null");
                    GameObject b = new GameObject("Board");
                    _ada = b.AddComponent<StartGame>();
                }
            }
            return _ada;
        }
    }

    public GameManager gameManager;
    public GameObject gmGO;

    //int nrPlayers; använd universalsettings ist
    public int turn;

    public int moves = 0;
    public const int maxMoves = 4;

    public GameObject mainMenu, gameSettings, settings;

    public MenuManager mm;
    [SerializeField]
    GameObject menuCanvas;
    [SerializeField]
    List<string> teams = new List<string>();
    [SerializeField] GameObject[] togglerinos;

    readonly string onlyAIWarning = "Only AI controlled players have been chosen, please assign atleast one team as player controlled.";

    // Start is called before the first frame update
    void Awake()
    {
        if (tag != "GameController")
        {
            Debug.Log(tag + " : " + name);
            Destroy(this);
        }
        ToggleCheck();
        menuCanvas = GameObject.FindGameObjectWithTag("Canvas");
        mm = menuCanvas.GetComponent<MenuManager>();
        gmGO = GameObject.FindGameObjectWithTag("GameController");
        gameManager = gmGO.GetComponent<GameManager>();
        //nrPlayers = 2;
        //universalSettings = UniversalSettings.USLovelace;
        teams.Clear();
        AddTeams();
        //nrPlayers = mm.nrOfActivatedPlayers;
        //mainMenu.SetActive(true);
        //gameSettings.SetActive(false);
        //settings.SetActive(false);

        //players = CreateSetOfPlayers(nrPlayers);
    }

    List<string> AddTeams()
    {
        teams.Add("Oden");
        teams.Add("Frigg");
        teams.Add("Tor");
        teams.Add("Frej");
        teams.Add("Freja");
        teams.Add("Balder");
        teams.Add("Loke");
        teams.Add("Heimdall");
        teams.Add("Tyr");
        teams.Add("Idun");
        return teams;
    }

    List<Player> CreateSetOfPlayers(int nrPlayas)
    {
        List<Player> temp = new List<Player>();
        for (int i = 1; i < nrPlayas + 1; i++)
        {
            Player newPlayer = new Player
            {
                Order = i, Regeneration = 0
            };
            temp.Add(newPlayer);
        }
        return temp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ChangeMainMenu(bool open)
    {
        mainMenu.SetActive(open);
    }

    public void ChangeGameSettingsMenu(bool open)
    {
        if (gameSettings == null)
        {
            gameSettings = GameObject.Find("GameSettings");
        }
        if (gameSettings != null)
        {
            gameSettings.SetActive(open);
        }
        else
        {
            Debug.LogWarning("Ingen ruta hittades...");
        }
    }

    public void ChangeSettingsMenu(bool open)
    {
        settings.SetActive(open);
    }

    public void ToggleChanger(int i) //Denna behövs ju inte, AI ändras iom togglerutan!!!
    {

    }

    void ToggleCheck()
    {
        togglerinos = GameObject.FindGameObjectsWithTag("Toggle");
        //Debug.LogWarning(togglerinos.Length);
        for (int i = 0; i < togglerinos.Length; i++)
        {
            aIToggles[i] = togglerinos[i].GetComponent<Toggle>();
            //Debug.Log(i + ", " + aIToggles[i].isOn);
        }
    }

    [SerializeField]
    Toggle[] aIToggles = new Toggle[9];

    public void SetNumberOfPlayers(int nr)
    {
        //tempPlayers.Clear();
        UniversalSettings.nrofPlayers = nr;
        //UniversalSettings.players = CreateSetOfPlayers(UniversalSettings.nrofPlayers);
    }

    public void GameStart() //Start button activation...
    {
        
        if (UniversalSettings.nrofPlayers > UniversalSettings.players.Count)
        {
            CheckPlayerTeams(UniversalSettings.nrofPlayers);
            //UniversalSettings.players = CreateSetOfPlayers(UniversalSettings.nrofPlayers); //add more players...
        }
        else if (UniversalSettings.nrofPlayers < UniversalSettings.players.Count)
        {
            RemovePlayers(UniversalSettings.nrofPlayers);
        }
        
        //Debug.LogWarning("Starting... players: " + UniversalSettings.players.Count + " teams remaining; " + teams.Count + " check ai: " + CheckAI(UniversalSettings.nrofPlayers));
        
        if (!CheckAI(UniversalSettings.nrofPlayers))
        {
            //Debug.LogWarning("Thi  hi");
            menuCanvas = GameObject.FindGameObjectWithTag("Canvas");
            mm = menuCanvas.GetComponent<MenuManager>();
            mm.InfoScreen(onlyAIWarning);
            //Warning about only ai
        }
        else
        {
            foreach (Player player in UniversalSettings.players)
            {
                //player.AI = aIToggles[player.Order - 1].isOn; // Sorts the AI for the Player classes
                //Debug.LogWarning(player.Team + ", " + player.Order + " AI: " + player.AI);
            }
            //gmGO = GameObject.FindGameObjectWithTag("GameController");
            //gameManager = gmGO.GetComponent<GameManager>();
            gameManager.SetUpGame(UniversalSettings.nrofPlayers);
            ChangeGameSettingsMenu(false);
            //Debug.LogWarning("Thi  ho");
        }
    }
    List<Player> tempPlayers = new List<Player>();
    int orderCounter = 0;

    void CheckPlayerTeams(int nrTeams)
    {
        teams.Clear();
        teams = AddTeams();
        for (int i = 0; i < teams.Count; i++)
        {
            foreach (string playerName in chosenPlayers)
            {
                //Debug.Log(teams[i] + ", " + playerName);
                if (teams[i].Equals(playerName))
                {
                    //Debug.LogError(teams[i] + ", " + playerName);
                    teams.Remove(playerName);
                }
            }
            
        }
        if (nrTeams != UniversalSettings.players.Count)
        {
            int len = nrTeams - UniversalSettings.players.Count; // Needs to be saved cause players.count will change
            orderCounter = 0;
            for (int i = 0; i < len; i++)
            {
                orderCounter++;
                string temp = RandomTeam();
                //int ord = i;// + 1;
                List<int> orders = GetOrders();
                foreach (int o in orders)
                {
                    if (o == orderCounter)// || ord == 0)
                    {
                        //Debug.LogError(orderCounter + " " + temp);
                        orderCounter++; //= 2;
                    }
                }
                Player p = new Player { Order = orderCounter, Team = temp }; 
                teams.Remove(temp);
                UniversalSettings.players.Add(p);
                //Debug.LogWarning(temp + ", " + p.Order);
            }
        }
        else
        {
            foreach (Player player in UniversalSettings.players)
            {
                if (player.Team == null)
                {
                    player.Team = RandomTeam();
                    teams.Remove(player.Team);
                }
                //
            }
        }
    }
    public string RandomTeam()
    {
        string nr = teams[Random.Range(0, teams.Count)];
        //Debug.LogError(teams.Count + ", " + nr);
        foreach (Player player in UniversalSettings.players)
        {
            if (player.Team == nr)
            {
                teams.Remove(nr);
                RandomTeam();
            }
        }
        return nr;
    }

    List<int> GetOrders()
    {
        List<int> temp = new List<int>();
        foreach (Player player in UniversalSettings.players)
        {
            if (player.Order != 0)
            {
                //Debug.LogError(player.Order + " :from: " + player.Team);
                temp.Add(player.Order);
            }
        }
        return temp;
    }

    //List<bool> aIActives = new List<bool>();

    bool CheckAI(int nr)
    {
        ToggleCheck();
        for (int i = 0; i < nr; i++)
        {
            if (!aIToggles[i].isOn)
            {
                return true;
            }
        }
        return false;
    }

    void RemovePlayers(int nr)
    {
        ToggleCheck();
        tempPlayers.Clear();
        foreach (Player player in UniversalSettings.players)
        {
            tempPlayers.Add(player);
            if (player.Order > nr)
            {
                tempPlayers.Remove(player);
            }
        }
        UniversalSettings.players = tempPlayers;
    }

    //List<Player> players = new List<Player>();
    List<string> chosenPlayers = new List<string>();
    int tempOrder;
    public Dropdown teamDrop;
    //int order = 0;

    public void AssignTeam(int order) //Accessed from dropdown, depending on team
    {
        //Debug.Log(order);
        tempOrder = order;
    }

    bool newRegPlayer = true;
    // 
    public void FindTeam(int dropdownIndex) //Accessed from dropdown, depending on dropdown value
    {
        newRegPlayer = true;
        Player player = new Player
        {
            Team = GetTag(dropdownIndex),
            Order = tempOrder
        };
        //Debug.Log(teams.Count + " playas: " + players.Count);
        foreach (Player p in UniversalSettings.players)
        {
            //Debug.Log(p.Team + " : "  + p.Order + ", temp: " + tempOrder);
            if (p.Order == tempOrder)
            {
                p.Team = GetTag(dropdownIndex);
                chosenPlayers.Add(p.Team);
                newRegPlayer = false;
                //Debug.Log(p.Team);
            }
        }
        //if (!OrderAlreadyExists(players, tempOrder))
        //{

        //}
        if (newRegPlayer)
        {
            UniversalSettings.players.Add(player);
            //Debug.Log(player.Team + " p åplats: " + player.Order);
        }
        //players.Add(player);
        //Debug.LogError(tempOrder + ", " + dropdownIndex + ", " + UniversalSettings.players.Count);
    }

    bool match = false;
    Player tempPlayer;

    bool OrderAlreadyExists(List<Player> playas, int newOrder)
    {
        match = false;
        foreach (Player item in playas)
        {
            if (item.Order == newOrder)
            {
                match = true;
                tempPlayer = item;
            }
        }
        if (match)
        {
            playas.Remove(tempPlayer);
            return true;
        }
        return false;
    }

    string GetTag(int order)
    {
        if (order == 1)
        {
            return "Oden";
        }
        else if (order == 2)
        {
            return "Tor";
        }
        else if (order == 3)
        {
            return "Frigg";
        }
        else if (order == 4)
        {
            return "Frej";
        }
        else if (order == 5)
        {
            return "Freja";
        }
        else if (order == 6)
        {
            return "Loke";
        }
        else if (order == 7)
        {
            return "Balder";
        }
        else if (order == 8)
        {
            return "Heimdall";
        }
        else if (order == 9)
        {
            return "Tyr";
        }
        else if (order == 10)
        {
            return "Idun";
        }
        else
        {
            return null;
        }
    }

    List<Vector2Int> GetPositions(string tagName)
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag(tagName);
        if (gos.Length > 0)
        {
            List<Vector2Int> tempList = new List<Vector2Int>();
            
            foreach (GameObject g in gos)
            {
                foreach (KeyValuePair<Vector3Int, Vector2Int> item in GridControl.GridControlSingleTon.posToColRow)
                {
                    if (item.Key.x == (int)g.transform.position.x && item.Key.z == (int)g.transform.position.z)
                    {
                        Vector2Int v2 = item.Value;
                        tempList.Add(v2);
                    }
                }
            }
            return tempList;
        }
        else
        {
            return null;
        }
    }
    
}

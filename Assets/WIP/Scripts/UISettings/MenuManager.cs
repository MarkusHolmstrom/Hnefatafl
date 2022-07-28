using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Canvas GO
public class MenuManager : MonoBehaviour
{
    
    //public InputField inputField;
    int nrOfActivatedPlayers = 2;

    public GameObject[] teamDropDowns;
    List<GameObject> tempDropDowns = new List<GameObject>();

    //GameManager gameManager;
    //GameObject gmGO;

    bool showing = false;

    
    // Start is called before the first frame update
    void Awake()
    {
        //gmGO = GameObject.FindGameObjectWithTag("GameController");
        //gameManager = gmGO.GetComponent<GameManager>();
        ActivateTeamDrops(teamDropDowns.Length);
        showing = true;
        infoScreen.SetActive(false);
        ActivateTeamDrops(nrOfActivatedPlayers);
    }


    public void ActivateTeamDrops(int teams)
    {
        //PlayerTurn.Ada.nrPlayers
        nrOfActivatedPlayers = teams;
        if (tempDropDowns.Count > 0)
        {
            foreach (var item in tempDropDowns)
            {
                item.SetActive(false);
            }
            tempDropDowns.Clear();
        }
        for (int i = 0; i < teams; i++)
        {
            tempDropDowns.Add(teamDropDowns[i]);
            teamDropDowns[i].SetActive(showing);
        }
    }

    

    bool chosenNr = false;
    // Update is called once per frame
    void Update()
    {
        //if (chosenNr && int.Parse(inputField.text) >= 2 && int.Parse(inputField.text) <= 9)
        //{
        //    nrTeams = int.Parse(inputField.text);
        //    ActivateTeamDrops(teamDropDowns.Length, false);
        //    ActivateTeamDrops(nrTeams, true);
        //}
        
    }

    public GameObject infoScreen;
    public Text infoText;

    public void InfoScreen(string message)
    {
        infoScreen.SetActive(true);
        infoText.text = message;
    }

    public void CloseInfoScreen()
    {
        infoScreen.SetActive(false);
    }
}

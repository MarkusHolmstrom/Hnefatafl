using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Programmer
{
    private string name;
    private int age;
    private string lookingFor;
    public bool desperate;

    public Programmer(string name, int age, string lookingFor)
    {
        this.name = name;
        this.age = age;
        this.lookingFor = lookingFor;
    }
}

public class MarkusCreator : MonoBehaviour
{
    void Start()
    {
        Programmer markusHolmstrom = new Programmer("Markus Holmström", 28, "Internship");
        //markusHolmstrom.desperate = ;
    }

    void Update()
    {
        
    }
}

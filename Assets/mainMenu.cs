using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    List<string> lvl = new List<String>();


    // Use this for initialization
    void Start()
    {
        Addlevel("Level1_TheOffice");
    }

    // Update is called once per frame
    void Update()
    {
        //quit();
        //pauseGame();
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

    // quit the game
    public void quit()
    {
        Application.Quit();
    }

    public void pauseGame()
    {

        //nput.GetButton("PAUSE");

    }

    //Adds level object to the list of levels.
    private void Addlevel(String lvlName)
    {
        lvl.Add(lvlName);
    }

    public void LevelPicker(int index)
    {
        if (index > lvl.Count)
            print("Cant do");
        else
            SceneManager.LoadScene(lvl[index]);

        
    }

}


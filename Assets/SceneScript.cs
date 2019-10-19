using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))//A
        {
            Debug.Log("Game");
            SceneLoad("Game");
        }

        if (Input.GetKeyDown(KeyCode.S))//S
        {
           Debug.Log("FishingShip");
           SceneLoad("FishingShip");
        }

        if (Input.GetKeyDown(KeyCode.D))//D
        {
            Debug.Log("STore");
            SceneLoad("Store");   
        }

        if (Input.GetKeyDown(KeyCode.F))//F
        {
            Debug.Log("Title");
            SceneLoad("Title");
        }
    }

    public void SceneLoad(string scene)
    {
        SceneManager.LoadScene(scene);
        Debug.Log("SceneLoad");
    }

}

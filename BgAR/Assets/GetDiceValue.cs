using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetDiceValue : MonoBehaviour
{
    // Start is called before the first frame update
    private GameManager gameManager;
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    private void OnTriggerEnter(Collider other)
    {
        int diceValue = 0;
        switch (gameObject.name)
        {
            case "1":
                diceValue = 1;
                break;
            case "2":
                diceValue = 2;
                break;
            case "3":
                diceValue = 3;
                break;
            case "4":
                diceValue = 4;
                break;
            case "5":
                diceValue = 5;
                break;
            case "6":
                diceValue = 6;
                break;
        }
        Debug.Log(gameObject.name + " " + diceValue);
        gameManager.IncreaseTotalDiceValue(diceValue);
    }
}

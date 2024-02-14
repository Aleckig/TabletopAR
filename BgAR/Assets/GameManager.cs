using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject AttachPoints;
    private List<Transform> attachPointsList = new ();
    private int totalDiceValue = 0;
    public GameObject player;
    public int currentSpot;
    void Start()
    {
        currentSpot = 0;
        foreach (Transform point in AttachPoints.transform)
        {
            attachPointsList.Add(point);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(totalDiceValue);
    }

    public void IncreaseTotalDiceValue()
    {
        IncreaseTotalDiceValue(-totalDiceValue);
    }
    public void IncreaseTotalDiceValue(int value)
    {
        totalDiceValue += value;
    }
    public void StartPlayerMovement()
    {
        StartCoroutine(StartMove());
    }
    private IEnumerator StartMove()
    {
        Vector3 currentPos = player.transform.position;
        Vector3 startingPosition = player.transform.position;
        float totalMovementTime = 0.3f;
        float currentMovementTime = 0f;
        yield return new WaitForSeconds(0.5f);
        //foreach (Transform attachPoint in attachPointsList.GetRange(0,5))
        //{
        //    Debug.Log(attachPoint.position);
        //}
        //foreach (Transform attachPoint in attachPointsList.GetRange(1, totalDiceValue))

        //attachPointsList.GetRange(currentSpot, totalDiceValue + currentSpot + 1).Count

        for (int i=currentSpot+1; i < currentSpot + totalDiceValue + 1; i++)
        {
            if (i > 39)
            {
                i -= 40;
                totalDiceValue = totalDiceValue - (40 - currentSpot);
                currentSpot = 0;
            }
            Debug.Log("Value of i is" + i);
            while (Vector3.Distance(currentPos, attachPointsList[i].position) > 0)
            {
                currentMovementTime += Time.deltaTime;
                player.transform.position = Vector3.Lerp(startingPosition, attachPointsList[i].position, currentMovementTime / totalMovementTime);
                currentPos = player.transform.position;
                yield return null;
            }
            startingPosition = player.transform.position;
            currentMovementTime = 0f;
            yield return new WaitForSeconds(0.03f);
            Debug.Log("Coroutine was called");
            Debug.Log("Total dice value is" + totalDiceValue);
        }
        currentSpot += totalDiceValue;
        //if (currentSpot > 40)
        //{
        //    currentSpot -= 40;
        //}
    }
}

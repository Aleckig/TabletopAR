using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject AttachPoints;
    private List<Transform> attachPointsList = new List<Transform>();
    private int totalDiceValue = 0;
    public GameObject player;
    public int currentSpot;
    public GameObject buyButton;
    [SerializeField] private ThrowDice throwDice;
    public float bounceHeight = 1f;
    public float bounceFrequency = 2f;

    void Start()
    {
        currentSpot = 0;
        foreach (Transform point in AttachPoints.transform)
        {
            attachPointsList.Add(point);
        }
    }

    void Update()
    {
        // Debug.Log(totalDiceValue);
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
        IncreaseTotalDiceValue();
        throwDice.Throw();
        StartCoroutine(StartMove());
    }

    private IEnumerator StartMove()
    {
        Vector3 currentPos = player.transform.position;

        yield return new WaitForSeconds(0.5f);

        for (int i = currentSpot + 1; i < currentSpot + totalDiceValue + 1; i++)
        {
            if (i > 39)
            {
                i -= 40;
                totalDiceValue = totalDiceValue - (40 - currentSpot);
                currentSpot = 0;
            }

            Vector3 startingPosition = player.transform.position;
            Vector3 targetPosition = attachPointsList[i].position;

            float currentMovementTime = 0f;
            float totalMovementTime = 0.3f; // You need to define totalMovementTime

            while (currentMovementTime < totalMovementTime)
            {
                currentMovementTime += Time.deltaTime;

                float bounceT = Mathf.Sin(currentMovementTime * Mathf.PI * bounceFrequency / totalMovementTime);
                Vector3 intermediatePos = Vector3.Lerp(startingPosition, targetPosition, currentMovementTime / totalMovementTime);
                intermediatePos.y += Mathf.Abs(bounceT) * bounceHeight;

                player.transform.position = intermediatePos;
                yield return null;
            }

            yield return new WaitForSeconds(0.03f);
        }

        currentSpot += totalDiceValue;
    }

    Vector3 BezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;

        return p;
    }
}

   


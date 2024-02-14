using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject AttachPoints;
    private List<Transform> attachPointsList = new ();
    private int totalDiceValue = 0;
    public GameObject player;
    public int currentSpot;
    public GameObject buyButton;
    [SerializeField] private ThrowDice throwDice;
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
        //Debug.Log(totalDiceValue);
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
        Vector3 startingPosition = player.transform.position;
        float totalMovementTime = 0.3f;
        float currentMovementTime = 0f;
        yield return new WaitForSeconds(0.5f);

        for (int i = currentSpot + 1; i < currentSpot + totalDiceValue + 1; i++)
        {
            if (i > 39)
            {
                i -= 40;
                totalDiceValue = totalDiceValue - (40 - currentSpot);
                currentSpot = 0;
            }

            while (Vector3.Distance(currentPos, attachPointsList[i].position) > 0)
            {
                currentMovementTime += Time.deltaTime;

                // Use BezierCurve for bouncing movement
                float bounceT = Mathf.PingPong(currentMovementTime, 1f);
                Vector3 targetPos = BezierCurve(startingPosition, startingPosition + Vector3.up * 2f, attachPointsList[i].position, bounceT);

                player.transform.position = Vector3.Lerp(startingPosition, targetPos, currentMovementTime / totalMovementTime);
                currentPos = player.transform.position;
                yield return null;
            }

            startingPosition = player.transform.position;
            currentMovementTime = 0f;
            yield return new WaitForSeconds(0.03f);
        }

        currentSpot += totalDiceValue;
    }

    // ... (existing code)

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

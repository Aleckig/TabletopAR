using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowDice : MonoBehaviour
{

    Rigidbody m_Rigidbody;
    Rigidbody m_Rigidbody2;
    [SerializeField] GameObject dice1;
    [SerializeField] GameObject dice2;
    [SerializeField] GameObject button;
    [SerializeField] Transform diceAnchor1;
    [SerializeField] Transform diceAnchor2;
    public bool canThrow;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = dice1.GetComponent<Rigidbody>();
        m_Rigidbody2 = dice2.GetComponent<Rigidbody>();
        canThrow = true;
    }

    public void Throw()
    {
        if (canThrow == true) {
            dice1.SetActive(true);
            dice2.SetActive(true);
            m_Rigidbody.AddForce(transform.up * 100f);
            m_Rigidbody2.AddForce(transform.up * 100f);
            m_Rigidbody.AddTorque(RandomXYZ());
            m_Rigidbody2.AddTorque(RandomXYZ());
            StartCoroutine(StartRoll());
        }
    }

    private Vector3 RandomXYZ()
    {
        float x = 0, y = 0, z = 0;

        x = Random.Range(0f, 360f);
        y = Random.Range(0f, 360f);
        z = Random.Range(0f, 360f);

        return new Vector3(x, y, z);
    }

    private IEnumerator StartRoll()
    {
        canThrow = false;
        button.SetActive(false);
        yield return new WaitForSeconds(4f);
        dice1.SetActive(false);
        dice2.SetActive(false);
        dice1.transform.position = diceAnchor1.position;
        dice2.transform.position = diceAnchor2.position;
        canThrow = true;
        button.SetActive(true);
        //Reset position of dices
        yield break;
    }
}

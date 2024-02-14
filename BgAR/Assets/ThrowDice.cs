using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowDice : MonoBehaviour
{

    Rigidbody m_Rigidbody;
    Rigidbody m_Rigidbody2;
    [SerializeField] GameObject dice1;
    [SerializeField] GameObject dice2;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = dice1.GetComponent<Rigidbody>();
        m_Rigidbody2 = dice2.GetComponent<Rigidbody>();
    }

    public void Throw()
    {
        dice1.SetActive(true);
        dice2.SetActive(true);

        m_Rigidbody.AddForce(transform.up * 100f);
        m_Rigidbody2.AddForce(transform.up * 100f);
        m_Rigidbody.AddTorque(RandomXYZ());
        m_Rigidbody2.AddTorque(RandomXYZ());
        StartCoroutine(StartRoll());
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
        yield return new WaitForSeconds(3f);
        dice1.SetActive(false);
        dice2.SetActive(false);
        dice1.transform.position = new Vector3(0.118799999f, 0.0189999994f, -0.0186000001f);
        dice2.transform.position = new Vector3(0.195999995f, 0.0218000002f, -0.0155999996f);
        //Reset position of dices
        yield break;
    }
}

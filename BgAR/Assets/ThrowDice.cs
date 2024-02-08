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


        m_Rigidbody.AddForce(-transform.forward * 300f);
        m_Rigidbody2.AddForce(-transform.forward * 300f);
        m_Rigidbody.AddTorque(RandomXYZ());
        m_Rigidbody2.AddTorque(RandomXYZ());
    }

    private Vector3 RandomXYZ()
    {
        float x = 0, y = 0, z = 0;

        x = Random.Range(0f, 360f);
        y = Random.Range(0f, 360f);
        z = Random.Range(0f, 360f);

        return new Vector3(x, y, z);
    }
}

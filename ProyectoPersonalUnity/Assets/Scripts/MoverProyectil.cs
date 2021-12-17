using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverProyectil : MonoBehaviour
{
    private Transform miTransform;
    public int velocidad;
    public Vector3 _velocidad;
    public Transform posicionInicial;
    public Quaternion posDisparo;

    // Start is called before the first frame update
    void Start()
    {
        miTransform = this.transform;
        posicionInicial = GameObject.Find("PosProyectil").transform;
        posDisparo = GameObject.Find("Disparo").transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(posDisparo + "sd");
        if(posDisparo.z == 0.0)
        {
            Debug.Log("ARRIBA");
        miTransform.Translate(Vector3.up * velocidad * Time.deltaTime);
        } else if(posDisparo.z == -0.7)
        {
            Debug.Log("Derecha");
            miTransform.Translate(Vector3.right * velocidad * Time.deltaTime);
        }
        else if (posDisparo.z == -1.0 || posDisparo.z == 1.0)
        {
            Debug.Log("ABAJO");
            miTransform.Translate(Vector3.down * velocidad * Time.deltaTime);
        }
        else if (posDisparo.z == 0.7)
        {
            Debug.Log("IZQUIERDA");
            miTransform.Translate(Vector3.left * velocidad * Time.deltaTime);
        }
    }

    public void Reiniciar()
    {
        this.gameObject.SetActive(false);
        miTransform.position = this.posicionInicial.position;
        GameObject.Find("PosProyectil").GetComponent<Proyectil>().AnadirProyectil(this.gameObject);
    }
}

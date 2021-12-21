using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoverProyectil : MonoBehaviour
{
    private Transform miTransform;
    public int velocidad;
    public Vector3 _velocidad;
    public Transform posicionInicial;
    public Quaternion posDisparo;
    public GameObject Disparo1;
    public static int puntuacion =1;


    // Start is called before the first frame update
    void Start()
    {
        miTransform = this.transform;
        posicionInicial = GameObject.Find("PosProyectil").transform;
        posDisparo = GameObject.Find("Disparo1").transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Disparo1.transform.position);
        miTransform.Translate(Vector3.up * velocidad * Time.deltaTime);

        /*
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
        */
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag.Equals("Muro"))
        {
            Debug.Log("CHOQUE MURO");
            Reiniciar();
        }
        else if (collision.transform.tag.Equals("Jugador1"))
        {
            Debug.Log("JUGADOR1");
         
            Reiniciar();
        }
        else if (collision.transform.tag.Equals("Jugador2"))
        {
            
            GameObject.Find("Marcador1").GetComponent<Text>().text = puntuacion++ + " POINTS";
            Debug.Log(puntuacion);
            Debug.Log("JUGADOR2");
            GameObject.Find("Jugador2").GetComponent<Animator>().SetBool("golpea", true);

            Reiniciar();
        } else if (collision.transform.tag.Equals("Limite"))
        {
            Debug.Log("CHOQUE LIMITE");
            Reiniciar();
        }
    }

    public void Reiniciar()
    {
        this.gameObject.SetActive(false);
        miTransform.position = this.posicionInicial.position;
        GameObject.Find("PosProyectil").GetComponent<Proyectil>().AnadirProyectil(this.gameObject);
    }

}

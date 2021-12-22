using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoverEnemigo : MonoBehaviour
{
    private Transform miTransform;
    public static float velocidadEnemigo;
    public static float puntuacionEspecial;


    // Start is called before the first frame update
    void Start()
    {
        puntuacionEspecial = 100;
        miTransform = this.transform;
        velocidadEnemigo = 1;

    }

    // Update is called once per frame
    void Update()
    {
        miTransform.Translate(Vector3.up * velocidadEnemigo * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag.Equals("Muro"))
        {
            Debug.Log("CHOQUE MURO");
            miTransform.transform.Rotate(0f, 0.0f, 180.0f, Space.Self);
        }
        else
        {
            velocidadEnemigo = 0;
        }
    }

    private void Desactivarenemigo()
    {
        miTransform.gameObject.SetActive(false);
    }
}



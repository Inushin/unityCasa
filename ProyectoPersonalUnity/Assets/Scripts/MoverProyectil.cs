using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverProyectil : MonoBehaviour
{
    private Transform miTransform;
    public int velocidad;
    public Vector3 _velocidad;
    public Transform posicionInicial;

    // Start is called before the first frame update
    void Start()
    {
        miTransform = this.transform;
        posicionInicial = GameObject.Find("PosProyectil").transform;

    }

    // Update is called once per frame
    void Update()
    {
        miTransform.Translate(_velocidad * velocidad * Time.deltaTime);
    }

    public void Reiniciar()
    {
        this.gameObject.SetActive(false);
        miTransform.position = this.posicionInicial.position;
        GameObject.Find("PosProyectil").GetComponent<Proyectil>().AnadirProyectil(this.gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectil2 : MonoBehaviour
{

    public GameObject proyectil2;
    public int cantidad2;
    public List<GameObject> listaProyectiles2;

    // Start is called before the first frame update
    void Start()
    {
        listaProyectiles2 = new List<GameObject>();
        for (int i = 0; i < cantidad2; i++)
        {
            listaProyectiles2.Add(Instantiate(proyectil2));
        }
    }

    public void CrearProyectiles(Vector3 pos)
    {
        GameObject proyectilesColocar = listaProyectiles2[0];
        proyectilesColocar.transform.localRotation = GameObject.Find("Disparo2").transform.rotation;
        listaProyectiles2.RemoveAt(0);
        proyectilesColocar.transform.position = pos;
        proyectilesColocar.SetActive(true);
    }

    public void AnadirProyectil(GameObject proyectilAnadir)
    {
        listaProyectiles2.Add(proyectilAnadir);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectil : MonoBehaviour
{

    public GameObject proyectil;
    public int cantidad;
    public List<GameObject> listaProyectiles;

    // Start is called before the first frame update
    void Start()
    {
        listaProyectiles = new List<GameObject>();
        for (int i = 0; i < cantidad; i++)
        {
            listaProyectiles.Add(Instantiate(proyectil));
        }
    }

    public void CrearProyectiles(Vector3 pos)
    {
        GameObject proyectilesColocar = listaProyectiles[0];
        listaProyectiles.RemoveAt(0);
        proyectilesColocar.transform.position = pos;
        proyectilesColocar.SetActive(true);
    }

    public void AnadirProyectil(GameObject proyectilAnadir)
    {
        listaProyectiles.Add(proyectilAnadir);
    }
}

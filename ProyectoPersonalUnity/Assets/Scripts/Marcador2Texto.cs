using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Marcador2Texto : MonoBehaviour
{
    // Start is called before the first frame update

    public int puntuacion;
    // Start is called before the first frame update
    public void SumarPuntos()

    {
        this.gameObject.GetComponent<Text>().text = puntuacion++ + " points";
    }
}
 


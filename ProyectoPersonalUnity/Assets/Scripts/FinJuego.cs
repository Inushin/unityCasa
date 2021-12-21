using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinJuego : MonoBehaviour
{

        public void JuegoFinalizado()
        {

            GameObject.Find("NetworkClient").GetComponent<NetworkClient>().panelJuego.SetActive(false);
            GameObject.Find("NetworkClient").GetComponent<NetworkClient>().panelFin.SetActive(true);
        }
    


}

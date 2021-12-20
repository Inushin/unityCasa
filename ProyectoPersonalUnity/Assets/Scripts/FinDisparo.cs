using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinDisparo : MonoBehaviour
{
    // Start is called before the first frame update
    public void ApagarDisparo()
    {
        this.gameObject.GetComponent<Animator>().SetBool("disparar", false);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using System.Text;
using Unity.Networking.Transport;
using UnityEngine.UI;
using NetworkMessages;
using NetworkObject;
using System;

public class NetworkClient : MonoBehaviour
{
    public NetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    private bool empezar = false;
    [Header("ip del servidor")]
    public string serverIP;
    [Header("puerto a la escucha")]
    public ushort serverPort;
    [Header("InputBox -(Escribir Usuario)")]
    public InputField inputNombre;

    public string idPlayer;
    public GameObject panelPrincipal, panelJuego, panelFin;
    public GameObject[] jugadoresGameObject;
    public GameObject disparoFlash;
    

    // Start is called before the first frame update
    public void Conectar()
    {
        m_Driver = NetworkDriver.Create();
        m_Connection = default(NetworkConnection);
        var endpoint = NetworkEndPoint.Parse(serverIP, serverPort);
        m_Connection = m_Driver.Connect(endpoint);

        inputNombre.gameObject.SetActive(false);
        GameObject.Find("Button").SetActive(false);
        //GameObject.Find("Text").GetComponent<Text>().text = "Esperando";
        empezar = true;

    }

    // Update is called once per frame
    void Update()
    {
        m_Driver.ScheduleUpdate().Complete();
        if(!m_Connection.IsCreated)
        {
            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd = m_Connection.PopEvent(m_Driver, out stream);

        while (cmd != NetworkEvent.Type.Empty) 
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                OnConnect();
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                OnData(stream);
            }
            else if (cmd==NetworkEvent.Type.Disconnect) 
            {
                OnDisconnect();
            }
            cmd = m_Connection.PopEvent(m_Driver, out stream);
        }
       // GameObject.Find("Jugador1").GetComponent<Animator>().SetInteger("velocidad", 0);
        //miAnimator.SetInteger("velocidad", 0);
        if (Input.GetKey(KeyCode.E))
        {
            PlayerInputMsg playerInputMsg = new PlayerInputMsg();
            playerInputMsg.id = idPlayer;
            playerInputMsg.myInput = "EMPEZAR";
            SendToServer(JsonUtility.ToJson(playerInputMsg));
        }
        if(Input.GetKey(KeyCode.UpArrow))
        {
            PlayerInputMsg playerInputMsg = new PlayerInputMsg();
            playerInputMsg.id = idPlayer;
            playerInputMsg.myInput = "ARRIBA";
            SendToServer(JsonUtility.ToJson(playerInputMsg));
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            PlayerInputMsg playerInputMsg = new PlayerInputMsg();
            playerInputMsg.id = idPlayer;
            playerInputMsg.myInput = "ABAJO";
            SendToServer(JsonUtility.ToJson(playerInputMsg));
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            PlayerInputMsg playerInputMsg = new PlayerInputMsg();
            playerInputMsg.id = idPlayer;
            playerInputMsg.myInput = "DERECHA";
            SendToServer(JsonUtility.ToJson(playerInputMsg));
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PlayerInputMsg playerInputMsg = new PlayerInputMsg();
            playerInputMsg.id = idPlayer;
            playerInputMsg.myInput = "IZQUIERDA";
            SendToServer(JsonUtility.ToJson(playerInputMsg));
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            PlayerInputMsg playerInputMsg = new PlayerInputMsg();
            playerInputMsg.id = idPlayer;
            playerInputMsg.myInput = "DISPARAR";
            SendToServer(JsonUtility.ToJson(playerInputMsg));
        }
    }

    private void OnConnect()
    {
        Debug.Log("Conectado Correctamente");
    }

    private void OnData(DataStreamReader stream) {
        NativeArray<byte> bytes = new NativeArray<byte>(stream.Length, Allocator.Temp);
        stream.ReadBytes(bytes);
        string recMsg = Encoding.ASCII.GetString(bytes.ToArray());
        NetworkHeader header = JsonUtility.FromJson<NetworkHeader>(recMsg);

        switch (header.command)
        {
            case Commands.HANDSHAKE:
                HandshakeMsg mensajeRecibido = JsonUtility.FromJson<HandshakeMsg>(recMsg);
                //asigno la ide de la conexion en cliente, para despues enviar mensajes
                idPlayer = mensajeRecibido.player.id;
                //Genero un nuevo mensaje para enviar la infromacion al servidor
                HandshakeMsg mensajeEnviar = new HandshakeMsg();
                mensajeEnviar.player.nombre = inputNombre.text;
                SendToServer(JsonUtility.ToJson(mensajeEnviar));
                break;
            case Commands.READY:
                ReadyMsg readyMsg = JsonUtility.FromJson<ReadyMsg>(recMsg);
                panelPrincipal.SetActive(false);
                int numPlayers = readyMsg.playerList.Count;
                panelJuego.SetActive(true);
                for(int i = 0; i < numPlayers; i++)
                {
                  //  jugadoresGameObject[i].GetComponentInChildren<Text>().text = readyMsg.playerList[i].nombre;
                }
                break;
            case Commands.PLAYERINPUT:
                PlayerInputMsg playerInputRecibido = JsonUtility.FromJson<PlayerInputMsg>(recMsg);
                if(playerInputRecibido.myInput == "EMPEZAR")
                {

                }
                break;
            case Commands.MOVER_TANQUE:
                MoverTanqueMsg moverTanqueMsg = JsonUtility.FromJson<MoverTanqueMsg>(recMsg);
                int idJugador;
                int.TryParse(moverTanqueMsg.jugador.id, out idJugador);
                GameObject.Find("Jugador1").transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("disparar", false);
                GameObject.Find("Jugador2").transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("disparar", false);
                GameObject.Find("Jugador1").GetComponent<Animator>().SetInteger("velocidad", 1);
                jugadoresGameObject[idJugador].transform.position = moverTanqueMsg.jugador.posJugador;
                jugadoresGameObject[idJugador].transform.rotation = moverTanqueMsg.jugador.rotacionJugador;
                break;
            case Commands.DISPARAR:
                DispararTanqueMsg dispararTanqueMsg = JsonUtility.FromJson<DispararTanqueMsg>(recMsg);
             //   int idJugador;
                int.TryParse(dispararTanqueMsg.jugador.id, out idJugador);
               // disparoFlash.transform.position = dispararTanqueMsg.posCanon;
                Debug.Log(dispararTanqueMsg.jugador.id);
                //disparoFlash.SetActive(true);
                if (dispararTanqueMsg.jugador.id == "0")
                {
                    Debug.Log("JUGADOR 1");
                    Debug.Log(dispararTanqueMsg.jugador.id);
                    GameObject.Find("Jugador1").transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("disparar", true);
                    GameObject.Find("PosProyectil").GetComponent<Proyectil>().CrearProyectiles(dispararTanqueMsg.posCanon);
                    
                }
                if (dispararTanqueMsg.jugador.id=="1")
                {
                    Debug.Log("JUGADOR 2");
                    GameObject.Find("Jugador2").transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("disparar", true);
                    GameObject.Find("PosProyectil2").GetComponent<Proyectil2>().CrearProyectiles(dispararTanqueMsg.posCanon);
                    
                }
                Debug.Log(disparoFlash.transform.position + "DISPARADO");
                //SendToServer(JsonUtility.ToJson(dispararTanqueMsg));
                break;
            default:
                Debug.Log("Mensaje desconocido");
                break;
            
        }
    }

    private void SendToServer(string v)
    {
        DataStreamWriter writer;
        m_Driver.BeginSend(NetworkPipeline.Null,m_Connection, out writer);
        NativeArray<byte> bytes = new
            NativeArray<byte>(Encoding.ASCII.GetBytes(v), Allocator.Temp);
        writer.WriteBytes(bytes);
        m_Driver.EndSend(writer);
    }

    private void OnDisconnect()
    {
        m_Connection = default(NetworkConnection);
    }

  

    public void OnDestroy()
    {
        m_Connection.Disconnect(m_Driver);
        m_Driver.Dispose();
    }
    

}

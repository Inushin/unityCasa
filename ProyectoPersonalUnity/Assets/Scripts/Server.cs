using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using System.Text;
using Unity.Networking.Transport;
using NetworkMessages;
using NetworkObject;
using System;
using UnityEngine.UI;

public class Server : MonoBehaviour
{
    public NetworkDriver m_Driver;
    public ushort serverPort;
    public NativeList<NetworkConnection> m_connections;
    public GameObject[] jugadoresSimulados;
    public List<NetworkObject.NetworkPlayer> jugadores;
    public bool juegoEmpezado = false;
    public float velocidadPala;
    public GameObject pelota;
    public int[] goles;

    // Start is called before the first frame update
    void Start()
    {
        m_Driver = NetworkDriver.Create();
        var endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = serverPort;
        if(m_Driver.Bind(endpoint) != 0)
        {
            Debug.Log("Failed to bind to port: " + serverPort);
        } else
        {
            m_Driver.Listen();
        }
        m_connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);


    }

    // Update is called once per frame
    void Update()
    {
        m_Driver.ScheduleUpdate().Complete();

        for(int i = 0; i < m_connections.Length; i++)
        {
            if(!m_connections[i].IsCreated)
            {
                m_connections.RemoveAtSwapBack(i);
                i--;
            }
        }

        //aceptamos las conexiones
        NetworkConnection c = m_Driver.Accept();
        while(c != default(NetworkConnection))
        {
            OnConnect(c);
            c = m_Driver.Accept();
        }

        //leer mensajes
        DataStreamReader stream;
        for (int i = 0; i < m_connections.Length; i++) 
        {
            Assert.IsTrue(m_connections[i].IsCreated);
            NetworkEvent.Type cmd;
            cmd = m_Driver.PopEventForConnection(m_connections[i], out stream);
            while (cmd != NetworkEvent.Type.Empty) 
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    OnData(stream,i);
                }
                else if (cmd==NetworkEvent.Type.Disconnect)
                {
                    OnDisconnect(i);
                }
                //pasamos al siguiente mensaje
                cmd = m_Driver.PopEventForConnection(m_connections[i], out stream);
            }
        }
    }

    void OnConnect(NetworkConnection c)
    {
        m_connections.Add(c);
        Debug.Log("Accepted connection");
        Debug.Log("Numero de Jugadores es:" + m_connections.Length);

        HandshakeMsg m = new HandshakeMsg();
        m.player.id = c.InternalId.ToString();
        SendToClient(JsonUtility.ToJson(m),c);
    }

    private void SendToClient(string message, NetworkConnection c)
    {
        //var writer = m_Driver.BeginSend(NetworkPipeline.Null, c);
        DataStreamWriter writer;
        m_Driver.BeginSend(NetworkPipeline.Null, c, out writer);
        NativeArray<byte> bytes = new
            NativeArray<byte>(Encoding.ASCII.GetBytes(message), Allocator.Temp);
        writer.WriteBytes(bytes);
        m_Driver.EndSend(writer);
    }

    private void OnData(DataStreamReader stream,int numJugador)
    {
        NativeArray<byte> bytes = new NativeArray<byte>(stream.Length, Allocator.Temp);
        stream.ReadBytes(bytes);
        string recMsg = Encoding.ASCII.GetString(bytes.ToArray());
        NetworkHeader header = JsonUtility.FromJson<NetworkHeader>(recMsg);

        switch (header.command)
        {
            case Commands.HANDSHAKE:
                HandshakeMsg mensajeRecibido = JsonUtility.FromJson<HandshakeMsg>(recMsg);
                //Escribo en un log la persona que se ha conectado
                Debug.Log("Se ha conectado: " + mensajeRecibido.player.nombre);
                NetworkObject.NetworkPlayer nuevoJugador = new NetworkObject.NetworkPlayer();
                nuevoJugador.id = mensajeRecibido.player.id;
                nuevoJugador.nombre = mensajeRecibido.player.nombre;
                jugadores.Add(nuevoJugador);
                int numJugadores = jugadores.Count;
                if(numJugadores == 2)
                {
                    Debug.Log("2 jugadores conectados");
                    ReadyMsg readyMsg = new ReadyMsg();
                    readyMsg.playerList = jugadores;
                    for(int i = 0; i < numJugadores; i++)
                    {
                       // jugadoresSimulados[i].GetComponentInChildren<Text>().text = jugadores[i].nombre;
                        SendToClient(JsonUtility.ToJson(readyMsg), m_connections[i]);
                    }
                }
                break;
            case Commands.PLAYERINPUT:
                PlayerInputMsg playerInputMsg = JsonUtility.FromJson<PlayerInputMsg>(recMsg);
                if(!juegoEmpezado && playerInputMsg.myInput == "EMPEZAR")
                {
                    int tamArray = jugadores.Count;
                    for (int i = 0; i < tamArray; i++)
                    {
                        // jugadoresSimulados[i].GetComponentInChildren<Text>().text = jugadores[i].nombre;
                        SendToClient(JsonUtility.ToJson(playerInputMsg), m_connections[i]);
                    }
                    juegoEmpezado = true;
                    Debug.Log("�Empezar!");
                } else if (juegoEmpezado && playerInputMsg.myInput == "DERECHA")
                {
                    int indiceJugador = -1;
                    int.TryParse(playerInputMsg.id, out indiceJugador);
                    jugadoresSimulados[indiceJugador].transform.Translate(Vector3.right * velocidadPala * Time.deltaTime);
                    int cantidadJugadores = jugadores.Count;
                    MoverTanqueMsg moverTanqueMsg = new MoverTanqueMsg();
                    moverTanqueMsg.jugador.id = playerInputMsg.id;
                    moverTanqueMsg.jugador.posJugador = jugadoresSimulados[indiceJugador].transform.position;
                    for (int i = 0; i < cantidadJugadores; i++)
                    {
                        SendToClient(JsonUtility.ToJson(moverTanqueMsg), m_connections[i]);
                    }
                }
                else if (juegoEmpezado && playerInputMsg.myInput == "IZQUIERDA")
                {
                    int indiceJugador = -1;
                    int.TryParse(playerInputMsg.id, out indiceJugador);
                    jugadoresSimulados[indiceJugador].transform.Translate(Vector3.left * velocidadPala * Time.deltaTime);
                    int cantidadJugadores = jugadores.Count;
                    MoverTanqueMsg moverTanqueMsg = new MoverTanqueMsg();
                    moverTanqueMsg.jugador.id = playerInputMsg.id;
                    moverTanqueMsg.jugador.posJugador = jugadoresSimulados[indiceJugador].transform.position;
                    for (int i = 0; i < cantidadJugadores; i++)
                    {
                        SendToClient(JsonUtility.ToJson(moverTanqueMsg), m_connections[i]);
                    }
                }
                else if (juegoEmpezado && playerInputMsg.myInput == "ARRIBA")
                {
                    int indiceJugador = -1;
                    int.TryParse(playerInputMsg.id, out indiceJugador);
                    jugadoresSimulados[indiceJugador].transform.Translate(Vector3.up * velocidadPala * Time.deltaTime);
                    int cantidadJugadores = jugadores.Count;
                    MoverTanqueMsg moverTanqueMsg = new MoverTanqueMsg();
                    moverTanqueMsg.jugador.id = playerInputMsg.id;
                    moverTanqueMsg.jugador.posJugador = jugadoresSimulados[indiceJugador].transform.position;
                    for (int i = 0; i < cantidadJugadores; i++)
                    {
                        SendToClient(JsonUtility.ToJson(moverTanqueMsg), m_connections[i]);
                    }
                }
                else if (juegoEmpezado && playerInputMsg.myInput == "ABAJO")
                {
                    int indiceJugador = -1;
                    int.TryParse(playerInputMsg.id, out indiceJugador);
                    jugadoresSimulados[indiceJugador].transform.Translate(Vector3.down* velocidadPala * Time.deltaTime);
                    int cantidadJugadores = jugadores.Count;
                    MoverTanqueMsg moverTanqueMsg = new MoverTanqueMsg();
                    moverTanqueMsg.jugador.id = playerInputMsg.id;
                    moverTanqueMsg.jugador.posJugador = jugadoresSimulados[indiceJugador].transform.position;
                    for (int i = 0; i < cantidadJugadores; i++)
                    {
                        SendToClient(JsonUtility.ToJson(moverTanqueMsg), m_connections[i]);
                    }
                }
                break;
            default:
                Debug.Log("Mensaje desconocido");
                break;
        }
    }

    private void OnDisconnect(int i)
    {
            m_connections[i] = default(NetworkConnection);
    }

    public void OnDestroy()
    {
        m_connections.Dispose();
        m_Driver.Dispose();
    }


}

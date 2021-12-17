using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using System.Text;
using Unity.Networking.Transport;

namespace NetworkObject {
    [System.Serializable]
    public class NetworkObject 
    {
        public string id;
    }

    [System.Serializable]
    public class NetworkPlayer:NetworkObject
    {
        public Vector3 posJugador;
        public Quaternion rotacionJugador;
        public string nombre;
    }
}

namespace NetworkMessages
{
    public enum Commands { 
        HANDSHAKE,
        READY,
        PLAYERINPUT,
        MOVER_TANQUE,
        DISPARAR
    }

    [System.Serializable]
    public class NetworkHeader
    {
        public Commands command;
    }

    [System.Serializable]
    public class HandshakeMsg : NetworkHeader
    {
        public NetworkObject.NetworkPlayer player;
        public HandshakeMsg() 
        {
            command = Commands.HANDSHAKE;
            player = new NetworkObject.NetworkPlayer();
        }
    }

    [System.Serializable]
    public class ReadyMsg : NetworkHeader
    {
        public List<NetworkObject.NetworkPlayer> playerList;
        public ReadyMsg()
        {
            command = Commands.READY;
            playerList = new List<NetworkObject.NetworkPlayer>();
        }
    }

    [System.Serializable]
    public class PlayerInputMsg : NetworkHeader
    {
        public string id;
        public string myInput;
        public PlayerInputMsg()
        {
            command = Commands.PLAYERINPUT;
            myInput = "";
            id = "0";

        }
    }

    [System.Serializable]
    public class MoverTanqueMsg: NetworkHeader
    {
        public NetworkObject.NetworkPlayer jugador;
        public MoverTanqueMsg()
        {
            jugador = new NetworkObject.NetworkPlayer();
            command = Commands.MOVER_TANQUE;
        }
        
    }
    
    [System.Serializable]
    public class DispararTanqueMsg : NetworkHeader
    {
        public NetworkObject.NetworkPlayer jugador;
        public Vector3 posCanon;
        public DispararTanqueMsg()
        {
           jugador = new NetworkObject.NetworkPlayer();
           posCanon = jugador.posJugador;
           command = Commands.DISPARAR;
            
        }

    }

}




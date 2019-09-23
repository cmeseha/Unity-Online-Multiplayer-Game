using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class MPManager : MonoBehaviourPunCallbacks
{

    public GameObject[] EnableObjectsOnConnect;
    public GameObject[] DisableObjectsOnConnect;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Use start for initialization
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Now connected to Photon");
        foreach (GameObject obj in EnableObjectsOnConnect)
        {
            obj.SetActive(true);
        }
        foreach(GameObject obj in DisableObjectsOnConnect)
        {
            obj.SetActive(false);
        }
        //PhotonNetwork.AutomaticallySyncScene = true;

    }
    
    public void JoinConnect4Room()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateConnect4Room();
    }

    public void CreateConnect4Room()
    {
        RoomOptions ro = new RoomOptions { MaxPlayers = 2, IsOpen = true, IsVisible = true };
        PhotonNetwork.CreateRoom("DefaultConnect4Room", ro, TypedLobby.Default);
        SceneManager.LoadScene("ConnectFour");
    }
    /* For example, you could define a low CCU situation as less than 20 rooms.
     * So, if LoadBalancingClient.RoomsCount < 20, your clients use no filtering and 
     * instead run the Random Matchmaking routine (try to find a room, create one and 
     * wait if that fails).
     * 
     * The LoadBalancingClient.RoomsCount should be a good, generic indicator of how busy
     * the game currently is. You could obviously also fine tune the matchmaking by on 
     * how many players are not in a room (LoadBalancingClient.PlayersOnMasterCount). 
     * Whoever is not in a room might be looking for one.
     * */
}

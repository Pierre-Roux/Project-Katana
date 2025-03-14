using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using System;


public class RoomManager : MonoBehaviourPunCallbacks
{

    public static RoomManager Instance;

    [Header(" Params ")]
    public Button buttonCreate;
    public GameObject Content;
    public GameObject RoomItemPrefab;
    public GameObject Loader;

    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();
    private string RoomToCreateName = ""; 


    void Awake()
    {
        Instance = this;

        buttonCreate.interactable = false;
        Loader.gameObject.SetActive(true);
    }

    private IEnumerator Start()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }

        yield return new WaitUntil(() => !PhotonNetwork.IsConnected);

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        if (cachedRoomList.Count <= 0)
        {
            cachedRoomList = roomList;
            buttonCreate.interactable = true;
            Loader.gameObject.SetActive(false);
        }
        else
        {
            foreach (RoomInfo room in cachedRoomList)
            {
                for (int i = 0; i < cachedRoomList.Count; i++)
                {
                    if(cachedRoomList[i].Name == room.Name)
                    {
                        List<RoomInfo> newList = cachedRoomList;
                        if (room.RemovedFromList)
                        {
                            newList.Remove(newList[i]);
                        }
                        else
                        {
                            newList[i] = room;
                        }

                        cachedRoomList = newList;
                    }
                }
            }
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        foreach(Transform roomItem in Content.transform)
        {
            Destroy(roomItem.gameObject);
        }

        foreach (var room in cachedRoomList)
        {
            GameObject roomItem = Instantiate(RoomItemPrefab,Content.transform);

            roomItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = room.Name;
            roomItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = room.PlayerCount + "/16";

            roomItem.GetComponent<RoomItem>().RoomToJoinOfItem = room.Name;
        }

        buttonCreate.interactable = true;
        Loader.gameObject.SetActive(false);
    }
    public void GameToCreateNameChanged(String Name)
    {
        RoomToCreateName = Name;
    }

    public void JoinRoomByName(String RoomToJoinName)
    {
        PlayerPrefs.SetString("RoomNameToJoinOrCreate", RoomToJoinName);

        SceneManager.LoadScene(2);
    }

    public void CreateGame()
    {
        PlayerPrefs.SetString("RoomNameToJoinOrCreate", RoomToCreateName);

        SceneManager.LoadScene(2);
    }
}

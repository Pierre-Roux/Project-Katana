using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Player & Spawning")]
    public GameObject PlayerPrefab;
    public Transform[] SpawnPoints;

    [Header("UI")]
    public string PlayerName;
    public GameObject RoomCam;
    public GameObject connectingUI;
    public GameObject GameUI;

    private void Awake()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            SceneManager.LoadScene(0);
        }

        string RoomNameToJoin = PlayerPrefs.GetString("RoomNameToJoinOrCreate");

        if (RoomNameToJoin == "")
        {
            RoomNameToJoin = "Room" + Random.Range(0,999);
        }

        PhotonNetwork.JoinOrCreateRoom(RoomNameToJoin, null, null);

        connectingUI.SetActive(true);

    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        connectingUI.SetActive(false);
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        RoomCam.SetActive(false);
        GameUI.SetActive(true);

        Debug.Log("Instanciate");
        Vector3 SpawnPos = SpawnPoints[Random.Range(0, SpawnPoints.Length)].position;

        GameObject player = PhotonNetwork.Instantiate(PlayerPrefab.name, SpawnPos, Quaternion.identity);

        player.GetComponent<PlayerSetup>().isLocalPlayer();

        //player.GetComponent<PhotonView>().RPC("SetNickName", RpcTarget.AllBuffered, PlayerName);

    }
}

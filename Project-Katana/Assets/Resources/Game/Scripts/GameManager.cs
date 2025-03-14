using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Player & Spawning")]
    public GameObject PlayerPrefab;
    public GameObject CamsPrefab;
    public PolygonCollider2D BoundingShape;
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
            SceneManager.LoadScene(1);
        }

        string RoomNameToJoin = PlayerPrefs.GetString("RoomNameToJoinOrCreate");

        if (RoomNameToJoin == "")
        {
            RoomNameToJoin = "Room" + Random.Range(0,999);
        }
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinOrCreateRoom(RoomNameToJoin, null, null);
        }

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

        Vector3 SpawnPos = SpawnPoints[Random.Range(0, SpawnPoints.Length)].position;

        GameObject player = PhotonNetwork.Instantiate(PlayerPrefab.name, SpawnPos, Quaternion.identity);

        GameObject Cam = PhotonNetwork.Instantiate(CamsPrefab.name, SpawnPos, Quaternion.identity);

        player.GetComponent<PlayerSetup>().PlayerCamera = Cam.transform.GetChild(0).gameObject;
        GameObject VirtualCamGameobject = player.GetComponent<PlayerSetup>().PlayerVirtualCamera = Cam.transform.GetChild(1).gameObject;
        CinemachineConfiner2D VirtualCam = VirtualCamGameobject.GetComponent<CinemachineConfiner2D>();
        VirtualCam.m_BoundingShape2D = BoundingShape;

        player.GetComponent<PlayerSetup>().isLocalPlayer();

        //player.GetComponent<PhotonView>().RPC("SetNickName", RpcTarget.AllBuffered, PlayerName);

    }
}

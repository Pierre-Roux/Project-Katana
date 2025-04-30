using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Player & Spawning")]
    public GameObject CamsPrefab;
    public PolygonCollider2D BoundingShape;
    public GameObject[] SpawnPoints;

    [Header("UI")]
    public string PlayerName;
    public GameObject RoomCam;
    public GameObject connectingUI;
    public GameObject GameUI;
    public GameObject CharacterSelectUI;
    public GameObject MapSelectUI;


    private GameObject CharacterSelected;
    private GameObject MapSelected;

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

        GameUI.SetActive(false);
        CharacterSelectUI.SetActive(false);
        MapSelectUI.SetActive(false);
        connectingUI.SetActive(true);

    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        connectingUI.SetActive(false);
        CharacterSelectUI.SetActive(true);

    }

    public void SpawnPlayer()
    {
        RoomCam.SetActive(false);
        GameUI.SetActive(true);

        Vector3 SpawnPos = SpawnPoints[Random.Range(0, SpawnPoints.Length)].transform.position;

        GameObject player = PhotonNetwork.Instantiate(CharacterSelected.name, SpawnPos, Quaternion.identity);

        GameObject Cam = PhotonNetwork.Instantiate(CamsPrefab.name, SpawnPos, Quaternion.identity);

        player.GetComponent<PlayerSetup>().PlayerCamera = Cam.transform.GetChild(0).gameObject;
        GameObject VirtualCamGameobject = player.GetComponent<PlayerSetup>().PlayerVirtualCamera = Cam.transform.GetChild(1).gameObject;
        CinemachineConfiner2D VirtualCam = VirtualCamGameobject.GetComponent<CinemachineConfiner2D>();
        VirtualCam.m_BoundingShape2D = BoundingShape;

        player.GetComponent<PlayerSetup>().isLocalPlayer();

        //player.GetComponent<PhotonView>().RPC("SetNickName", RpcTarget.AllBuffered, PlayerName);

    }

    public void SelectCharacter(GameObject PrefabCharacter)
    {
        CharacterSelected = PrefabCharacter;
        CharacterSelectUI.SetActive(false);
        MapSelectUI.SetActive(true);
    }

    public void SelectMap(GameObject MapPrefab)
    {
        MapSelected = MapPrefab;
        MapSelectUI.SetActive(false);
        GameUI.SetActive(true);
        if (!GameObject.FindWithTag("Map"))
        {
            SpawnMap();
        }
        else
        {
            SpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
            BoundingShape = GameObject.FindGameObjectWithTag("CamLimiter").GetComponent<PolygonCollider2D>();
        }
        SpawnPlayer();
    }

    public void SpawnMap()
    {
        GameObject Map = PhotonNetwork.Instantiate(MapSelected.name, Vector3.zero, Quaternion.identity);
        SpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        BoundingShape = GameObject.FindGameObjectWithTag("CamLimiter").GetComponent<PolygonCollider2D>();

    }
}

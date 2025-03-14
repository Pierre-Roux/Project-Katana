using System;
using Cinemachine;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public Player Player;
    public GameObject PlayerCamera;
    public GameObject PlayerVirtualCamera;

    public TextMeshPro NickNameText;

    public void isLocalPlayer()
    {
        
        Player.enabled = true;
        PlayerCamera.SetActive(true);
        PlayerVirtualCamera.SetActive(true);
        PlayerVirtualCamera.GetComponent<CinemachineVirtualCamera>().Follow = Player.gameObject.transform ;
    }

    /*[PunRPC]
    public void SetNickName(String NickName)
    {
        NickNameText.text = NickName;
    }*/
}

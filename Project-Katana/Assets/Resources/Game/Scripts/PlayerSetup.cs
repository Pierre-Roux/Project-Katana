using System;
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
    }

    /*[PunRPC]
    public void SetNickName(String NickName)
    {
        NickNameText.text = NickName;
    }*/
}

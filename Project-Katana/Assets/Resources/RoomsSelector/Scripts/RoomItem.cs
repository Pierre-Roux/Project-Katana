using System;
using UnityEngine;

public class RoomItem : MonoBehaviour
{
    public String RoomToJoinOfItem;

    public void JoinRoom()
    {
        RoomManager.Instance.JoinRoomByName(RoomToJoinOfItem);
    }
}

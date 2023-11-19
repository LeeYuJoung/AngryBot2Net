using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomData : MonoBehaviour
{
    private RoomInfo _roomInfo;
    private Text roomInfoText;
    private PhotonManager photonManager;

    void Awake()
    {
        roomInfoText = GetComponentInChildren<Text>();
        photonManager = GameObject.Find("PhotonManager").GetComponent<PhotonManager>();
    }

    public RoomInfo RoomInfo
    {
        get { return _roomInfo; }
        set
        {
             _roomInfo = value;
            roomInfoText.text = $"{_roomInfo.Name} ({_roomInfo.PlayerCount} / {_roomInfo.MaxPlayers})";

            GetComponent<Button>().onClick.AddListener(() => OnEnterRoom(_roomInfo.Name));
        }
    }
 
    void OnEnterRoom(string roomName)
    {
        photonManager.SetUserID();

        // 冯 加己 沥狼
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 20;
        ro.IsOpen = true;
        ro.IsVisible = true;

        // 冯 立加
        PhotonNetwork.JoinOrCreateRoom(roomName, ro, TypedLobby.Default); 
    }
}

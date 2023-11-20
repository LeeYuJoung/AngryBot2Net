using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public Text roomName;
    public Text connectInfo;
    public Text msgList;

    public Button exitButton;
    public Button gameoverExit;

    void Awake()
    {
        CreatePlayer();
        SetRoomInfo();

        exitButton.onClick.AddListener(() => OnExitClick());
        gameoverExit.onClick.AddListener(() => OnExitClick());
    }

    private void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
    }

    void CreatePlayer()
    {
        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(0, points.Length);

        // 네트워크상에 캐릭터 생성
        PhotonNetwork.Instantiate("Player_0", points[idx].position, points[idx].rotation, 0);
    }

    // 룸 접속 정보 출력
    void SetRoomInfo()
    {
        Room room = PhotonNetwork.CurrentRoom;
        roomName.text = room.Name;
        connectInfo.text = $"{room.PlayerCount}/{room.MaxPlayers}";  
    }

    private void OnExitClick()
    {
        PhotonNetwork.LeaveRoom();
    }

    // 포톤 룸에서 퇴장했을 때 호출되는 콜백 함수
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    // 룸을 새로운 네트워크 유저가 접속했을 때 호출되는 콜백 함수
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        SetRoomInfo();
        string mag = $"\n<color=#00ff00>{newPlayer.NickName}</color> is joined room";
        msgList.text += mag;
    }

    // 룸에서 네트워크 유저가 퇴장했을 때 호출되는 콜백 함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        SetRoomInfo();
        string mag = $"\n<color=#ff0000>{otherPlayer.NickName}</color> is left room";
        msgList.text += mag;
    }
}

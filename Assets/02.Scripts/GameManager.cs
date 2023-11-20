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

        // ��Ʈ��ũ�� ĳ���� ����
        PhotonNetwork.Instantiate("Player_0", points[idx].position, points[idx].rotation, 0);
    }

    // �� ���� ���� ���
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

    // ���� �뿡�� �������� �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    // ���� ���ο� ��Ʈ��ũ ������ �������� �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        SetRoomInfo();
        string mag = $"\n<color=#00ff00>{newPlayer.NickName}</color> is joined room";
        msgList.text += mag;
    }

    // �뿡�� ��Ʈ��ũ ������ �������� �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        SetRoomInfo();
        string mag = $"\n<color=#ff0000>{otherPlayer.NickName}</color> is left room";
        msgList.text += mag;
    }
}

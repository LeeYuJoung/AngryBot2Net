using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private readonly string version = "1.0";
    private string userID = "Zack";

    public InputField userIF;
    public InputField roomNameIF;

    // �� ��Ͽ� ���� �����͸� �����ϱ� ���� ��ųʸ�
    private Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>();
    private GameObject roomItemPrefab;
    public Transform scrollContent;

    void Awake()
    {
        // ������ Ŭ���̾�Ʈ�� �� �ڵ� ����ȭ �ɼ�
        PhotonNetwork.GameVersion = version;

        // ���� �������� �������� �ʴ� ���� Ƚ��
        Debug.Log(PhotonNetwork.SendRate);

        roomItemPrefab = Resources.Load<GameObject>("RoomItem");

        //���� ���� ����
        if(PhotonNetwork.IsConnected ==  false)
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.AutomaticallySyncScene = true;
        }
    }

    private void Start()
    {
        userID = PlayerPrefs.GetString("USER_ID", $"USER_{Random.Range(1, 21):00}");
        userIF.text = userID;
        PhotonNetwork.NickName = userID;
    }

    public void SetUserID()
    {
        if (string.IsNullOrEmpty(userIF.text))
        {
            userID = $"USER_{Random.Range(1, 21):00}";
        }
        else
        {
            userID = userIF.text;
        }

        PlayerPrefs.SetString("USER_ID", userID);
        PhotonNetwork.NickName = userID;
    }

    string SetRoomName()
    {
        if (string.IsNullOrEmpty(roomNameIF.text))
        {
            roomNameIF.text = $"ROOM_{Random.Range(1, 101):000}";
        }

        return roomNameIF.text;
    }

    // ���� ������ ���� �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnConnectedToMaster()
    {
        Debug.Log(": : : Connected To Master : : :");
        Debug.Log($"PhotonNetwork.InLobby : : : {PhotonNetwork.InLobby}");

        // �κ� ����
        PhotonNetwork.JoinLobby();
    }

    // �κ� ���� �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLobby : : : {PhotonNetwork.InLobby}");
    }

    // ������ �� ������ �������� ��� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Faild : : : {returnCode} : {message}");

        OnMakeRoomClick();
    }

    // �� ������ �Ϸ�� �� ȣ��Ǵ�  �ݹ� �Լ�
    public override void OnCreatedRoom()
    {
        Debug.Log(": : : Created Room : : :");
        Debug.Log($"Room Name : {PhotonNetwork.CurrentRoom.Name}");
    }

    // �뿡 ������ �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonNetwork.InRoom : {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count : {PhotonNetwork.CurrentRoom.PlayerCount}");

        foreach(var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName} , {player.Value.ActorNumber}");
        }

        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.IsMessageQueueRunning = false;
            PhotonNetwork.LoadLevel("Main"); 
        }
    }

    // �� ����� �����ϴ� �ݹ� �Լ�
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GameObject tempRoom = null;

        foreach(var roomInfo in roomList)
        {
            // ���� ������ ���
            if(roomInfo.RemovedFromList == true)
            {
                rooms.TryGetValue(roomInfo.Name, out tempRoom);
                
                Destroy(tempRoom);
                rooms.Remove(roomInfo.Name);
            }
            else // ���� �����ǰų� ����� ���
            {
                // ���� ������ ���
                if(rooms.ContainsKey(roomInfo.Name) == false)
                {
                    GameObject roomPrefab = Instantiate(roomItemPrefab, scrollContent);
                    roomPrefab.GetComponent<RoomData>().RoomInfo = roomInfo;
                    rooms.Add(roomInfo.Name, roomPrefab);
                }
                else // ���� ����� ���  
                {
                    rooms.TryGetValue(roomInfo.Name, out tempRoom);
                    tempRoom.GetComponent<RoomData>().RoomInfo = roomInfo;
                }
            }

            Debug.Log($"Room : : : {roomInfo.Name} ({roomInfo.PlayerCount} / {roomInfo.MaxPlayers})");
        }
    }

    #region UI_BUTTON_EVENT

    public void OnLoginClick()
    {
        SetUserID();
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnMakeRoomClick()
    {
        SetUserID();

        // �� �Ӽ� ����
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 20;
        ro.IsOpen = true;
        ro.IsVisible = true;

        // �� ����
        PhotonNetwork.CreateRoom(SetRoomName(), ro);
    }

    #endregion
}

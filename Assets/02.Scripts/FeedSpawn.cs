using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class FeedSpawn : MonoBehaviourPunCallbacks
{
    public void RandomFeedSpawn()
    {
        float posX = Random.Range(-20.0f, 20.0f);
        float posY = Random.Range(-20.0f, 20.0f);

        PhotonNetwork.Instantiate("Feed", new Vector3(posX, posY, 0.0f), Quaternion.identity);
    }
}

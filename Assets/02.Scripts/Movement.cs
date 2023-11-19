using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

public class Movement : MonoBehaviourPunCallbacks, IPunObservable
{
    private CharacterController characterController;
    private Transform tr;
    private Animator animator;
    private Camera cam;
    private CinemachineVirtualCamera virtualCamera;
    private PhotonView pv;
    public Text nickName;

    public int currentScale = 1;
    public float moveSpeed = 5.0f;
    // 수신된 좌표로의 이동 및 회전  속도의 민감도
    public float damping = 10.0f;
    public bool isMove = true;

    // 수신된 위치와 회전값을 저장할 변수
    private Vector3 receivePos;
    private Quaternion receiveRot;
    private Vector3 receiveScale;

    float h => Input.GetAxis("Horizontal");
    float v => Input.GetAxis("Vertical");

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        tr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        cam = Camera.main;

        virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        pv = GetComponent<PhotonView>();

        // PhotonView가 자신의 것일 경우 시네머신 가상카메라를 연결
        if (pv.IsMine)
        {
            virtualCamera.Follow = tr;
            virtualCamera.LookAt = tr;
        }

        nickName.text = photonView.Owner.NickName;
    }

    void Update()
    {
        if (pv.IsMine)
        {
            Move();
        }
        else
        {
            tr.position = Vector3.Lerp(tr.position, receivePos, Time.deltaTime * damping);
            tr.rotation = Quaternion.Slerp(tr.rotation, receiveRot, Time.deltaTime * damping);
            tr.localScale = Vector3.Lerp(tr.localScale, receiveScale, Time.deltaTime * damping);
        }
    }

    public void Move()
    {
        Vector3 cameraForward = cam.transform.up;
        Vector3 cameraRight = cam.transform.right;
        cameraForward.z = 0.0f;
        cameraRight.z = 0.0f;

        Vector3 moveDir = (cameraForward * v) + (cameraRight * h);
        moveDir.Set(moveDir.x, moveDir.y, 0.0f);

        characterController.Move(moveDir * moveSpeed * Time.deltaTime);


        float forward = Vector3.Dot(moveDir, tr.up);
        float right = Vector3.Dot(moveDir, tr.right);

        if (h < 0)
        {
            //tr.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (h > 0)
        {
            //tr.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            GetComponent<SpriteRenderer>().flipX = false;
        }

        animator.SetInteger("MoveX", (int)right);
        animator.SetInteger("MoveY", (int)forward);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 자신의 로컬 캐릭터인 경우 자신의 데이터를 다른 네트워크 유저에게 송신
        if (stream.IsWriting)
        {
            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);
            stream.SendNext(tr.localScale);
        }
        else
        {
            receivePos = (Vector3)stream.ReceiveNext();
            receiveRot = (Quaternion)stream.ReceiveNext();
            receiveScale = (Vector3)stream.ReceiveNext();
        }
    }

    IEnumerator PlayerDie()
    {
        characterController.enabled = false;
        animator.SetTrigger("Die");

        yield return new WaitForSeconds(1.25f);

        Destroy(gameObject);
        GameObject.Find("Canvas").transform.GetChild(1).gameObject.SetActive(true);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.CompareTag("Feed"))
        {
            currentScale += 1;
            tr.localScale += new Vector3(0.25f, 0.25f, 0.25f);
            Destroy(hit.gameObject);

            GameObject.Find("FeedSpawn").GetComponent<FeedSpawn>().RandomFeedSpawn();
        }
        else if(hit.transform.CompareTag("Player"))
        {
            StartCoroutine(PlayerDie());
        }
    }
}

using Cinemachine;
using Photon.Pun;
using UnityEngine;

public class CameraSetup : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
		// ���� �ڽ��� ���� �÷��̾���
		if (photonView.IsMine)
		{
			// ���� �ִ� �ó׸ӽ� ���� ī�޶� ã��
			CinemachineVirtualCamera followCam = FindObjectOfType<CinemachineVirtualCamera>();
			// ���� ī�޶��� ���� ����� �ڽ��� Ʈ���������� ����
			followCam.Follow = transform;
			followCam.LookAt = transform;
		}
    }
}

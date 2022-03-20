using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

// 점수와 게임 오버 여부를 관리하는 게임 매니저
public class GameManager : MonoBehaviourPunCallbacks, IPunObservable {
    // 싱글톤 접근용 프로퍼티
    public static GameManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<GameManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private static GameManager m_instance; // 싱글톤이 할당될 static 변수

    public GameObject playerPreFeb; // 생성할 플레이어 케릭터 프리팹

    private int score = 0; // 현재 게임 점수
    public bool isGameover { get; private set; } // 게임 오버 상태

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		// 로컬 오브젝트라면 쓰기 부분이 실행됨
		if (stream.IsWriting)
		{
            // 네트워크를 통해 score값 보내기
            stream.SendNext(score);
		}
		else
		{
            // 네트워크를 통해 score값 받기
            score = (int)stream.ReceiveNext();

            // 동기화하여 받은 점수를 UI로 표시
            UIManager.instance.UpdateScoreText(score);
		}
	}

    private void Awake() {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
    }

    private void Start() {
        // 생성할 랜덤 위치 지정
        Vector3 randomSpawnPos = Random.insideUnitSphere * 5f;
        randomSpawnPos.y = 0;

        // 네트워크상의 모든 클라이언트에서 생성 실행
        // 해당 게임 오브젝트의 주도권은 생성 메서드를 직접 실행한 클라이언트에 있음
        PhotonNetwork.Instantiate(playerPreFeb.name, randomSpawnPos, Quaternion.identity);
    }

    // 점수를 추가하고 UI 갱신
    public void AddScore(int newScore) {
        // 게임 오버가 아닌 상태에서만 점수 증가 가능
        if (!isGameover)
        {
            // 점수 추가
            score += newScore;
            // 점수 UI 텍스트 갱신
            UIManager.instance.UpdateScoreText(score);
        }
    }

    // 게임 오버 처리
    public void EndGame() {
        // 게임 오버 상태를 참으로 변경
        isGameover = true;
        // 게임 오버 UI를 활성화
        UIManager.instance.SetActiveGameoverUI(true);
    }

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
            PhotonNetwork.LeaveRoom();
		}
	}

	// 룸을 나갈 때 자동 실행되는 메서드
	public override void OnLeftRoom()
	{
        SceneManager.LoadScene("Lobby");
	}
}
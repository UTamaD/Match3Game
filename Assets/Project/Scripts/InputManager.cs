using System;
using UnityEngine;

/// <summary>
/// 플레이어 입력을 처리하는 클래스
/// </summary>
public class InputManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static InputManager Instance { get; private set; }
    
    private GameObject firstPiece;     // 첫 번째 선택된 조각
    private GameObject secondPiece;    // 두 번째 선택된 조각

    private bool inputEnabled = true;  // 입력 활성화 여부

    /// <summary>
    /// 입력 활성화 상태 설정 함수
    /// </summary>
    /// <param name="enable">활성화 여부</param>
    public void SetInputEnabled(bool enable)
    {
        inputEnabled = enable;
    }
    
    /// <summary>
    /// 초기화 시 싱글톤 인스턴스 설정
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 시작 시 초기 설정
    /// </summary>
    private void Start()
    {
        inputEnabled = true;
    }

    /// <summary>
    /// 매 프레임마다 입력 처리
    /// </summary>
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && PieceManager.Instance.IsPieceMoving == false && inputEnabled)
        {
            // 클릭 위치를 월드 좌표로 변환
            Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D collider = Physics2D.OverlapPoint(clickPosition);

            if (collider != null)
            {
                if (firstPiece == null)
                {
                    // 첫 번째 조각 선택
                    firstPiece = collider.gameObject;
                    AudioManager.Instance.PlaySelectSound();
   
                    firstPiece.GetComponent<Piece>().HighlightPiece(true);
                }
                else
                {
                    // 두 번째 조각 선택
                    secondPiece = collider.gameObject;
                    AudioManager.Instance.PlaySelectSound();
                    
                    secondPiece.GetComponent<Piece>().HighlightPiece(true);
                    PieceManager.Instance.SwapPieces(firstPiece, secondPiece);
                    
                    // 선택 초기화
                    firstPiece = null;
                    secondPiece = null;
                }
            }
        }
    }
}
using UnityEngine;

/// <summary>
/// 게임 내 각 조각 블록을 관리하는 클래스
/// </summary>
public class Piece : MonoBehaviour
{
    public Vector2 index;               // 그리드 내 조각의 인덱스 위치
    private Renderer pieceRenderer;     // 조각의 렌더러 컴포넌트

    /// <summary>
    /// 초기화 시 렌더러 컴포넌트 참조
    /// </summary>
    private void Awake()
    {
        pieceRenderer = GetComponent<Renderer>();
    }

    /// <summary>
    /// 조각 강조 효과 설정 함수
    /// </summary>
    /// <param name="highlight">강조 여부</param>
    public void HighlightPiece(bool highlight)
    {
        pieceRenderer.material.SetFloat("_OutlineEnabled", highlight ? 1.0f : 0.0f);
    }
}
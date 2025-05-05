using System;
using System.Collections;
using System.Collections.Generic;
using SmoothShakeFree;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 게임 내 조각들의 움직임과 상호작용을 관리하는 클래스
/// </summary>
public class PieceManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static PieceManager Instance { get; private set; }

    public float moveSpeed = 0.3f; // 조각이 이동하는 속도
    public float dropSpeed = 0.5f; // 조각이 떨어지는 속도

    private Color originalColor1;   // 원래 색상 1 저장
    private Color originalColor2;   // 원래 색상 2 저장

    public bool IsPieceMoving { get; private set; }  // 조각 이동 중 여부
    public GameObject disappearEffectPrefab;         // 조각 사라짐 이펙트 프리팹
    public GameObject comboEffectPrefab;             // 콤보 이펙트 프리팹
    public GameObject explodeEffectPrefab;           // 폭발 이펙트 프리팹
    private int currentCombo = 0;                    // 현재 콤보 수


    public SmoothShake smoothShake;                  // 화면 흔들림 효과 컴포넌트
    private bool isFirstMoveAfterManualErase = false; // 수동 삭제 후 첫 이동인지 여부
    
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
    /// 컴포넌트 및 프리팹 참조 확인
    /// </summary>
    private void Start()
    {
        if (smoothShake == null)
        {
            smoothShake = FindObjectOfType<SmoothShake>().GetComponent<SmoothShake>();
        }
        if (disappearEffectPrefab == null)
        {
            Debug.LogError("사라짐 이펙트 프리팹이 할당되지 않았습니다!");
        }
    }

    /// <summary>
    /// 두 조각이 인접해 있는지 확인하는 함수
    /// </summary>
    /// <param name="piece1">첫 번째 조각</param>
    /// <param name="piece2">두 번째 조각</param>
    /// <returns>인접 여부</returns>
    private bool AreAdjacent(GameObject piece1, GameObject piece2)
    {
        Vector2 index1 = piece1.GetComponent<Piece>().index;
        Vector2 index2 = piece2.GetComponent<Piece>().index;

        // 두 조각이 한 칸의 거리만큼 떨어져 있는지 확인
        return (Mathf.Abs(index1.x - index2.x) == 1 && index1.y == index2.y) || (Mathf.Abs(index1.y - index2.y) == 1 && index1.x == index2.x);
    }

    /// <summary>
    /// 조각 삭제 시 호출되는 함수
    /// </summary>
    /// <param name="piece">삭제할 조각</param>
    public void HandlePieceDisappearance(GameObject piece)
    {
        if (disappearEffectPrefab == null) return;

        // 삭제 이펙트 생성
        Instantiate(disappearEffectPrefab, piece.transform.position, Quaternion.identity);
        
        Destroy(piece);
        
        // 점수 추가
        ScoreManager.Instance.AddScore(currentCombo);
        
        // 아이템 게이지 증가
        string pieceTag = piece.tag;
        ItemManager.Instance.AddGauge(pieceTag, 10f);
    }

    /// <summary>
    /// 두 조각을 교환하는 함수
    /// </summary>
    /// <param name="piece1">첫 번째 조각</param>
    /// <param name="piece2">두 번째 조각</param>
    public void SwapPieces(GameObject piece1, GameObject piece2)
    {
        UnhighlightPieces(piece1, piece2);
        if (AreAdjacent(piece1, piece2))
        {
            IsPieceMoving = true;
            ICommand swapCommand = new SwapCommand(piece1, piece2, moveSpeed);
            CommandManager.Instance.ExecuteCommand(swapCommand);
            StartCoroutine(CheckMatchesAfterMove(piece1, piece2, swapCommand));
            IsPieceMoving = false;
        }
    }

    /// <summary>
    /// 콤보 증가 함수
    /// </summary>
    public void IncreaseCombo()
    {
        currentCombo = Mathf.Min(currentCombo + 1, 3);

        if (currentCombo == 1)
        {
            // 첫 콤보 이펙트 없음
        }
        else if (currentCombo == 2)
        {
            CharacterManager.Instance.SetImage(5, 0.2f);
            AudioManager.Instance.PlayMaxComboSound(1);
        }
        else if (currentCombo >= 3)
        {
            CharacterManager.Instance.SetImage(6, 0.2f);
            AudioManager.Instance.PlayMaxComboSound(2);
        }
    }

    /// <summary>
    /// 콤보 초기화 함수
    /// </summary>
    public void ResetCombo()
    {
        CharacterManager.Instance.SetImage(0, 0.0f);   
        currentCombo = 0;
    }
    
    /// <summary>
    /// 조각 이동 후 매치 확인하는 코루틴
    /// </summary>
    /// <param name="piece1">첫 번째 조각</param>
    /// <param name="piece2">두 번째 조각</param>
    /// <param name="swapCommand">교환 명령 객체</param>
    /// <returns>대기 시간</returns>
    private IEnumerator CheckMatchesAfterMove(GameObject piece1, GameObject piece2, ICommand swapCommand)
    {
        yield return new WaitForSeconds(moveSpeed);
        var initialMatches = MatchManager.Instance.FindMatchesFromMove(piece1, piece2);
        if (initialMatches.Count > 0)
        {
            // 매치된 조각 강조
            foreach (var match in initialMatches)
            {
                match.GetComponent<Piece>().HighlightPiece(true);
            }

            if (isFirstMoveAfterManualErase)
            {
                isFirstMoveAfterManualErase = false;
            }
            else
            {
                ResetCombo();
            }
            yield return ClearAndDropPieces(initialMatches);
        }
        else
        {
            CommandManager.Instance.UndoCommand();
        }
    }
    
    /// <summary>
    /// 매치된 조각을 제거하고 새 조각을 드롭하는 코루틴
    /// </summary>
    /// <param name="matches">매치된 조각 목록</param>
    /// <returns>대기 시간</returns>
    private IEnumerator ClearAndDropPieces(List<GameObject> matches)
    {
        // 매치된 조각들 강조
        foreach (var match in matches)
        {
            match.GetComponent<Piece>().HighlightPiece(true);
        }

        // 강조 효과를 보여주기 위해 잠시 대기
        yield return new WaitForSeconds(0.2f);
        smoothShake.StartShake();
        
        if (comboEffectPrefab != null && currentCombo > 0)
        {
            Vector3 centerPosition = CalculateCenterOfMatches(matches);
            GameObject effect = Instantiate(comboEffectPrefab, centerPosition, Quaternion.identity);
            Destroy(effect, 1.0f);
        }

        foreach (var match in matches)
        {
            Vector2 index = match.GetComponent<Piece>().index;
            GridManager.Instance.SetPieceAt((int)index.x, (int)index.y, null);
            HandlePieceDisappearance(match);
        }
        AudioManager.Instance.PlayComboSound(currentCombo);
        
        // 점수 업데이트
        int points = matches.Count * 5;
        ScoreManager.Instance.AddScore(points);
        GameManager.Instance.AddTime(matches.Count * 0.1f * (currentCombo+1));

        yield return new WaitForSeconds(0.1f);
        smoothShake.StopShake();
        IsPieceMoving = true;
        yield return StartCoroutine(DropPieces()); // 조각 낙하

        yield return new WaitForSeconds(dropSpeed);
        var additionalMatches = MatchManager.Instance.FindAllMatches(); // 매치 탐색
        if (additionalMatches.Count > 0)
        {
            yield return ClearAndDropPieces(additionalMatches);
        }
        else
        {
            ResetCombo();
            IsPieceMoving = false;
        }
    }

    /// <summary>
    /// 빈 공간으로 조각을 떨어뜨리는 코루틴
    /// </summary>
    /// <returns>대기 시간</returns>
    private IEnumerator DropPieces()
    {
        List<KeyValuePair<GameObject, Vector3>> fallingPieces = new List<KeyValuePair<GameObject, Vector3>>();

        for (int x = 0; x < GridManager.Instance.width; x++)
        {
            for (int y = 0; y < GridManager.Instance.height; y++)
            {
                if (GridManager.Instance.GetPieceAt(x, y) == null)
                {
                    for (int newY = y + 1; newY < GridManager.Instance.height; newY++)
                    {
                        GameObject pieceAbove = GridManager.Instance.GetPieceAt(x, newY);
                        if (pieceAbove != null)
                        {
                            Vector3 targetPosition = new Vector3(x, y, 0);
                            fallingPieces.Add(new KeyValuePair<GameObject, Vector3>(pieceAbove, targetPosition));
                            
                            // 그리드 업데이트
                            GridManager.Instance.SetPieceAt(x, y, pieceAbove);
                            GridManager.Instance.SetPieceAt(x, newY, null);
                            pieceAbove.GetComponent<Piece>().index = new Vector2(x, y);
                            break;
                        }
                    }
                }
            }
        }

        // 애니메이션
        float elapsedTime = 0;
        while (elapsedTime < dropSpeed)
        {
            foreach (var kvp in fallingPieces)
            {
                GameObject piece = kvp.Key;
                Vector3 targetPosition = kvp.Value;
                if (piece != null) piece.transform.position = Vector3.Lerp(piece.transform.position, targetPosition, (elapsedTime / dropSpeed));
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        foreach (var kvp in fallingPieces)
        {
            GameObject piece = kvp.Key;
            Vector3 targetPosition = kvp.Value;
            if (piece != null) piece.transform.position = targetPosition;
        }

        // 빈 공간 채우기
        FillEmptySpaces();
    }

    /// <summary>
    /// 빈 공간에 새 조각을 채우는 함수
    /// </summary>
    private void FillEmptySpaces()
    {
        List<KeyValuePair<GameObject, Vector3>> newPieces = new List<KeyValuePair<GameObject, Vector3>>();

        for (int x = 0; x < GridManager.Instance.width; x++)
        {
            for (int y = 0; y < GridManager.Instance.height; y++)
            {
                if (GridManager.Instance.GetPieceAt(x, y) == null)
                {
                    // 새 조각 생성
                    int pieceIndex = UnityEngine.Random.Range(0, GridManager.Instance.pieces.Length);
                    Vector3 spawnPosition = new Vector3(x, GridManager.Instance.height, 0);
                    GameObject newPiece = Instantiate(GridManager.Instance.pieces[pieceIndex], spawnPosition, Quaternion.identity, GridManager.Instance.gridParent);
                    newPiece.GetComponent<Piece>().index = new Vector2(x, y);
                    GridManager.Instance.SetPieceAt(x, y, newPiece);

                    // 새 조각 드롭 애니메이션
                    Vector3 targetPosition = new Vector3(x, y, 0);
                    newPieces.Add(new KeyValuePair<GameObject, Vector3>(newPiece, targetPosition));
                }
            }
        }

        StartCoroutine(AnimateDrop(newPieces));
    }

    /// <summary>
    /// 조각 드롭 애니메이션 코루틴
    /// </summary>
    /// <param name="pieces">드롭할 조각과 목표 위치 쌍</param>
    /// <returns>대기 시간</returns>
    private IEnumerator AnimateDrop(List<KeyValuePair<GameObject, Vector3>> pieces)
    {
        float elapsedTime = 0;
        while (elapsedTime < dropSpeed)
        {
            foreach (var kvp in pieces)
            {
                GameObject piece = kvp.Key;
                Vector3 targetPosition = kvp.Value;
                if (piece != null) piece.transform.position = Vector3.Lerp(piece.transform.position, targetPosition, (elapsedTime / dropSpeed));
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        foreach (var kvp in pieces)
        {
            GameObject piece = kvp.Key;
            Vector3 targetPosition = kvp.Value;
            if (piece != null) piece.transform.position = targetPosition;
        }
    }

    /// <summary>
    /// 조각 강조 효과 해제 함수
    /// </summary>
    /// <param name="piece1">첫 번째 조각</param>
    /// <param name="piece2">두 번째 조각</param>
    private void UnhighlightPieces(GameObject piece1, GameObject piece2)
    {
        if (piece1 != null && piece1.GetComponent<Piece>() != null)
        {
            piece1.GetComponent<Piece>().HighlightPiece(false);
        }

        if (piece2 != null && piece2.GetComponent<Piece>() != null)
        {
            piece2.GetComponent<Piece>().HighlightPiece(false);
        }
    }

    /// <summary>
    /// 매치된 조각들의 중심 위치 계산 함수
    /// </summary>
    /// <param name="matches">매치된 조각 목록</param>
    /// <returns>중심 위치 (Vector3)</returns>
    private Vector3 CalculateCenterOfMatches(List<GameObject> matches)
    {
        Vector3 sum = Vector3.zero;
        foreach (var match in matches)
        {
            sum += match.transform.position;
        }
        return sum / matches.Count;
    }

    /// <summary>
    /// 모든 조각 지우고 새 조각 드롭하는 코루틴 (아이템 효과)
    /// </summary>
    /// <returns>대기 시간</returns>
    public IEnumerator ClearAllAndDropNew()
    {
        isFirstMoveAfterManualErase = true;
        IncreaseCombo();

        List<GameObject> allPieces = new List<GameObject>();
        for (int x = 0; x < GridManager.Instance.width; x++)
        {
            for (int y = 0; y < GridManager.Instance.height; y++)
            {
                GameObject piece = GridManager.Instance.GetPieceAt(x, y);
                if (piece != null && piece.CompareTag("Wall") == false)
                {
                    allPieces.Add(piece);
                }
            }
        }

        smoothShake.StartShake();
        yield return new WaitForSeconds(0.2f);

        foreach (var piece in allPieces)
        {
            Vector2 index = piece.GetComponent<Piece>().index;
            GridManager.Instance.SetPieceAt((int)index.x, (int)index.y, null);
            
            if (explodeEffectPrefab != null)
            {
                GameObject effect = Instantiate(explodeEffectPrefab, piece.transform.position, Quaternion.identity);
                Destroy(effect, 1.0f);
            }
            
            Destroy(piece);
        }

        smoothShake.StopShake();
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(DropPieces());
        
        List<GameObject> additionalMatches = MatchManager.Instance.FindAllMatches();
        if (additionalMatches.Count > 0)
        {
            yield return StartCoroutine(ClearAndDropPieces(additionalMatches));
        }
    }

    /// <summary>
    /// 십자 모양으로 조각 지우고 드롭하는 코루틴 (아이템 효과)
    /// </summary>
    /// <returns>대기 시간</returns>
    public IEnumerator ClearCrossDrop()
    {
        isFirstMoveAfterManualErase = true;
        IncreaseCombo();
        smoothShake.StartShake();
        yield return new WaitForSeconds(0.2f);
        GridManager.Instance.ClearCrossBlock();
        smoothShake.StopShake();
        
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(DropPieces());
    }

    /// <summary>
    /// 새 조각 드롭 애니메이션 코루틴
    /// </summary>
    /// <param name="pieces">드롭할 조각 목록</param>
    /// <returns>대기 시간</returns>
    private IEnumerator DropNewPieces(List<GameObject> pieces)
    {
        // 드롭 애니메이션 구현
        yield return new WaitForSeconds(dropSpeed);
        
        // 추가 매치 검사
        var additionalMatches = MatchManager.Instance.FindAllMatches();
        if (additionalMatches.Count > 0)
        {
            yield return ClearAndDropPieces(additionalMatches);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using SmoothShakeFree;
using UnityEngine;
using Random = UnityEngine.Random;

public class PieceManager : MonoBehaviour
{
    public static PieceManager Instance { get; private set; }

    public float moveSpeed = 0.3f; // 조각이 이동하는 속도
    public float dropSpeed = 0.5f; // 조각이 떨어지는 속도

    private Color originalColor1;
    private Color originalColor2;

    public bool IsPieceMoving { get; private set; }
    public GameObject disappearEffectPrefab;
    public GameObject comboEffectPrefab;

    private int currentCombo = 0;


    public SmoothShake smoothShake;
    private bool isFirstMoveAfterManualErase = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (smoothShake == null)
        {
            smoothShake = FindObjectOfType<SmoothShake>().GetComponent<SmoothShake>();
        }
        if (disappearEffectPrefab == null)
        {
            Debug.LogError("Disappear effect prefab is not assigned!");
        }
    }


    // 두 조각이 인접해 있는지 확인하는 함수
    private bool AreAdjacent(GameObject piece1, GameObject piece2)
    {
        Vector2 index1 = piece1.GetComponent<Piece>().index;
        Vector2 index2 = piece2.GetComponent<Piece>().index;

        // 두 조각이 한 칸의 거리만큼 떨어져 있는지 확인
        return (Mathf.Abs(index1.x - index2.x) == 1 && index1.y == index2.y) || (Mathf.Abs(index1.y - index2.y) == 1 && index1.x == index2.x);
    }

    // 조각 삭제시 호출
    public void HandlePieceDisappearance(GameObject piece)
    {
        if (disappearEffectPrefab == null) return;

        // 삭제 이펙트 생성
        Instantiate(disappearEffectPrefab, piece.transform.position, Quaternion.identity);



        AudioManager.Instance.playComboSound(currentCombo);
        Destroy(piece);
        
        
        ScoreManager.Instance.AddScore(currentCombo);
    }
    public void SwapPieces(GameObject piece1, GameObject piece2)
    {
        UnhighlightPieces(piece1, piece2);
        if (AreAdjacent(piece1, piece2))
        {
            IsPieceMoving = true;
            ICommand swapCommand = new SwapCommand(piece1, piece2,moveSpeed);
            CommandManager.Instance.ExecuteCommand(swapCommand);
            StartCoroutine(CheckMatchesAfterMove(piece1, piece2, swapCommand));
            IsPieceMoving = false;
        }
    }


    
    public void IncreaseCombo()
    {
        currentCombo = Mathf.Min(currentCombo + 1, 3);

        if (currentCombo >= 3)
        {
            AudioManager.Instance.PlayMaxComboSound();
        }
    }


    public void ResetCombo()
    {
        currentCombo = 0;
    }
    

    private IEnumerator CheckMatchesAfterMove(GameObject piece1, GameObject piece2, ICommand swapCommand)
    {
        yield return new WaitForSeconds(moveSpeed);
        var initialMatches = MatchManager.Instance.FindMatchesFromMove(piece1, piece2);
        if (initialMatches.Count > 0)
        {
            // 매치된 조각들 즉시 강조
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

        // 점수 업데이트
        int points = matches.Count * 5;
        ScoreManager.Instance.AddScore(points);
        GameManager.Instance.AddTime(matches.Count * 0.1f * (currentCombo+1));

        yield return new WaitForSeconds(0.1f);
        smoothShake.StopShake();
        IsPieceMoving = true;
        yield return StartCoroutine(DropPieces()); // 조각 떨어뜨리기

        yield return new WaitForSeconds(dropSpeed);
        var additionalMatches = MatchManager.Instance.FindAllMatches(); // 새로운 매치 검색
        if (additionalMatches.Count > 0)
        {
            yield return ClearAndDropPieces(additionalMatches);
        }
        else
        {
            IsPieceMoving = false;
        }
    }

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

        // 모든 조각을 동시에 떨어뜨리기 위한 애니메이션
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
        IncreaseCombo();
        // 빈 공간을 채우기 위해 새 조각을 생성합니다.
        FillEmptySpaces();
    }

    private void FillEmptySpaces()
    {
        List<KeyValuePair<GameObject, Vector3>> newPieces = new List<KeyValuePair<GameObject, Vector3>>();

        for (int x = 0; x < GridManager.Instance.width; x++)
        {
            for (int y = 0; y < GridManager.Instance.height; y++)
            {
                if (GridManager.Instance.GetPieceAt(x, y) == null)
                {
                    Vector3 startPosition = new Vector3(x, GridManager.Instance.height, 0);
                    Vector3 targetPosition = new Vector3(x, y, 0);
                    int pieceIndex = Random.Range(0, GridManager.Instance.pieces.Length);
                    GameObject piece = Instantiate(GridManager.Instance.pieces[pieceIndex], startPosition, Quaternion.identity, GridManager.Instance.gridParent);
                    piece.GetComponent<Piece>().index = new Vector2(x, y); // 조각의 인덱스 설정
                    GridManager.Instance.SetPieceAt(x, y, piece); // 그리드에 조각 추가

                    newPieces.Add(new KeyValuePair<GameObject, Vector3>(piece, targetPosition));
                }
            }
        }

        // 모든 새 조각을 동시에 떨어뜨리기 위한 애니메이션
        StartCoroutine(AnimateDrop(newPieces));
    }

    
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

    // 조각의 하이라이트를 해제하는 함수
    private void UnhighlightPieces(GameObject piece1, GameObject piece2)
    {

            if (piece1 != null)
            {
                piece1.GetComponent<Renderer>().material.SetFloat("_OutlineEnabled",0.0f);
            }

            if (piece2 != null)
            {
                piece2.GetComponent<Renderer>().material.SetFloat("_OutlineEnabled",0.0f);
            }


    }
    
    
    private Vector3 CalculateCenterOfMatches(List<GameObject> matches)
    {
        if (matches.Count == 0) return Vector3.zero;

        Vector3 sum = Vector3.zero;
        foreach (var match in matches)
        {
            sum += match.transform.position;
        }

        return sum / matches.Count;
    }
}

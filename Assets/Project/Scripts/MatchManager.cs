using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 내 매치 탐색 및 관리를 담당하는 클래스
/// </summary>
public class MatchManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static MatchManager Instance { get; private set; }

    /// <summary>
    /// 초기화 시 싱글톤 인스턴스 설정
    /// </summary>
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

    /// <summary>
    /// 이동한 조각으로부터 매치를 찾는 함수
    /// </summary>
    /// <param name="piece1">첫 번째 조각</param>
    /// <param name="piece2">두 번째 조각</param>
    /// <returns>매치된 조각 목록</returns>
    public List<GameObject> FindMatchesFromMove(GameObject piece1, GameObject piece2)
    {
        HashSet<GameObject> matches = new HashSet<GameObject>();

        // 두 조각에서 매치 검색
        matches.UnionWith(GetMatchesForPiece(piece1));
        matches.UnionWith(GetMatchesForPiece(piece2));

        return new List<GameObject>(matches);
    }

    /// <summary>
    /// 그리드 전체의 매치를 찾는 함수
    /// </summary>
    /// <returns>매치된 조각 목록</returns>
    public List<GameObject> FindAllMatches()
    {
        HashSet<GameObject> matches = new HashSet<GameObject>();

        for (int x = 0; x < GridManager.Instance.width; x++)
        {
            for (int y = 0; y < GridManager.Instance.height; y++)
            {
                GameObject currentPiece = GridManager.Instance.GetPieceAt(x, y);
                if (currentPiece != null)
                {
                    matches.UnionWith(GetMatchesForPiece(currentPiece));
                }
            }
        }

        return new List<GameObject>(matches);
    }

    /// <summary>
    /// 특정 조각에 대한 매치를 찾는 함수
    /// </summary>
    /// <param name="piece">매치를 찾을 조각</param>
    /// <returns>매치된 조각 집합</returns>
    private HashSet<GameObject> GetMatchesForPiece(GameObject piece)
    {
        HashSet<GameObject> matches = new HashSet<GameObject>();
        Vector2 index = piece.GetComponent<Piece>().index;

        matches.UnionWith(GetHorizontalMatch((int)index.x, (int)index.y));
        matches.UnionWith(GetVerticalMatch((int)index.x, (int)index.y));

        return matches;
    }

    /// <summary>
    /// 수평 방향 매치를 찾는 함수
    /// </summary>
    /// <param name="x">기준 x좌표</param>
    /// <param name="y">기준 y좌표</param>
    /// <returns>수평 매치된 조각 집합</returns>
    private HashSet<GameObject> GetHorizontalMatch(int x, int y)
    {
        var gridManager = GridManager.Instance; // 싱글톤 사용
        HashSet<GameObject> match = new HashSet<GameObject>();
        GameObject startPiece = gridManager.GetPieceAt(x, y);
        
        if (startPiece.tag == "Wall")
        {
            return match;
        }
        
        // 오른쪽 방향 탐색
        for (int i = x; i < gridManager.width; i++)
        {
            GameObject nextPiece = gridManager.GetPieceAt(i, y);
            if (nextPiece != null && nextPiece.tag == startPiece.tag)
            {
                match.Add(nextPiece);
            }
            else
            {
                break;
            }
        }

        // 왼쪽 방향 탐색
        for (int i = x; i >= 0; i--)
        {
            GameObject nextPiece = gridManager.GetPieceAt(i, y);
            if (nextPiece != null && nextPiece.tag == startPiece.tag)
            {
                match.Add(nextPiece);
            }
            else
            {
                break;
            }
        }

        // 매치가 3개 이상인 경우에만 반환
        if (match.Count >= 3)
        {
            return match;
        }

        return new HashSet<GameObject>();
    }

    /// <summary>
    /// 수직 방향 매치를 찾는 함수
    /// </summary>
    /// <param name="x">기준 x좌표</param>
    /// <param name="y">기준 y좌표</param>
    /// <returns>수직 매치된 조각 집합</returns>
    private HashSet<GameObject> GetVerticalMatch(int x, int y)
    {
        HashSet<GameObject> match = new HashSet<GameObject>();
        GameObject startPiece = GridManager.Instance.GetPieceAt(x, y);
        
        if (startPiece.tag == "Wall")
        {
            return match;
        }
        
        // 위쪽 방향 탐색
        for (int i = y; i < GridManager.Instance.height; i++)
        {
            GameObject nextPiece = GridManager.Instance.GetPieceAt(x, i);
            if (nextPiece != null && nextPiece.tag == startPiece.tag)
            {
                match.Add(nextPiece);
            }
            else
            {
                break;
            }
        }

        // 아래쪽 방향 탐색
        for (int i = y; i >= 0; i--)
        {
            GameObject nextPiece = GridManager.Instance.GetPieceAt(x, i);
            if (nextPiece != null && nextPiece.tag == startPiece.tag)
            {
                match.Add(nextPiece);
            }
            else
            {
                break;
            }
        }

        // 매치가 3개 이상인 경우에만 반환
        if (match.Count >= 3)
        {
            return match;
        }

        return new HashSet<GameObject>();
    }
}

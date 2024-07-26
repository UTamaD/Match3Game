using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public static MatchManager Instance { get; private set; }

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

    // 이동에 의해 발생한 매치 탐색
    public List<GameObject> FindMatchesFromMove(GameObject piece1, GameObject piece2)
    {

        HashSet<GameObject> matches = new HashSet<GameObject>();

        // 두 조각에서 매치 검색
        matches.UnionWith(GetMatchesForPiece(piece1));
        matches.UnionWith(GetMatchesForPiece(piece2));

        return new List<GameObject>(matches);
    }

    // 그리드 전체에서 매치 탐색
    public List<GameObject> FindAllMatches()
    {
        var gridManager = GridManager.Instance; // Singleton 사용
        HashSet<GameObject> matches = new HashSet<GameObject>();

        for (int x = 0; x < gridManager.width; x++)
        {
            for (int y = 0; y < gridManager.height; y++)
            {
                GameObject currentPiece = gridManager.GetPieceAt(x, y);
                if (currentPiece != null)
                {
                    matches.UnionWith(GetMatchesForPiece(currentPiece));
                }
            }
        }

        return new List<GameObject>(matches);
    }

    // 특정 조각에 대해 매치를 검색
    private HashSet<GameObject> GetMatchesForPiece(GameObject piece)
    {
        var gridManager = GridManager.Instance; // Singleton 사용
        HashSet<GameObject> matches = new HashSet<GameObject>();
        Vector2 index = piece.GetComponent<Piece>().index;

        matches.UnionWith(GetHorizontalMatch((int)index.x, (int)index.y));
        matches.UnionWith(GetVerticalMatch((int)index.x, (int)index.y));

        return matches;
    }

    // 수평 매치 탐색
    private HashSet<GameObject> GetHorizontalMatch(int x, int y)
    {

        var gridManager = GridManager.Instance; // Singleton 사용
        HashSet<GameObject> match = new HashSet<GameObject>();
        GameObject startPiece = gridManager.GetPieceAt(x, y);
        
        if (startPiece.tag == "Wall")
        {
            return match;
        }
        

        for (int i = x; i < gridManager.width; i++)
        {
            GameObject nextPiece = gridManager.GetPieceAt(i, y);
            if (nextPiece != null  && nextPiece.tag == startPiece.tag)
            {
                match.Add(nextPiece);
            }
            else
            {
                break;
            }
        }

        for (int i = x; i >= 0; i--)
        {
            GameObject nextPiece = gridManager.GetPieceAt(i, y);
            if (nextPiece != null  && nextPiece.tag == startPiece.tag)
            {
                match.Add(nextPiece);
            }
            else
            {
                break;
            }
        }

        if (match.Count >= 3)
        {
            return match;
        }

        return new HashSet<GameObject>();
    }

    // 수직 매치 탐색
    private HashSet<GameObject> GetVerticalMatch(int x, int y)
    {
        var gridManager = GridManager.Instance; // Singleton 사용
        HashSet<GameObject> match = new HashSet<GameObject>();
        GameObject startPiece = gridManager.GetPieceAt(x, y);

        
        if (startPiece.tag == "Wall")
        {
            return match;
        }

        
        for (int i = y; i < gridManager.height; i++)
        {
            GameObject nextPiece = gridManager.GetPieceAt(x, i);
            if (nextPiece != null &&nextPiece.tag == startPiece.tag)
            {
                match.Add(nextPiece);
            }
            else
            {
                break;
            }
        }

        for (int i = y; i >= 0; i--)
        {
            GameObject nextPiece = gridManager.GetPieceAt(x, i);
            if (nextPiece != null && nextPiece.tag == startPiece.tag )
            {
                match.Add(nextPiece);
            }
            else
            {
                break;
            }
        }

        if (match.Count >= 3)
        {
            return match;
        }

        return new HashSet<GameObject>();
    }
}

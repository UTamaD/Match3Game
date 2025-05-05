using UnityEngine;

/// <summary>
/// 게임 그리드 생성 및 관리를 담당하는 클래스
/// </summary>
public class GridManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GridManager Instance { get; private set; }
    
    public int width = 8;               // 그리드의 가로 길이
    public int height = 8;              // 그리드의 세로 길이
    public GameObject[] pieces;         // 조각 프리팹 배열
    public GameObject borderBlockPrefab; // 테두리 프리팹
    public Transform gridParent;        // 그리드 부모 오브젝트
    public float borderSize = 0.25f;    // 테두리 크기
    private GameObject[,] gridArray;    // 그리드 배열

    public Sprite IceBlock;             // 얼음 블록 스프라이트
    
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
    /// 게임 시작 시 그리드 초기화
    /// </summary>
    void Start()
    {
        gridArray = new GameObject[width, height];
        ResetGrid();
    }

    /// <summary>
    /// 그리드 테두리 생성 함수
    /// </summary>
    private void CreateBorder()
    {
        // 하단 테두리
        for (int x = -1; x <= width; x++)
        {
            Instantiate(borderBlockPrefab, new Vector3(x * borderSize, -1 * borderSize, 0), Quaternion.identity, gridParent);
        }
        
        // 왼쪽 테두리
        for (int y = 0; y < height+1; y++)
        {
            Instantiate(borderBlockPrefab, new Vector3(-1 * borderSize, y * borderSize, 0), Quaternion.identity, gridParent);
        }

        // 오른쪽 테두리
        for (int y = 0; y < height+1; y++)
        {
            Instantiate(borderBlockPrefab, new Vector3(width * borderSize, y * borderSize, 0), Quaternion.identity, gridParent);
        }
    }

    /// <summary>
    /// 그리드 생성 함수
    /// </summary>
    void CreateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x, y, 0);
                int pieceIndex = Random.Range(0, pieces.Length);
                GameObject piece = Instantiate(pieces[pieceIndex], position, Quaternion.identity, gridParent);
                piece.GetComponent<Piece>().index = new Vector2(x, y); // 조각의 인덱스 설정
                gridArray[x, y] = piece; // 그리드에 조각 추가
            }
        }
    }
    
    /// <summary>
    /// 모든 블록 제거 함수
    /// </summary>
    public void ClearAllBlocks()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (gridArray[x, y] != null)
                {
                    Destroy(gridArray[x, y]);
                    gridArray[x, y] = null;
                }
            }
        }
    }

    /// <summary>
    /// 십자 모양으로 블록 제거 함수 (아이템 효과)
    /// </summary>
    public void ClearCrossBlock()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if ((x > width / 2 - 2 && x < width / 2 + 1) || (y > height / 2 - 2 && y < height / 2 + 1))
                {
                    if (gridArray[x, y] != null)
                    {
                        gridArray[x, y].GetComponent<SpriteRenderer>().sprite = IceBlock;
                        gridArray[x, y].tag = "Ice";
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 그리드 초기화 함수
    /// </summary>
    public void ResetGrid()
    {
        // 기존 그리드의 모든 블록 제거
        ClearAllBlocks();
        
        // 새로운 그리드 생성
        CreateGrid();
        CreateBorder();
    }
    
    /// <summary>
    /// 특정 위치의 조각을 반환하는 함수
    /// </summary>
    /// <param name="x">x 좌표</param>
    /// <param name="y">y 좌표</param>
    /// <returns>해당 위치의 게임 오브젝트</returns>
    public GameObject GetPieceAt(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return gridArray[x, y];
        }
        return null;
    }

    /// <summary>
    /// 특정 위치에 조각을 설정하는 함수
    /// </summary>
    /// <param name="x">x 좌표</param>
    /// <param name="y">y 좌표</param>
    /// <param name="piece">설정할 게임 오브젝트</param>
    public void SetPieceAt(int x, int y, GameObject piece)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            gridArray[x, y] = piece;
        }
    }
}
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    
    public int width = 8; // 그리드의 가로 길이
    public int height = 8; // 그리드의 세로 길이
    public GameObject[] pieces; // 조각 프리팹 배열
    public GameObject borderBlockPrefab;//테두리 프리팹 배열
    public Transform gridParent; // 그리드 부모 오브젝트
    public float borderSize = 0.25f;
    private GameObject[,] gridArray; // 그리드 배열

    public Sprite IceBlock;
    
    

    
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
    
    void Start()
    {
        gridArray = new GameObject[width, height];
        ResetGrid();
    }

    private void CreateBorder()
    {
        // Bottom 
        for (int x = -1; x <= width; x++)
        {
            Instantiate(borderBlockPrefab, new Vector3(x * borderSize, -1 * borderSize, 0), Quaternion.identity, gridParent);
        }
        
        //Left
        for (int y = 0; y < height+1; y++)
        {
            Instantiate(borderBlockPrefab, new Vector3(-1 * borderSize, y * borderSize, 0), Quaternion.identity, gridParent);
        }

        // Right
        for (int y = 0; y < height+1; y++)
        {
            Instantiate(borderBlockPrefab, new Vector3(width * borderSize, y * borderSize, 0), Quaternion.identity, gridParent);
        }
    }
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

    public void ClearCrossBlock()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                if ((x > width / 2 - 2 && x < width / 2 + 1) || (y > height / 2 - 2 && y < height / 2 + 1))
                {
                    if (gridArray[x, y] != null )
                    {
                        gridArray[x, y].GetComponent<SpriteRenderer>().sprite = IceBlock;
                        gridArray[x, y].tag = "Ice";
                    }
                }

            }
        }
    }
    
    public void ResetGrid()
    {
        // 기존 그리드의 모든 블록 제거
        ClearAllBlocks();
        
        // 새로운 그리드 생성
        CreateGrid();
        CreateBorder();
    }
    
    // 특정 위치의 조각을 반환
    public GameObject GetPieceAt(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return gridArray[x, y];
        }
        return null;
    }

    // 특정 위치에 조각을 설정
    public void SetPieceAt(int x, int y, GameObject piece)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            gridArray[x, y] = piece;
        }
    }
}
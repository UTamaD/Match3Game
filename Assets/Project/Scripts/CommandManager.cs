using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 명령 인터페이스 - 커맨드 패턴 구현
/// </summary>
public interface ICommand
{
    void Execute();  // 명령 실행
    void Undo();     // 명령 취소
}

/// <summary>
/// 조각 교환 명령 클래스
/// </summary>
public class SwapCommand : ICommand
{
    private GameObject piece1;    // 첫 번째 조각
    private GameObject piece2;    // 두 번째 조각
    private float moveSpeed;      // 이동 속도

    /// <summary>
    /// 조각 교환 명령 생성자
    /// </summary>
    /// <param name="piece1">첫 번째 조각</param>
    /// <param name="piece2">두 번째 조각</param>
    /// <param name="moveSpeed">이동 속도</param>
    public SwapCommand(GameObject piece1, GameObject piece2, float moveSpeed)
    {
        this.piece1 = piece1;
        this.piece2 = piece2;
        this.moveSpeed = moveSpeed;
    }

    /// <summary>
    /// 명령 실행 함수
    /// </summary>
    public void Execute()
    {
        CommandManager.Instance.StartCoroutine(SwapWithAnimation(piece1, piece2));
        AudioManager.Instance.PlaySwapSound();
    }

    /// <summary>
    /// 명령 취소 함수
    /// </summary>
    public void Undo()
    {
        CommandManager.Instance.StartCoroutine(SwapWithAnimation(piece2, piece1));
        AudioManager.Instance.PlayUndoSound();
    }

    /// <summary>
    /// 애니메이션과 함께 조각 교환 코루틴
    /// </summary>
    /// <param name="piece1">첫 번째 조각</param>
    /// <param name="piece2">두 번째 조각</param>
    /// <returns>대기 시간</returns>
    private IEnumerator SwapWithAnimation(GameObject piece1, GameObject piece2)
    {
        Vector3 position1 = piece1.transform.position;
        Vector3 position2 = piece2.transform.position;

        float elapsedTime = 0;
        while (elapsedTime < moveSpeed)
        {
            piece1.transform.position = Vector3.Lerp(position1, position2, (elapsedTime / moveSpeed));
            piece2.transform.position = Vector3.Lerp(position2, position1, (elapsedTime / moveSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        piece1.transform.position = position2;
        piece2.transform.position = position1;

        // 인덱스 교환
        Vector2 tempIndex = piece1.GetComponent<Piece>().index;
        piece1.GetComponent<Piece>().index = piece2.GetComponent<Piece>().index;
        piece2.GetComponent<Piece>().index = tempIndex;

        // 그리드 업데이트
        GridManager.Instance.SetPieceAt((int)piece1.GetComponent<Piece>().index.x, (int)piece1.GetComponent<Piece>().index.y, piece1);
        GridManager.Instance.SetPieceAt((int)piece2.GetComponent<Piece>().index.x, (int)piece2.GetComponent<Piece>().index.y, piece2);
    }
}

/// <summary>
/// 명령 패턴 관리 클래스
/// </summary>
public class CommandManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static CommandManager Instance { get; private set; }

    private Stack<ICommand> commandStack = new();  // 명령 스택

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
    /// 명령 실행 함수
    /// </summary>
    /// <param name="command">실행할 명령</param>
    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        commandStack.Push(command);
    }

    /// <summary>
    /// 명령 취소 함수
    /// </summary>
    public void UndoCommand()
    {
        if (commandStack.Count > 0)
        {
            ICommand command = commandStack.Pop();
            command.Undo();
        }
    }
}

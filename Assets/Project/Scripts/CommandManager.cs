using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommand
{
    void Execute();
    void Undo();
}

public class SwapCommand : ICommand
{
    private GameObject piece1;
    private GameObject piece2;
    private float moveSpeed;

    public SwapCommand(GameObject piece1, GameObject piece2, float moveSpeed)
    {
        this.piece1 = piece1;
        this.piece2 = piece2;
        this.moveSpeed = moveSpeed;
    }

    public void Execute()
    {
        CommandManager.Instance.StartCoroutine(SwapWithAnimation(piece1, piece2));
        AudioManager.Instance.PlaySwapSound();
    }

    public void Undo()
    {
        CommandManager.Instance.StartCoroutine(SwapWithAnimation(piece2, piece1));
        AudioManager.Instance.PlayUndoSound();
    }

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

public class CommandManager : MonoBehaviour
{
    public static CommandManager Instance { get; private set; }

    private Stack<ICommand> commandStack = new();

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

    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        commandStack.Push(command);
    }

    public void UndoCommand()
    {
        if (commandStack.Count > 0)
        {
            ICommand command = commandStack.Pop();
            command.Undo();
        }
    }
}

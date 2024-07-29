using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    private GameObject firstPiece;
    private GameObject secondPiece;

    private bool inputEnabled = true;

    public void SetInputEnabled(bool enable)
    {
        inputEnabled = enable;
    }
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

    private void Start()
    {
        inputEnabled = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && PieceManager.Instance.IsPieceMoving == false && inputEnabled)
        {
            Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D collider = Physics2D.OverlapPoint(clickPosition);


            if (collider != null)
            {
                if (firstPiece == null)
                {

                    firstPiece = collider.gameObject;
                    AudioManager.Instance.PlaySelectSound();
   
                    firstPiece.GetComponent<Piece>().HighlightPiece(true);
                }
                else
                {

                    secondPiece = collider.gameObject;
                    AudioManager.Instance.PlaySelectSound();
                    
                    secondPiece.GetComponent<Piece>().HighlightPiece(true);
                    PieceManager.Instance.SwapPieces(firstPiece, secondPiece);
                    
                    
                    firstPiece = null;
                    secondPiece = null;
                    

                }
            }
        }
    }
}
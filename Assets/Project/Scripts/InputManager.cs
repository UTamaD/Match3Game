using UnityEngine;

public class InputManager : MonoBehaviour
{


    private GameObject firstPiece;
    private GameObject secondPiece;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D collider = Physics2D.OverlapPoint(clickPosition);


            if (collider != null && PieceManager.Instance.IsPieceMoving == false)
            {
                if (firstPiece == null)
                {

                    firstPiece = collider.gameObject;
                    AudioManager.Instance.PlaySelectSoubd();
                    //firstPiece.GetComponent<SpriteRenderer>().color = Color.red;
                    firstPiece.GetComponent<Renderer>().material.SetFloat("_OutlineEnabled",1.0f);
                    
                }
                else
                {

                    secondPiece = collider.gameObject;
                    AudioManager.Instance.PlaySelectSoubd();
                    //secondPiece.GetComponent<SpriteRenderer>().color = Color.red;
                    secondPiece.GetComponent<Renderer>().material.SetFloat("_OutlineEnabled",1.0f);
                    PieceManager.Instance.SwapPieces(firstPiece, secondPiece);
                    
                    
                    firstPiece = null;
                    secondPiece = null;
                    

                }
            }
        }
    }
}
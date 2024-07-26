using UnityEngine;

public class Piece : MonoBehaviour
{
    public Vector2 index;
    private Renderer pieceRenderer;

    private void Awake()
    {
        pieceRenderer = GetComponent<Renderer>();
    }

    public void HighlightPiece(bool highlight)
    {
        pieceRenderer.material.SetFloat("_OutlineEnabled", highlight ? 1.0f : 0.0f);
    }
}
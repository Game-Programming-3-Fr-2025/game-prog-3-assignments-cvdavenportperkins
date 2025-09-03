using UnityEngine;

public class ShapeVisualController : MonoBehaviour
{
    [SerializeField] private GameObject circleVisual;
    [SerializeField] private GameObject squareVisual;
    [SerializeField] private GameObject triangleVisual;

    public void SetFactionVisual(FactionType faction)
    {
        ShapeType shape = FactionManager.GetShape(faction);
        Color color = FactionManager.GetColor(faction);

        SetShape(shape);
        SetColor(color);
    }
    public void SetShape(ShapeType shape)
    {
        circleVisual.SetActive(shape == ShapeType.Circle);
        squareVisual.SetActive(shape == ShapeType.Square);
        triangleVisual.SetActive(shape == ShapeType.Triangle);
    }

    public void SetColor(Color color)
    {
        var activeVisual = GetActiveVisual();
        var renderer = activeVisual.GetComponent<SpriteRenderer>();
        if (renderer != null) renderer.color = color;
    }

    private GameObject GetActiveVisual()
    {
        if (circleVisual.activeSelf) return circleVisual;
        if (squareVisual.activeSelf) return squareVisual;
        if (triangleVisual.activeSelf) return triangleVisual;
        return null;
    }



}



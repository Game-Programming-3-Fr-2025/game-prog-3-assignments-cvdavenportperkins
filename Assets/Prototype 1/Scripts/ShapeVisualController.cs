using UnityEngine;


namespace PrototypeOne
{

    public class ShapeVisualController : MonoBehaviour
    {
        [SerializeField] private GameObject circleVisual;
        [SerializeField] private GameObject squareVisual;
        [SerializeField] private GameObject triangleVisual;
        [SerializeField] private GameObject capsuleVisual;

        private void Awake()
        {

        }

        public void SetFactionVisual(FactionType faction)
        {
            ShapeType shape = FactionManager.GetShape(faction);
            Color color = FactionManager.GetColor(faction);

            SetShape(shape);
            SetColor(color);
        }

        public void SetShape(ShapeType shape)
        {
            if (circleVisual != null) circleVisual.SetActive(shape == ShapeType.Circle);
            if (squareVisual != null) squareVisual.SetActive(shape == ShapeType.Square);
            if (triangleVisual != null) triangleVisual.SetActive(shape == ShapeType.Triangle);
            if (capsuleVisual != null) capsuleVisual.SetActive(shape == ShapeType.Capsule);
        }

        public void SetColor(Color color)
        {
            var activeVisual = GetActiveVisual();
            if (activeVisual == null) return;
            var sr = activeVisual.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = color;
        }

        private GameObject GetActiveVisual()
        {
            if (circleVisual != null && circleVisual.activeSelf) return circleVisual;
            if (squareVisual != null && squareVisual.activeSelf) return squareVisual;
            if (triangleVisual != null && triangleVisual.activeSelf) return triangleVisual;
            if (capsuleVisual != null && capsuleVisual.activeSelf) return capsuleVisual;
            return null;
        }

    }
}
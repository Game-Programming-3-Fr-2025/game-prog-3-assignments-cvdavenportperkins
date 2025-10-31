using UnityEngine;

public class GradientBuilder : MonoBehaviour
{
    public Gradient gradual;
    public AnimationCurve progression;
    public float timer;
    public Color color;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        timer = Mathf.PingPong(timer, 1);

        color = gradual.Evaluate(progression.Evaluate(timer));
       
    }
}

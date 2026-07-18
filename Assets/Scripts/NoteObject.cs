using UnityEngine;

public class NoteObject : MonoBehaviour
{
    private float _speed = 0.65f;
    private float _xThereshold;

    public void Initialize(float speed, float xThreshold)
    {
        _speed = speed;
        _xThereshold = xThreshold;
    }

    public void SetXThereshold(float value)
    {
        _xThereshold = value;
    }

    void Update()
    {
        transform.position += new Vector3(-_speed, 0f, 0f) * Time.deltaTime;

        if (transform.position.x < _xThereshold)
        {
            Destroy(gameObject);
        }
    }
}

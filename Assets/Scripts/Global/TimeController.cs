using UnityEngine;

public class TimeController : MonoBehaviour
{
    [SerializeField]
    private float dayDurationSeconds = 30f;

    [SerializeField]
    private float nightDurationSeconds = 30f;

    public bool IsNight { get; private set; }

    private float timeRemaining;

    void Start()
    {
        IsNight = false;
        timeRemaining = Mathf.Max(0.1f, dayDurationSeconds);
    }

    void Update()
    {
        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            Debug.Log("Noite? " + IsNight);
            IsNight = !IsNight;
            timeRemaining = Mathf.Max(0.1f, IsNight ? nightDurationSeconds : dayDurationSeconds);
        }
    }
}

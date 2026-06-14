using UnityEngine;

public class TimeController : MonoBehaviour
{
    [SerializeField]
    private float dayDurationSeconds = 30f;

    [SerializeField]
    private float nightDurationSeconds = 30f;

    public bool IsNight { get; private set; }

    private float timeRemaining;

    private void Start()
    {
        IsNight = false;
        timeRemaining = Mathf.Max(1f, dayDurationSeconds);
        Debug.Log("Dia começou");
    }

    private void Update()
    {
        if (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;
        }

        if (timeRemaining > 0f)
        {
            return;
        }

        IsNight = !IsNight;
        timeRemaining = Mathf.Max(1f, IsNight ? nightDurationSeconds : dayDurationSeconds);
        Debug.Log(IsNight ? "Noite começou" : "Dia começou");
    }

    private void OnValidate()
    {
        dayDurationSeconds = Mathf.Max(1f, dayDurationSeconds);
        nightDurationSeconds = Mathf.Max(1f, nightDurationSeconds);
    }
}

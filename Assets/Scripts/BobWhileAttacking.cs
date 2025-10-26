using UnityEngine;

public class BobWhileAttacking : MonoBehaviour
{
    [Header("Bob Settings")]
    public float amplitude = 10f;    // For UI, use small pixel values (e.g. 10)
    public float frequency = 2f;     // Oscillation speed

    private Vector3 startPos;
    private RectTransform rectTransform;
    private bool isUI;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        isUI = rectTransform != null;

        if (isUI)
            startPos = rectTransform.anchoredPosition;
        else
            startPos = transform.localPosition;
    }

    void Update()
    {
        if (AttackManager.Instance != null && AttackManager.Instance.inAttackLoop)
        {
            float offset = Mathf.Sin(Time.time * frequency) * amplitude;

            if (isUI)
            {
                rectTransform.anchoredPosition = new Vector2(startPos.x, startPos.y + offset);
            }
            else
            {
                transform.localPosition = new Vector3(startPos.x, startPos.y + offset, startPos.z);
            }
        }
        else
        {
            // Reset to starting position when not in attack loop
            if (isUI)
                rectTransform.anchoredPosition = startPos;
            else
                transform.localPosition = startPos;
        }
    }
}

using System.Collections;
using UnityEngine;

public class EnergyNodeAnimator : MonoBehaviour
{
    [Header("Rotation")]
    public float rotateSpeed = 90f;         // degrees per second on Y axis

    [Header("Bob")]
    public float bobAmplitude = 0.3f;       // units up/down
    public float bobFrequency = 1.5f;       // full cycles per second

    [Header("Collect Pulse")]
    public float pulseScale    = 1.5f;      // peak scale multiplier
    public float pulseUpTime   = 0.12f;     // seconds to reach peak
    public float pulseDownTime = 0.08f;     // seconds to shrink to zero

    private float    startY;
    private Vector3  baseScale;
    private bool     collected = false;

    private void Start()
    {
        startY    = transform.position.y;
        baseScale = transform.localScale;
    }

    private void Update()
    {
        if (collected) return;

        // ── Spin ─────────────────────────────────────────────────────────
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f, Space.World);

        // ── Bob ───────────────────────────────────────────────────────────
        // Offset Y using a sine wave anchored to the spawn position.
        // Multiplying Time.time by 2π converts frequency from cycles/sec to rad/sec.
        float bobOffset = Mathf.Sin(Time.time * bobFrequency * Mathf.PI * 2f) * bobAmplitude;
        Vector3 pos = transform.position;
        pos.y = startY + bobOffset;
        transform.position = pos;
    }

    // Called by EnergyNode instead of SetActive(false)
    public void Collect()
    {
        collected = true;
        StartCoroutine(CollectPulse());
    }

    // ── Collect pulse ─────────────────────────────────────────────────────
    // Scales the node to pulseScale× over pulseUpTime seconds to give a
    // satisfying "pop", then shrinks it to zero before destroying.

    private IEnumerator CollectPulse()
    {
        Vector3 peakScale = baseScale * pulseScale;

        // Scale up to peak
        yield return LerpScale(baseScale, peakScale, pulseUpTime);

        // Shrink to nothing
        yield return LerpScale(peakScale, Vector3.zero, pulseDownTime);

        Destroy(gameObject);
    }

    private IEnumerator LerpScale(Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        transform.localScale = to;
    }
}

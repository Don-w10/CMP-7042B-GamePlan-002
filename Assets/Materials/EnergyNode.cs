using UnityEngine;

public class EnergyNode : MonoBehaviour
{
    public GameManager gameManager;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            collected = true;
            gameManager.CollectNode();

            // Play the scale-pulse animation then self-destruct.
            // Falls back to immediate deactivation if no animator is present.
            var animator = GetComponent<EnergyNodeAnimator>();
            if (animator != null)
                animator.Collect();
            else
                gameObject.SetActive(false);
        }
    }
}
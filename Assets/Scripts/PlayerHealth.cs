using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float currentHealth;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private Image redSplatterImage;
    [SerializeField] private Image hurtImage;
    [SerializeField] private float fadeDuration = 0.1f; 

    [SerializeField] private bool showBlood;

    private void Start()
    {
        currentHealth = maxHealth;
        redSplatterImage.color = new Color(1, 0, 0, 0); // Ensure the splatter starts invisible
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damage;
            UpdateHealth();
        }
    }

    private void UpdateHealth()
    {
        StartCoroutine(HurtFlash());
        if (showBlood) 
        {
            Color splatterAlpha = redSplatterImage.color;
            splatterAlpha.a = 1 - (currentHealth / maxHealth);
            redSplatterImage.color = splatterAlpha; // Применить изменения цвета обратно на изображение
        }
    }

    private IEnumerator HurtFlash()
    {
        hurtImage.enabled = true;
        hurtImage.color = new Color(hurtImage.color.r, hurtImage.color.g, hurtImage.color.b, 0.6f);
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            float newAlpha = 0.6f - (elapsedTime / fadeDuration);
            hurtImage.color = new Color(hurtImage.color.r, hurtImage.color.g, hurtImage.color.b, newAlpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        hurtImage.enabled = false; // Выключаем изображение после угасания
        hurtImage.color = new Color(hurtImage.color.r, hurtImage.color.g, hurtImage.color.b, 0.6f); // Возвращаем начальный альфа-канал, если это необходимо
    }
}
 
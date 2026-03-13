using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DiceRoller : MonoBehaviour
{
    public Image diceImage;
    public Sprite[] diceFaces;

    public int finalValue;

    public GameObject GC;

    // Button will call this
    public void RollDice()
    { 
        GameCalculator gc = GC.GetComponent<GameCalculator>();

        gc.ResetCards();     // hide frames
        gc.ClearFields();    // clear equation fields

        StartCoroutine(RollDiceRoutine());
    }

    public IEnumerator RollDiceRoutine()
    {
        float rollTime = 1.5f;
        float elapsed = 0f;

        Vector3 originalPos = transform.position;

        while (elapsed < rollTime)
        {
            int randomFace = Random.Range(0, 6);
            diceImage.sprite = diceFaces[randomFace];

            // Dice shake
            transform.position = originalPos + Random.insideUnitSphere * 2f;

            yield return new WaitForSeconds(0.08f);
            elapsed += 0.08f;
        }

        // reset position
        transform.position = originalPos;

        finalValue = Random.Range(1, 7);
        diceImage.sprite = diceFaces[finalValue - 1];
    }
}
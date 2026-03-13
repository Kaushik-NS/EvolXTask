using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class GameCalculator : MonoBehaviour
{
    public DiceRoller dice;

    public TMP_InputField pointsField;
    public TMP_InputField multiplierField;
    public TMP_InputField totalField;

    public Image totalHighlight;

    public Image cardAFrame;
    public Image cardBFrame;

    public GameObject MainMenuPanel;



    public RawImage cardAImage;
    public RawImage cardBImage;

    public AudioSource DiceRollSound;
    public AudioSource PlayButtonSound;
    public AudioSource ExitButtonSound;
    public AudioSource RGBframeSound;

    int points = 0;     
    int multiplier = 10;
    int total = 0;

    public void Roll()
    {
        DiceRollSound.Play();
        ClearFields();          // clear UI first
        StartCoroutine(RollSequence());
    }

    public void ClearFields()
    {
        pointsField.text = "";
        multiplierField.text = "";
        totalField.text = "";
    }

    IEnumerator HideFrameAfterDelay(GameObject frame, float delay)
    {
        yield return new WaitForSeconds(delay);
        frame.SetActive(false);
    }


    IEnumerator RollSequence()
    {
        yield return StartCoroutine(dice.RollDiceRoutine());

        int result = dice.finalValue;

        // Default values after roll
        points = result;
        multiplier = 10;

        ApplySpiritCards(result);

        total = points * multiplier;

        UpdateUI();
    }

    IEnumerator HighlightTotal()
    {
        totalHighlight.gameObject.SetActive(true);

        Color original = totalHighlight.color;

        totalHighlight.color = Color.yellow;
        totalHighlight.transform.localScale = Vector3.one * 1.2f;

        // stay visible for 2 seconds
        yield return new WaitForSeconds(2f);

        totalHighlight.color = original;
        totalHighlight.transform.localScale = Vector3.one;

        totalHighlight.gameObject.SetActive(false);
    }

    IEnumerator RGBFrame(Image frame)
    {
        RGBframeSound.Play();
        while (frame.gameObject.activeSelf)
        {
            frame.color = Color.HSVToRGB(
                Mathf.PingPong(Time.time * 0.5f, 1f),
                1f,
                1f
            );

            yield return null;
        }
    }


    void ApplySpiritCards(int diceResult)
    {
        if (diceResult == 6)
        {
            multiplier = 2;

            cardAFrame.gameObject.SetActive(true);
            StartCoroutine(RGBFrame(cardAFrame));

            StartCoroutine(HideFrameAfterDelay(cardAFrame.gameObject, 2f));
        }

        if (diceResult == 3)
        {
            points += 10;

            cardBFrame.gameObject.SetActive(true);
            StartCoroutine(RGBFrame(cardBFrame));

            StartCoroutine(HideFrameAfterDelay(cardBFrame.gameObject, 2f));
        }
    }

    public void ResetCards()
    {
        cardAFrame.gameObject.SetActive(false);
        cardBFrame.gameObject.SetActive(false);
    }

    IEnumerator CountUp(TMPro.TMP_InputField field, int target, bool triggerHighlight = false)
    {
        int current = 0;

        while (current < target)
        {
            current += Mathf.CeilToInt(target / 20f);

            if (current > target)
                current = target;

            field.text = current.ToString();

            yield return new WaitForSeconds(0.03f);
        }

        // trigger highlight only after final value
        if (triggerHighlight)
        {
            StartCoroutine(HighlightTotal());
        }
    }

    void UpdateUI()
    {
        StartCoroutine(CountUp(pointsField, points));
        StartCoroutine(CountUp(multiplierField, multiplier));
        StartCoroutine(CountUp(totalField, total, true));
    }

    public void StartGame()
    {
        PlayButtonSound.Play();
        MainMenuPanel.SetActive(false);
    }

    public void ExitGame()
    {
        ExitButtonSound.Play();
        Application.Quit();
    }
}

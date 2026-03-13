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

    public GameObject MainMenuPanel;
    public GameObject GamePanel;
    public GameObject RollButton;

    public Image totalHighlight;

    public Image cardAFrame;
    public Image cardBFrame;

    public RawImage cardAImage;
    public RawImage cardBImage;

    public AudioSource DiceRollSound;
    public AudioSource PlayButtonSound;
    public AudioSource ExitButtonSound;
    public AudioSource RGBframeSound;

    int points;
    int multiplier;
    int total;

    public void Roll()
    {
        RollButton.SetActive(false);
        DiceRollSound.Play();
        ClearFields();
        dice.RollDice();
    }

    void ClearFields()
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

    IEnumerator HighlightTotal()
    {
        totalHighlight.gameObject.SetActive(true);

        Color original = totalHighlight.color;

        Color c = Color.yellow;
        c.a = 0.25f;
        totalHighlight.color = c;

        totalHighlight.transform.localScale = Vector3.one * 1.2f;

        // stay visible for 2 seconds
        yield return new WaitForSeconds(2f);

        totalHighlight.color = original;
        totalHighlight.transform.localScale = Vector3.one;

        totalHighlight.gameObject.SetActive(false);
        RollButton.SetActive(true);
    }

    //IEnumerator RGBFrame(Image frame)
    //{
    //    RGBframeSound.Play();
    //    while (frame.gameObject.activeSelf)
    //    {
    //        frame.color = Color.HSVToRGB(
    //            Mathf.PingPong(Time.time * 0.5f, 1f),
    //            1f,
    //            1f
    //        );

    //        yield return null;
    //    }
    //}

    //IEnumerator RGBFrame(Image frame)
    //{
    //    RGBframeSound.Play();

    //    while (RGBframeSound.isPlaying)
    //    {
    //        frame.color = Color.HSVToRGB(
    //            Mathf.PingPong(Time.time * 0.5f, 1f),
    //            1f,
    //            1f
    //        );

    //        yield return null;
    //    }

    //    frame.color = Color.white;
    //}

    IEnumerator RGBFrame(Image frame, RawImage image)
    {
        RGBframeSound.Play();

        RectTransform frameRect = frame.rectTransform;
        RectTransform imageRect = image.rectTransform;

        Vector3 frameOriginal = frameRect.localScale;
        Vector3 imageOriginal = imageRect.localScale;

        while (RGBframeSound.isPlaying)
        {
            // RGB color animation
            frame.color = Color.HSVToRGB(
                Mathf.PingPong(Time.time * 0.5f, 1f),
                1f,
                1f
            );

            // pulse scale
            float scale = 1f + Mathf.PingPong(Time.time * 2f, 0.25f);

            frameRect.localScale = frameOriginal * scale;
            imageRect.localScale = imageOriginal * scale;

            yield return null;
        }

        // reset
        frame.color = Color.white;
        frameRect.localScale = frameOriginal;
        imageRect.localScale = imageOriginal;
    }



    void ApplySpiritCards(int diceResult)
    {
        if (diceResult == 6)
        {
            multiplier = 2;

            cardAFrame.gameObject.SetActive(true);
            StartCoroutine(RGBFrame(cardAFrame, cardAImage));

            StartCoroutine(HideFrameAfterDelay(cardAFrame.gameObject, 2f));
        }

        if (diceResult == 3)
        {
            points += 10;

            cardBFrame.gameObject.SetActive(true);
            StartCoroutine(RGBFrame(cardBFrame, cardBImage));

            StartCoroutine(HideFrameAfterDelay(cardBFrame.gameObject, 2f));
        }
       
    }

    public void OnDiceRollFinished(int result)
    {
        points = result;
        multiplier = 10;

        if (result == 6)
            multiplier = 2;

        if (result == 3)
            points += 10;

        total = points * multiplier;

        UpdateUI();
        ApplySpiritCards(result);
       
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
        //pointsField.text = points.ToString();
        //multiplierField.text = multiplier.ToString();
        //totalField.text = total.ToString();


        StartCoroutine(CountUp(pointsField, points));
        StartCoroutine(CountUp(multiplierField, multiplier));
        StartCoroutine(CountUp(totalField, total, true));
    }

    public void PlayGame()
    {
        PlayButtonSound.Play();
        MainMenuPanel.SetActive(false);
        GamePanel.SetActive(true);
    }

    public void ExitGame()
    {
        ExitButtonSound.Play();
        Application.Quit();
    }
}
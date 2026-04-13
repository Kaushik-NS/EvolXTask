using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class GameCalculator : MonoBehaviour
{
    // Reference to DiceRoller script (controls dice animation and result)
    public DiceRoller dice;

    // UI input fields used to display points, multiplier and total score
    public TMP_InputField pointsField;
    public TMP_InputField multiplierField;
    public TMP_InputField totalField;

    // Panels for menu and gameplay
    public GameObject MainMenuPanel;
    public GameObject GamePanel;

    // Roll button that triggers the dice roll
    public GameObject RollButton;

    // UI highlight used when displaying the final score
    public Image totalHighlight;

    // Frames that highlight the special cards
    public Image cardAFrame;
    public Image cardBFrame;

    // Images for the card graphics
    public RawImage cardAImage;
    public RawImage cardBImage;

    // Audio sources used for various game actions
    public AudioSource DiceRollSound;
    public AudioSource PlayButtonSound;
    public AudioSource ExitButtonSound;
    public AudioSource RGBframeSound;

    // Internal score variables
    int points;
    int multiplier;
    int total;

    // Called when the player presses the Roll button
    public void Roll()
    {
        // Disable the roll button to prevent multiple rolls during animation
        RollButton.SetActive(false);

        // Play dice rolling sound
        DiceRollSound.Play();

        // Clear previous score values from the UI
        ClearFields();

        // Trigger the dice roll animation
        dice.RollDice();
    }

    // Clears the score fields in the UI
    void ClearFields()
    {
        pointsField.text = "";
        multiplierField.text = "";
        totalField.text = "";
    }

    // Coroutine to hide card highlight frames after a delay
    IEnumerator HideFrameAfterDelay(GameObject frame, float delay)
    {
        yield return new WaitForSeconds(delay);
        frame.SetActive(false);
    }

    // Coroutine that highlights the total score briefly
    IEnumerator HighlightTotal()
    {
        // Show highlight background
        totalHighlight.gameObject.SetActive(true);

        // Store original color so we can restore it later
        Color original = totalHighlight.color;

        // Set highlight color to semi-transparent yellow
        Color c = Color.yellow;
        c.a = 0.25f;
        totalHighlight.color = c;

        // Slight scale increase for emphasis
        totalHighlight.transform.localScale = Vector3.one * 1.2f;

        // Keep highlight visible for 2 seconds
        yield return new WaitForSeconds(2f);

        // Restore original appearance
        totalHighlight.color = original;
        totalHighlight.transform.localScale = Vector3.one;

        // Hide highlight
        totalHighlight.gameObject.SetActive(false);

        // Re-enable roll button after the entire animation finishes
        RollButton.SetActive(true);
    }

    // Coroutine that applies RGB animation and scale pulse to card frames
    IEnumerator RGBFrame(Image frame, RawImage image)
    {
        // Play special card activation sound
        RGBframeSound.Play();

        // Cache references to RectTransforms
        RectTransform frameRect = frame.rectTransform;
        RectTransform imageRect = image.rectTransform;

        // Store original scales so they can be restored later
        Vector3 frameOriginal = frameRect.localScale;
        Vector3 imageOriginal = imageRect.localScale;

        // Run animation while sound is playing
        while (RGBframeSound.isPlaying)
        {
            // Animate frame color using HSV to create a rainbow effect
            frame.color = Color.HSVToRGB(
                Mathf.PingPong(Time.time * 0.5f, 1f),
                1f,
                1f
            );

            // Create a pulsing scale animation
            float scale = 1f + Mathf.PingPong(Time.time * 2f, 0.25f);

            frameRect.localScale = frameOriginal * scale;
            imageRect.localScale = imageOriginal * scale;

            yield return null;
        }

        // Reset color and scale after animation ends
        frame.color = Color.white;
        frameRect.localScale = frameOriginal;
        imageRect.localScale = imageOriginal;
    }

    // Applies special card effects based on the dice result
    void ApplySpiritCards(int diceResult)
    {
        // Card A: Double multiplier when dice = 6
        if (diceResult == 6)
        {
            multiplier = 2;

            cardAFrame.gameObject.SetActive(true);

            // Start RGB animation on card frame
            StartCoroutine(RGBFrame(cardAFrame, cardAImage));

            // Hide frame after delay
            StartCoroutine(HideFrameAfterDelay(cardAFrame.gameObject, 2f));
        }

        // Card B: Add bonus points when dice = 3
        if (diceResult == 3)
        {
            points += 10;

            cardBFrame.gameObject.SetActive(true);

            // Start RGB animation on card frame
            StartCoroutine(RGBFrame(cardBFrame, cardBImage));

            // Hide frame after delay
            StartCoroutine(HideFrameAfterDelay(cardBFrame.gameObject, 2f));
        }
    }

    // Called by DiceRoller when the dice animation finishes
    public void OnDiceRollFinished(int result)
    {
        // Base points equal the dice result
        points = result;

        // Default multiplier
        multiplier = 10;

        // Apply special card logic
        if (result == 6)
            multiplier = 2;

        if (result == 3)
            points += 10;

        // Calculate total score
        total = points * multiplier;

        // Update UI with animated counting
        UpdateUI();

        // Activate card effects if needed
        ApplySpiritCards(result);
    }

    // Resets card frames when needed
    public void ResetCards()
    {
        cardAFrame.gameObject.SetActive(false);
        cardBFrame.gameObject.SetActive(false);
    }

    // Coroutine that animates numbers counting up in UI fields
    IEnumerator CountUp(TMPro.TMP_InputField field, int target, bool triggerHighlight = false)
    {
        int current = 0;

        // Increment value gradually until reaching target
        while (current < target)
        {
            current += Mathf.CeilToInt(target / 20f);

            if (current > target)
                current = target;

            field.text = current.ToString();

            yield return new WaitForSeconds(0.03f);
        }

        // Trigger highlight after the total finishes counting
        if (triggerHighlight)
        {
            StartCoroutine(HighlightTotal());
        }
    }

    // Updates all UI score fields with animated counting
    void UpdateUI()
    {
        StartCoroutine(CountUp(pointsField, points));
        StartCoroutine(CountUp(multiplierField, multiplier));
        StartCoroutine(CountUp(totalField, total, true));
    }

    // Called when the Play button is pressed
    public void PlayGame()
    {
        PlayButtonSound.Play();

        // Hide main menu and show gameplay UI
        MainMenuPanel.SetActive(false);
        GamePanel.SetActive(true);
    }

    // Called when the Exit button is pressed
    public void ExitGame()
    {
        ExitButtonSound.Play();

        // Close the application
        Application.Quit();
    }
}
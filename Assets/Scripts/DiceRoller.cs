using UnityEngine;
using System.Collections;

public class DiceRoller : MonoBehaviour
{
    // Reference to the dice object in the scene
    public Transform dice;

    // Reference to the GameCalculator script that handles scoring
    public GameCalculator calculator;

    // Stores the final dice value after rolling
    public int finalValue;

    // Called when the player presses the Roll button
    public void RollDice()
    {
        // Start the dice roll animation coroutine
        StartCoroutine(RollRoutine());
    }

    IEnumerator RollRoutine()
    {
        // Total time the roll animation should run
        float rollTime = 1.5f;

        // Timer to track animation progress
        float elapsed = 0f;

        // Store the starting position of the dice
        Vector3 startPos = dice.position;

        // Choose the dice result BEFORE the animation begins
        // This ensures deterministic gameplay while keeping animation random
        finalValue = Random.Range(1, 7);

        // Run the roll animation for the specified time
        while (elapsed < rollTime)
        {
            // Continuously rotate the dice on all axes to simulate rolling
            dice.Rotate(
                Random.Range(500f, 800f) * Time.deltaTime,
                Random.Range(500f, 800f) * Time.deltaTime,
                Random.Range(500f, 800f) * Time.deltaTime
            );

            // Scale animation: makes the dice grow and shrink during the roll
            // This simulates a jump-like effect without changing position
            float scale = 1f + Mathf.Sin((elapsed / rollTime) * Mathf.PI) * 1f;
            dice.localScale = Vector3.one * scale;

            // Increase elapsed time
            elapsed += Time.deltaTime;

            // Wait for next frame
            yield return null;
        }

        // Reset dice scale after animation completes
        dice.localScale = Vector3.one;

        // Ensure dice returns to its original position
        dice.position = startPos;

        // Reset rotation before applying the final face rotation
        dice.rotation = Quaternion.identity;

        // Variable to store the final rotation corresponding to the dice value
        Quaternion targetRotation = Quaternion.identity;

        // Set the dice orientation so the correct face appears on top
        switch (finalValue)
        {
            case 1: targetRotation = Quaternion.Euler(-90, 0, 0); break;
            case 2: targetRotation = Quaternion.Euler(0, 0, 0); break;
            case 3: targetRotation = Quaternion.Euler(0, 0, -90); break;
            case 4: targetRotation = Quaternion.Euler(0, 0, 90); break;
            case 5: targetRotation = Quaternion.Euler(180, 0, 0); break;
            case 6: targetRotation = Quaternion.Euler(90, 0, 0); break;
        }

        // Apply the final rotation to display the correct dice face
        dice.rotation = targetRotation;

        // Print result to console for debugging
        Debug.Log("final value is " + finalValue);

        // Notify GameCalculator so score can be calculated
        calculator.OnDiceRollFinished(finalValue);
    }
}
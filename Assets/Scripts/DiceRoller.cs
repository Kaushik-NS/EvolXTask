using UnityEngine;
using System.Collections;

public class DiceRoller : MonoBehaviour
{
    public Transform dice;
    public GameCalculator calculator;

    public int finalValue;

    public void RollDice()
    {
        StartCoroutine(RollRoutine());
    }

    //IEnumerator RollRoutine()
    //{
    //    float rollTime = 1.5f;
    //    float elapsed = 0f;

    //    Vector3 startPos = dice.position;

    //    // Choose result first
    //    finalValue = Random.Range(1, 7);

    //    // Roll animation
    //    while (elapsed < rollTime)
    //    {
    //        dice.Rotate(
    //            Random.Range(500f, 800f) * Time.deltaTime,
    //            Random.Range(500f, 800f) * Time.deltaTime,
    //            Random.Range(500f, 800f) * Time.deltaTime
    //        );

    //        dice.position = startPos + Random.insideUnitSphere * 0.05f;

    //        elapsed += Time.deltaTime;
    //        yield return null;
    //    }

    //    dice.position = startPos;

    //    dice.rotation = Quaternion.identity;

    //    Quaternion targetRotation = Quaternion.identity;

    //    switch (finalValue)
    //    {
    //        case 1: targetRotation = Quaternion.Euler(-90, 0, 0); break;
    //        case 2: targetRotation = Quaternion.Euler(0, 0, 0); break;
    //        case 3: targetRotation = Quaternion.Euler(0, 0, -90); break;
    //        case 4: targetRotation = Quaternion.Euler(0, 0, 90); break;
    //        case 5: targetRotation = Quaternion.Euler(180, 0, 0); break;
    //        case 6: targetRotation = Quaternion.Euler(90, 0, 0); break;
    //    }

    //    dice.rotation = targetRotation;

    //    Debug.Log("final value is " + finalValue);

    //    calculator.OnDiceRollFinished(finalValue);
    //}


    IEnumerator RollRoutine()
    {
        float rollTime = 1.5f;
        float elapsed = 0f;

        Vector3 startPos = dice.position;

        // choose result first
        finalValue = Random.Range(1, 7);

        while (elapsed < rollTime)
        {
            // spin dice
            dice.Rotate(
                Random.Range(500f, 800f) * Time.deltaTime,
                Random.Range(500f, 800f) * Time.deltaTime,
                Random.Range(500f, 800f) * Time.deltaTime
            );

            // scale animation (grow then shrink)
            float scale = 1f + Mathf.Sin((elapsed / rollTime) * Mathf.PI) * 1f;
            dice.localScale = Vector3.one * scale;

            elapsed += Time.deltaTime;
            yield return null;
        }
        dice.localScale = Vector3.one;
        dice.position = startPos;

        dice.position = startPos;

        // reset before snapping
        dice.rotation = Quaternion.identity;

        Quaternion targetRotation = Quaternion.identity;

        switch (finalValue)
        {
            case 1: targetRotation = Quaternion.Euler(-90, 0, 0); break;
            case 2: targetRotation = Quaternion.Euler(0, 0, 0); break;
            case 3: targetRotation = Quaternion.Euler(0, 0, -90); break;
            case 4: targetRotation = Quaternion.Euler(0, 0, 90); break;
            case 5: targetRotation = Quaternion.Euler(180, 0, 0); break;
            case 6: targetRotation = Quaternion.Euler(90, 0, 0); break;
        }

        dice.rotation = targetRotation;

        Debug.Log("final value is " + finalValue);

        calculator.OnDiceRollFinished(finalValue);
    }

    int GetFaceTowardsCamera()
    {
        float bestDot = -1f;
        int bestFace = 1;

        Vector3 camDir = (Camera.main.transform.position - dice.position).normalized;

        foreach (Transform face in dice.GetComponentsInChildren<Transform>())
        {
            if (!face.name.StartsWith("Face"))
                continue;

            float dot = Vector3.Dot(face.up, camDir);

            if (dot > bestDot)
            {
                bestDot = dot;
                bestFace = int.Parse(face.name.Replace("Face", ""));
            }
        }

        return bestFace;
    }

    int GetFaceValue()
    {
        float upDot = Vector3.Dot(dice.up, Vector3.up);
        float forwardDot = Vector3.Dot(dice.forward, Vector3.up);
        float rightDot = Vector3.Dot(dice.right, Vector3.up);

        float max = Mathf.Max(Mathf.Abs(upDot), Mathf.Abs(forwardDot), Mathf.Abs(rightDot));

        if (max == Mathf.Abs(upDot))
            return upDot > 0 ? 2 : 5;

        if (max == Mathf.Abs(forwardDot))
            return forwardDot > 0 ? 1 : 6;

        return rightDot > 0 ? 3 : 4;
    }
}

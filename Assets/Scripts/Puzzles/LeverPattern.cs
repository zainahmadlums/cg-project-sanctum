using UnityEngine;
using System.Collections.Generic;

public class LeverPattern : Mechanism
{
    [Header("Puzzle Config")]
    [Tooltip("The mechanism to unlock when the sequence is correct.")]
    public Mechanism finalMechanism;

    [Tooltip("Drag Levers here in the CORRECT ORDER.")]
    public List<GameObject> correctSequence;

    [Header("Debug Status")]
    [SerializeField] private int currentIndex = 0;

    // Override the base AddTrigger to intercept the logic
    public override void AddTrigger(GameObject source = null)
    {
        if (source == null) return;

        // 1. Check if the pulled lever matches the expected lever at the current step
        if (currentIndex < correctSequence.Count && source == correctSequence[currentIndex])
        {
            // CORRECT!
            currentIndex++;
            
            // Check if done
            if (currentIndex >= correctSequence.Count)
            {
                // PUZZLE SOLVED
                // We activate ourselves (which invokes events)
                // And we activate the final door
                base.AddTrigger(source); // Updates internal state to active
                if (finalMechanism) finalMechanism.AddTrigger();
            }
        }
        else
        {
            // WRONG LEVER!
            // Or they pulled the correct one twice (turned it off)?
            // Reset the entire puzzle.
            ResetPuzzle();
        }
    }

    public override void RemoveTrigger(GameObject source = null)
    {
        // If a player manually turns OFF a lever, that breaks the sequence.
        // Reset everything.
        ResetPuzzle();
    }

    private void ResetPuzzle()
    {
        currentIndex = 0;

        // Physically reset all levers in the list so they pop back up
        foreach (GameObject obj in correctSequence)
        {
            var lever = obj.GetComponent<LeverController>();
            if (lever) lever.ForceReset();
        }

        // If we were active (puzzle solved), we need to close the door
        if (isActive)
        {
            if (finalMechanism) finalMechanism.RemoveTrigger();
            base.RemoveTrigger(); // Updates internal state to inactive
        }
    }

    protected override void HandleStateChange(bool active)
    {
        // Visual feedback for the puzzle controller itself?
        // Maybe turn a light green?
        // For now, logic is handled in AddTrigger/ResetPuzzle
    }
}
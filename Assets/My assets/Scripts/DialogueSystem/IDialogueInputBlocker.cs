namespace Hubble.DialogueSystem
{
    /// <summary>
    /// Optional hook to freeze player movement/actions while dialogue is active.
    /// Implement this on your player/controller (or a small adapter) and assign it to DialogueManager.
    /// </summary>
    public interface IDialogueInputBlocker
    {
        void SetDialogueInputBlocked(bool blocked);
    }
}

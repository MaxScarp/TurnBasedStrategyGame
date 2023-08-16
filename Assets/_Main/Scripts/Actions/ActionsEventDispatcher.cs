using System;

public static class ActionsEventDispatcher
{
    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;

    public static void OnAnyActionStartedInvoke(BaseAction baseAction)
    {
        OnAnyActionStarted?.Invoke(baseAction, EventArgs.Empty);
    }

    public static void OnAnyActionCompletedInvoke(BaseAction baseAction)
    {
        OnAnyActionCompleted?.Invoke(baseAction, EventArgs.Empty);
    }
}

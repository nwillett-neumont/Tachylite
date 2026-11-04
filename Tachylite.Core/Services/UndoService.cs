using System;

namespace Tachylite.Core.Services;

public class UndoService : IUndoService
{
    public const int TIMER_DELAY = 600;
    public const int CHANGE_HISTORY_MAX = 80;
    private readonly Stack<string> changeHistory = new();

    private string currentStateReference;

    private string? mostRecentUndoneState;

    private bool canUpdateChangeHistory= false;

    private bool contentStateHasChanged = false;

    private readonly PeriodicTimer _saveStateTimer;
    private readonly CancellationTokenSource _cancellationToken;
    private bool _timerShouldStop = false;
    private readonly Lock _locker = new();

    public UndoService()
    {
        _saveStateTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(TIMER_DELAY));
        _cancellationToken = new();

        currentStateReference = "";

        _ = Task.Run(StartTimer);
    }

    public void SetCurrentStateReference(string currentStateReference)
    {
        this.currentStateReference = currentStateReference;
    }

    public void SaveState(string newState)
    {
        if (changeHistory.Count >= CHANGE_HISTORY_MAX) RemoveBottomState();
        contentStateHasChanged = HasContentStateChanged(newState);
        if (canUpdateChangeHistory && contentStateHasChanged && !newState.Equals(mostRecentUndoneState))
        {
            changeHistory.Push(newState);
            canUpdateChangeHistory = false;
        }
    }

    public void OnTimerReset()
    {
        canUpdateChangeHistory = true;
        SaveState(currentStateReference);
        if (_timerShouldStop)
        {
            _cancellationToken.Dispose();
            _saveStateTimer.Dispose();
        }
    }

    public string Undo()
    {
        lock (_locker)
        {
            if (changeHistory.Count == 0) throw new NullReferenceException("There are no elements in the change history.");
            if (mostRecentUndoneState == changeHistory.Peek()) _ = changeHistory.Pop();
            mostRecentUndoneState = changeHistory.Peek();
            return changeHistory.Pop();
        }
    }

    public void RemoveBottomState()
    {
        lock (_locker)
        {
            if (changeHistory.Count < 2) return;
            string[] changeHistoryDuplicate = [..changeHistory];
            changeHistory.Clear();
            for (int i = changeHistoryDuplicate.Length - 2; i >= 0; i--)
            {
                changeHistory.Push(changeHistoryDuplicate[i]);
            }
        }
    }

    public bool HasContentStateChanged(string currentState)
    {
        lock (_locker)
        {
            if (changeHistory.Count > 0 && currentState.Equals(changeHistory.Peek())) return false;
            return true;
        }
    }

    public async Task StartTimer()
    {
        try
        {
            while (await _saveStateTimer.WaitForNextTickAsync(_cancellationToken.Token))
            {
                OnTimerReset();
            }
        } catch(OperationCanceledException)
        {
            _saveStateTimer.Dispose();
        } catch(Exception exception)
        {
            Console.WriteLine(exception.Message);
        }
    }

    public void StopTimer()
    {
        _timerShouldStop = true;
    }
}

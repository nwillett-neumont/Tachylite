using System;

namespace Tachylite.Core.Services;

public class UndoService : IUndoService
{
    public const int TIMER_DELAY = 200;
    public const int CHANGE_HISTORY_MAX = 10;
    public Stack<string> ChangeHistory { get; } = new Stack<string>();

    public string CurrentStateReference { get; set; } = "";

    public bool CanUpdateChangeHistory { get; set; } = false;

    public bool ContentStateHasChanged { get; set; } = false;

    private readonly PeriodicTimer _saveStateTimer;
    private readonly CancellationTokenSource _cancellationToken;
    private bool _timerShouldStop = false;

    private readonly Lock _locker = new();

    public UndoService()
    {
        _saveStateTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(TIMER_DELAY));
        _cancellationToken = new();

        _ = Task.Run(StartTimer);
    }

    public void SaveState(string newState)
    {
        if (ChangeHistory.Count >= CHANGE_HISTORY_MAX) RemoveBottomState();
        ContentStateHasChanged = HasContentStateChanged(newState);
        if (CanUpdateChangeHistory && ContentStateHasChanged)
        {
            ChangeHistory.Push(newState);
            CanUpdateChangeHistory = false;
        }
    }

    public void OnTimerReset()
    {
        CanUpdateChangeHistory = true;
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
            if (ChangeHistory.Count == 0) throw new NullReferenceException("There are no elements in the change history.");
            return ChangeHistory.Pop();
        }
    }

    public void RemoveBottomState()
    {
        lock (_locker)
        {
            if (ChangeHistory.Count < 2) return;
            string[] changeHistoryDuplicate = [..ChangeHistory];
            ChangeHistory.Clear();
            for (int i = changeHistoryDuplicate.Length - 2; i >= 0; i--)
            {
                ChangeHistory.Push(changeHistoryDuplicate[i]);
            }
        }
    }

    public bool HasContentStateChanged(string currentState)
    {
        lock (_locker)
        {
            if (currentState.Equals(ChangeHistory.Peek())) return false;
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

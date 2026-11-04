using System;

namespace Tachylite.Core.Services;

public interface IUndoService
{
    public void SetCurrentStateReference(string currentStateReference);
    public void SaveState(string newState);

    public void OnTimerReset();

    public string Undo();

    public void RemoveBottomState();

    public bool HasContentStateChanged(string currentState);

    public Task StartTimer();

    public void StopTimer();
}

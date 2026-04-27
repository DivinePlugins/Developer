namespace Debugger.Tools;

using System;

internal interface IDebuggerTool : IDisposable
{
    void IDisposable.Dispose()
    {
    }

    void Activate()
    {
    }

    void Deactivate()
    {
    }
}

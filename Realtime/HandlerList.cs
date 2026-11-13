using System;

namespace Supabase.Realtime
{
/// <summary>
/// Event handlers that are registered, removed and invoked from different threads.
///
/// Application threads register handlers, the socket's receive thread and timeout timers remove
/// them, and the receive thread invokes them. Each change swaps in a new array under a lock, so
/// <see cref="Snapshot"/> is safe to read and enumerate without one.
/// </summary>
/// <typeparam name="THandler">The delegate type of the handlers.</typeparam>
internal sealed class HandlerList<THandler> where THandler : Delegate
{
    private readonly object _gate = new object();
    private volatile THandler[] _handlers = Array.Empty<THandler>();

    /// <summary>
    /// The handlers registered at the time of the call. The array is never modified afterwards.
    /// </summary>
    public THandler[] Snapshot => _handlers;

    /// <summary>
    /// Registers a handler, unless an equal one is already registered.
    /// </summary>
    /// <param name="handler"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void Add(THandler handler)
    {
        if (handler == null) throw new ArgumentNullException(nameof(handler));

        lock (_gate)
        {
            var current = _handlers;
            if (Array.IndexOf(current, handler) >= 0) return;

            var next = new THandler[current.Length + 1];
            Array.Copy(current, next, current.Length);
            next[current.Length] = handler;
            _handlers = next;
        }
    }

    /// <summary>
    /// Removes a handler. Does nothing when it is not registered.
    /// </summary>
    /// <param name="handler"></param>
    public void Remove(THandler handler)
    {
        lock (_gate)
        {
            var current = _handlers;
            var index = Array.IndexOf(current, handler);
            if (index < 0) return;

            var next = new THandler[current.Length - 1];
            Array.Copy(current, 0, next, 0, index);
            Array.Copy(current, index + 1, next, index, next.Length - index);
            _handlers = next;
        }
    }

    /// <summary>
    /// Removes every handler.
    /// </summary>
    public void Clear()
    {
        lock (_gate)
            _handlers = Array.Empty<THandler>();
    }
}
}

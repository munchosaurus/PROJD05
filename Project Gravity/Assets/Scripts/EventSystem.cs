using System;
using System.Collections.Generic;
using UnityEngine;

public class EventSystem : MonoBehaviour
{
    class GameListener
    {
        public EventListener listener;
        public System.Guid guid;

        public GameListener(EventListener listener)
        {
            this.listener = listener;
            this.guid = Guid.NewGuid();
        }
    }

    private static EventSystem _current;
    private Dictionary<System.Type, List<GameListener>> _eventListeners;

    delegate void EventListener(Event e);

    void OnEnable()
    {
        _current = this;
        GravityController.Init();
    }

    public static EventSystem Current
    {
        get
        {
            // If Current gets called upon prior to OnEnable.
            // Should take a look at the execution order instead, expensive
            // to check if _current is null for each instance that registers a listener.
            if (_current == null)
            {
                _current = FindObjectOfType<EventSystem>();
            }

            return _current;
        }
    }

    // Adds a listener to the dictionary, the appended Guid will be gererated when the gamelistener is
    // instanciated, will also be stored in the class listening. The reasoning behind not using instance IDs could be that
    // classes want to make use of several listeners in the same class.
    public void RegisterListener<T>(System.Action<T> listener, ref Guid guid) where T : Event
    {
        System.Type eventType = typeof(T);
        if (_eventListeners == null)
        {
            _eventListeners = new Dictionary<System.Type, List<GameListener>>();
        }

        if (_eventListeners.ContainsKey(eventType) == false || _eventListeners[eventType] == null)
        {
            _eventListeners[eventType] = new List<GameListener>();
        }

        EventListener wrapper = (eventInfo) => { listener((T) eventInfo); };
        GameListener gameListener = new GameListener(wrapper);
        _eventListeners[eventType].Add(gameListener);
        guid = gameListener.guid;
    }

    // Should be called upon as soon as soon as we switch scenes.
    public void ClearListeners()
    {
        if (_eventListeners != null)
        {
            _eventListeners = new Dictionary<Type, List<GameListener>>();
        }
    }

    public void FireEvent(Event e)
    {
        System.Type trueEventInfoClass = e.GetType();
        if (_eventListeners == null || _eventListeners[trueEventInfoClass] == null)
        {
            // No one is listening, we are done.
            return;
        }

        foreach (GameListener el in _eventListeners[trueEventInfoClass])
        {
            el.listener(e);
        }
    }
}
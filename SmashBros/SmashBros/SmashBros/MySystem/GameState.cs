﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmashBros.Controllers;
using System.Diagnostics;

namespace SmashBros.MySystem
{
    public enum GameState
    {
        StartScreen, CharacterMenu, MapsMenu, GamePlay, GamePause, GameOver

    }

    public class GameStateManager
    {
        private GameStateTracker tracker;
        private List<Controller> controllers;

        private GameState currentState;
        private GameState prevState;
        public GameState CurrentState
        {
            get { return currentState; }
            set
            {
                if (value != currentState)
                {
                    prevState = currentState;
                    currentState = value; 
                    tracker.UpdateSubscribers(this);
                }
            }
        }

        public GameState PreviousState { get { return prevState; } }

        public GameStateManager()
        {
            tracker = new GameStateTracker();
            controllers = new List<Controller>();
        }

        public void Add(Controller controller){
            tracker.Subscribe(controller);
            controller.OnNext(this);
        }

        internal void Remove(Controller controller)
        {
            tracker.UnSubscribe(controller);
        }
    }

    public class GameStateTracker : IObservable<GameStateManager>
    {

        private List<IObserver<GameStateManager>> observers;
        public GameStateTracker()
        {
            observers = new List<IObserver<GameStateManager>>();
        }
 
        public IDisposable Subscribe(IObserver<GameStateManager> observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }

            return null;
        }

        public IDisposable UnSubscribe(IObserver<GameStateManager> observer)
        {
            if(observers.Contains(observer))
                observers.Remove(observer);
            return null;
        }
 
        public void UpdateSubscribers(GameStateManager c)
        {
            observers.ForEach(x => x.OnNext(c));
        }
    }
}

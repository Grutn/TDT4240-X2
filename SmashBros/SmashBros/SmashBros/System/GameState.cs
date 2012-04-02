using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmashBros.Controllers;

namespace SmashBros.System
{
    public enum GameState
    {
        StartScreen, SelectionMenu, OptionsMenu, GamePlay 
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
            tracker.UpdateSubscribers(this);
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
            observers.Add(observer);

            return null;
        }

        public IDisposable UnSubscribe(IObserver<GameStateManager> observer)
        {
            observers.Remove(observer);
            return null;
        }
 
        public void UpdateSubscribers(GameStateManager c)
        {
            observers.ForEach(x => x.OnNext(c));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using GameOfLife.GameLogic.GameStates;
using GameOfLife.GameLogic.GameStates.Factories;
using Microsoft.Xna.Framework;

namespace GameOfLife.BoilerPlate.FSM {
    public class FSM<T, U> where T : class, IState<T, U> {
        private readonly Stack<T> _states;
        private readonly Stack<T> _lazyStates;

        public FSM() {
            _states = new Stack<T>();
            _lazyStates = new Stack<T>();
        }

        public void PerformLogic(GameTime gameTime, U arg) {
            while (_lazyStates.Any()) Push(_lazyStates.Pop());

            T currentState = Pop();
            if (currentState == null) return;

            T[] returnedStates = currentState.PerformLogic(gameTime, arg);
            if (returnedStates != null) {
                Array.ForEach(returnedStates, Push);
            }
        }

        public void Push(T state) {
            if (state == null) throw new NullReferenceException("Attempted to add null state to " + this);
            _states.Push(state);
        }

        public void LazyPush(T state) {
            if (state == null) throw new NullReferenceException("Attempted to add null state to " + this);
            _lazyStates.Push(state);
        }

        public T Pop() {
            return _states.Count > 0 ? _states.Pop() : default(T);
        }

        public void Clear() {
            _states.Clear();
        }

        public bool Remove(T state) {
            var extraStack = new Stack<T>();
            T tempState = null;
            while (_states.Any() && (tempState = _states.Pop()) != state) {
                extraStack.Push(tempState);
            }
            var found = tempState == state;
            while (extraStack.Any()) {
                _states.Push(extraStack.Pop());
            }
            return found;
        }

        public T Peek() {
            return _states.Count > 0 ? _states.Peek() : default(T);
        }

        public int Size() {
            return _states.Count;
        }

        public override string ToString() {
            return String.Join("\n", _states);
        }
    }

    public interface IState<T, U> where T : IState<T, U> {
        T[] PerformLogic(GameTime gameTime, U gameInfo);
    }
}

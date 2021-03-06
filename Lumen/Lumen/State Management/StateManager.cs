﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen.State_Management
{
    //Singleton design of a statemanager
    public class StateManager
    {
        private readonly List<State> _states = new List<State>();
        public GameDriver Game = null;

        #region Singleton Data

        private static StateManager _instance;

        private StateManager()
        {
        }

        private StateManager(StateManager s)
        {
        }

        public static StateManager Instance
        {
            get { return _instance ?? (_instance = new StateManager()); }
        }

        #endregion

        public void PushState(State state)
        {
            state.LoadContent(Game.Content, Game.GraphicsDevice);
            state.Initialize(Game);
            _states.Add(state);
        }

        public State PopState()
        {
            State state = null;

            if (_states.Count > 0) {
                state = _states[_states.Count - 1];
                _states.RemoveAt(_states.Count - 1);
                state.Shutdown();
            }

            return state;
        }

        public void PopAll()
        {
            while (PopState() != null) {
                ;
            }
        }

        public void Update(GameTime gameTime)
        {
            if (_states.Count > 0) {
                _states[_states.Count - 1].Update(gameTime);
            }
        }

        public void Draw(SpriteBatch g, GraphicsDevice gd)
        {
            if (_states.Count > 0) {
                _states[_states.Count - 1].Draw(g, gd);
            }
        }
    }
}
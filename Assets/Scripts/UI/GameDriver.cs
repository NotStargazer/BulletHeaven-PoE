using System.Collections.Generic;
using Input;
using UI.DriverStates;

namespace UI
{
    public enum DriverState
    {
        Game
    }
    
    public interface IDriverState
    {
        public void EnterState(IUIRoot ui, DriverState? from);
        public void ExitState(IUIRoot ui, DriverState to);
    }
    
    public class GameDriver
    {
        private Dictionary<DriverState, IDriverState> _driverStates;
        private DriverState _state;
        private IUIRoot _uiRoot;

        private InputController _inputController;
        
        public GameDriver(IUIRoot uiRoot, DriverState initialState)
        {
            _state = initialState;
            _uiRoot = uiRoot;
            
            _driverStates = new Dictionary<DriverState, IDriverState>()
            {
                { DriverState.Game, new State_Game() }
            };
            
            _driverStates[initialState].EnterState(_uiRoot, null);
        }
        
        public void ChangeState(DriverState to)
        {
            var from = _state;
            _state = to;
            
            _driverStates[from].ExitState(_uiRoot, to);
            _driverStates[to].EnterState(_uiRoot, from);
        }
    }
}

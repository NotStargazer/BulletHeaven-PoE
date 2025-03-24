using System;

namespace UI.DriverStates
{
    public class State_Game : IDriverState
    {
        public void EnterState(IUIRoot ui, DriverState? from)
        {
            ui.StartLevel();
        }

        public void ExitState(IUIRoot ui, DriverState to)
        {
            
        }
    }
}

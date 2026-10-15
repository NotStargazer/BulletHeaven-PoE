namespace UI.DriverStates
{
    public class State_MainMenu : IDriverState
    {
        public void EnterState(IUIRoot ui, DriverState? from)
        {
            ui.SetMainMenuVisibility(true);
        }

        public void ExitState(IUIRoot ui, DriverState to)
        {
            ui.SetMainMenuVisibility(false);
        }
    }
}
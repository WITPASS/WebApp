using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public class AppStateService
    {
        public string SelectedColour { get; private set; }

        public event Action ColorChanged;

        public void SetColour(string colour)
        {
            SelectedColour = colour;
            ColorChanged?.Invoke();
        }
    }
}

/*


<!-- Source Component (State Changer) -->

@inject AppState AppState

<button onclick="@SelectColour">Select Red</button>

@functions {

    void SelectColour()
    {
        AppState.SetColour("Red");
    }

}



<!-- Destination Component (State User) -->

@inject AppState AppState

@AppState.SelectedColour

@functions {

    protected override void OnInit()
    {
        AppState.ColorChanged += StateHasChanged;
    }

}


*/

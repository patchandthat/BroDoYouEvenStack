using Caliburn.Micro;

namespace Bootstrapper.Interface.UI
{
    /// <summary>
    /// This is just a viewmodel to represent an indeterminate busy/please wait screen
    /// Has no real functionality
    /// </summary>
    class BusyViewModel : Screen
    {
        private readonly IEventAggregator _agg;

        public BusyViewModel(IEventAggregator agg)
        {
            _agg = agg;
            _agg.Subscribe(this);
        }
    }
}

﻿using Caliburn.Micro;

namespace Bootstrapper.Interface.UI
{
    class UninstallSuccessViewModel : Screen
    {
        private readonly IEventAggregator _agg;

        public UninstallSuccessViewModel(IEventAggregator agg)
        {
            _agg = agg;
            _agg.Subscribe(this);
        }
    }
}

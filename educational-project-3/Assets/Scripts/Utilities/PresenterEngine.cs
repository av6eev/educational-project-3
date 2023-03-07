using System.Collections.Generic;

namespace Utilities
{
    public class PresenterEngine : IPresenter
    {
        private readonly List<IPresenter> _presenters = new();
        
        public void Deactivate()
        {
            foreach (var presenter in _presenters)
            {
                presenter.Deactivate();
            }
        }

        public void Activate()
        {
            foreach (var presenter in _presenters)
            {
                presenter.Activate();
            }
        }
        
        public void Add(IPresenter presenter)
        {
            _presenters.Add(presenter);
        }

        public void Clear()
        {
            _presenters.Clear();
        }
    }
}
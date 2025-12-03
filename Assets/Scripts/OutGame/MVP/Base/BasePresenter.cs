using System;
using Cysharp.Threading.Tasks;
using R3;

namespace OutGame.MVP
{
    /// <summary>
    /// Presenter の基底クラス
    /// </summary>
    public abstract class BasePresenter<TView, TModel> : IPresenter
        where TView : IView
        where TModel : IModel
    {
        protected TView View { get; private set; }
        protected TModel Model { get; private set; }
        protected CompositeDisposable Disposables { get; } = new();

        public BasePresenter(TView view, TModel model)
        {
            View = view;
            Model = model;
        }

        public virtual async UniTask Initialize()
        {
            await UniTask.CompletedTask;
        }

        public virtual async UniTask Show()
        {
            await View.Show();
        }

        public virtual async UniTask Hide()
        {
            await View.Hide();
        }

        public virtual void Dispose()
        {
            Disposables?.Dispose();
            View?.Dispose();
            Model?.Dispose();
        }
    }
}


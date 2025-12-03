using System;
using R3;

namespace OutGame.MVP
{
    /// <summary>
    /// Model の基底クラス
    /// </summary>
    public abstract class BaseModel : IModel
    {
        protected CompositeDisposable Disposables { get; } = new();

        public virtual void Dispose()
        {
            Disposables?.Dispose();
        }
    }
}


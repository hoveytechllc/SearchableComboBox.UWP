using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SearchableComboBox.Samples.Extensions;

namespace SearchableComboBox.Samples
{
    public interface IMainThreadInvoker
    {
        Task Invoke(Action action);
    }

    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected BaseViewModel()
        {

        }

        protected static IMainThreadInvoker MainThreadInvoker;

        public static void Initialize(IMainThreadInvoker mainThreadInvoker)
        {
            MainThreadInvoker = mainThreadInvoker;
        }

        public Task RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var name = this.GetPropertyNameFromExpression(propertyExpression);
            return RaisePropertyChanged(name);
        }

        public virtual Task RaisePropertyChanged([CallerMemberName] string whichProperty = "")
        {
            var changedArgs = new PropertyChangedEventArgs(whichProperty);
            return RaisePropertyChanged(changedArgs);
        }

        public virtual async Task RaisePropertyChanged(PropertyChangedEventArgs changedArgs)
        {
            if (PropertyChanged == null)
                return;

            await MainThreadInvoker.Invoke(() => { PropertyChanged?.Invoke(this, changedArgs); });
        }
    }
}

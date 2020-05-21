using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Biaui.Controls.Mock.Foundation.Mvvm
{
    public class NotificationObject : INotifyPropertyChanged
    {
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;

            RaisePropertyChanged(propertyName);

            return true;
        }

        protected bool SetFlagProperty(ref uint storage, uint flag, bool value, [CallerMemberName] string propertyName = "")
        {
            if (value)
            {
                if ((storage & flag) != 0)
                    return false;

                storage |= flag;
            }
            else
            {
                if ((storage & flag) == 0)
                    return false;

                storage &= ~flag;
            }

            RaisePropertyChanged(propertyName);

            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            // ReSharper disable once InconsistentlySynchronizedField
            var pc = (PropertyChangedEventArgs)_propChanged[propertyName];

            if (pc is null)
            {
                // double-checked;
                lock (_propChanged)
                {
                    pc = (PropertyChangedEventArgs)_propChanged[propertyName];

                    if (pc is null)
                    {
                        pc = new PropertyChangedEventArgs(propertyName);
                        _propChanged[propertyName] = pc;
                    }
                }
            }

            PropertyChanged?.Invoke(this, pc);
        }

        // use Hashtable to get free lockless reading
        private static readonly Hashtable _propChanged = new Hashtable();
    }
}
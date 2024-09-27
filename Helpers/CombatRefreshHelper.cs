// Copyright (c) 2024, Todd Taylor (https://github.com/zxeltor)
// All rights reserved.
// 
// This source code is licensed under the Apache-2.0-style license found in the
// LICENSE file in the root directory of this source tree.

using System.ComponentModel;
using System.Reflection;
using zxeltor.StoCombat.Lib.Classes;

namespace zxeltor.StoCombat.Lib.Helpers;

public static class CombatRefreshHelper
{
    #region Public Members

    public static void RefreshObject(object thisObject)
    {
        var fieldsList = thisObject.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic).ToList();
        foreach (var field in fieldsList)
        {
            var resetAttribute = field.GetCustomAttribute<ResetFieldOnRefreshAttribute>();
            if (resetAttribute != null) field.SetValue(thisObject, resetAttribute.ResetValue);
        }

        if (thisObject is INotifyPropertyChanged)
        {
            var objectFieldValue = thisObject.GetType().GetField("PropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(thisObject);

            if(objectFieldValue == null) return;

            var eventDelegate = objectFieldValue as MulticastDelegate;

            if (eventDelegate == null) return;
            
            var propertyList = thisObject.GetType().GetProperties().ToList();

            foreach (var property in propertyList)
            {
                var notifyProperty = property.GetCustomAttribute<NotifyPropertyChangedOnRefreshAttribute>();
                if (notifyProperty == null) continue;

                foreach (var handler in eventDelegate.GetInvocationList())
                    handler.Method.Invoke(handler.Target,
                        new[] { thisObject, new PropertyChangedEventArgs(notifyProperty.PropertyName) });
            }
        }
    }

    #endregion
}
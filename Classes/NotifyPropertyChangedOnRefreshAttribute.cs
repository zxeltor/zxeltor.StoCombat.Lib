// Copyright (c) 2024, Todd Taylor (https://github.com/zxeltor)
// All rights reserved.
// 
// This source code is licensed under the Apache-2.0-style license found in the
// LICENSE file in the root directory of this source tree.

using System.Runtime.CompilerServices;

namespace zxeltor.StoCombat.Lib.Classes;

/// <summary>
///     An attribute used by the combat refresh mechanism. This specifies the property should send change notification during a refresh.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class NotifyPropertyChangedOnRefreshAttribute : Attribute
{
    #region Constructors
    /// <summary>
    ///     The default constructor.
    /// </summary>
    /// <param name="propertyName"></param>
    public NotifyPropertyChangedOnRefreshAttribute([CallerMemberName] string? propertyName = null)
    {
        this.PropertyName = propertyName;
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///     The name of the property
    /// </summary>
    public string? PropertyName { get; }

    #endregion
}
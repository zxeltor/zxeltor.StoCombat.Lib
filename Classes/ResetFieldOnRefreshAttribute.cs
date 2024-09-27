// Copyright (c) 2024, Todd Taylor (https://github.com/zxeltor)
// All rights reserved.
// 
// This source code is licensed under the Apache-2.0-style license found in the
// LICENSE file in the root directory of this source tree.

namespace zxeltor.StoCombat.Lib.Classes;

/// <summary>
///     An attribute used by the combat refresh mechanism. This specifies the field should be set to null, or some other default value during a refresh.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class ResetFieldOnRefreshAttribute : Attribute
{
    #region Constructors
    /// <summary>
    ///     The default constructor
    /// </summary>
    /// <param name="resetValue">What to use as a reset value. Defaults to null.</param>
    public ResetFieldOnRefreshAttribute(object? resetValue = null)
    {
        this.ResetValue = resetValue;
    }

    #endregion

    #region Public Properties
    /// <summary>
    ///     The value to use to reset the field.
    /// </summary>
    public object? ResetValue { get; }

    #endregion
}
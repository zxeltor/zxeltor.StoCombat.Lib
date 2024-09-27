// Copyright (c) 2024, Todd Taylor (https://github.com/zxeltor)
// All rights reserved.
// 
// This source code is licensed under the Apache-2.0-style license found in the
// LICENSE file in the root directory of this source tree.

using System.Runtime.CompilerServices;

namespace zxeltor.StoCombat.Lib.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class PropertySettingAttribute : Attribute
{
    #region Constructors

    public PropertySettingAttribute(string description, string? note = null, [CallerMemberName] string? name = null,
        string? label = null)
    {
        this.Name = name;
        this.Description = description;
        this.Note = note;
        this.Label = label ?? name;
    }

    #endregion

    #region Public Properties

    public string Description { get; private set; }
    public string Name { get; private set; }
    public string? Label { get; private set; }
    public string? Note { get; private set; }

    #endregion
}
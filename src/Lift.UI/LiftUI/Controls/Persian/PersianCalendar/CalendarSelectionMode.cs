﻿//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

namespace Microsoft.Windows.Controls;

/// <summary>
/// Specifies values for the different selection modes of a PersianCalendar. 
/// </summary>
public enum CalendarSelectionMode
{
    /// <summary>
    /// One date can be selected at a time.
    /// </summary>
    SingleDate = 0,

    /// <summary>
    /// One range of dates can be selected at a time.
    /// </summary>
    SingleRange = 1,

    /// <summary>
    /// Multiple dates or ranges can be selected at a time.
    /// </summary>
    MultipleRange = 2,

    /// <summary>
    /// No dates can be selected.
    /// </summary>
    None = 3,
}

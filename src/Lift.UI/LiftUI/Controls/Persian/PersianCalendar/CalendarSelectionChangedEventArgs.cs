﻿//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Windows.Controls;

/// <summary>
/// Workaround for Dev10 Bug 527138 UIElement.RaiseEvent(e) throws InvalidCastException when 
/// e is of type SelectionChangedEventArgs 
/// e.RoutedEvent was registered with a handler not of type System.Windows.Controls.SelectionChangedEventHandler
/// </summary>
internal class CalendarSelectionChangedEventArgs : SelectionChangedEventArgs
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="eventId">Routed Event</param>
    /// <param name="removedItems">Items removed from selection</param>
    /// <param name="addedItems">Items added to selection</param>
    public CalendarSelectionChangedEventArgs(RoutedEvent eventId, IList removedItems, IList addedItems) :
        base(eventId, removedItems, addedItems)
    {
    }

    protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
    {
        EventHandler<SelectionChangedEventArgs> handler = genericHandler as EventHandler<SelectionChangedEventArgs>;
        if (handler != null)
        {
            handler(genericTarget, this);
        }
        else
        {
            base.InvokeEventHandler(genericHandler, genericTarget);
        }
    }
}

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace ShortDev.IO.Bond;

public struct DeserializerControls
{
    // Default settings
    public static readonly DeserializerControls Default;

    // Current active settings
    public static DeserializerControls Active { get; set; }

    static DeserializerControls()
    {
        Default.MaxPreallocatedContainerElements = 64 * 1024;
        Default.MaxPreallocatedBlobBytes = 64 * 1024 * 1024;
        Default.MaxDepth = 64;
        Active = Default;
    }

    public int MaxPreallocatedContainerElements
    {
        get;
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, 0);
            field = value;
        }
    }

    public int MaxPreallocatedBlobBytes
    {
        get;
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, 0);
            field = value;
        }
    }

    public int MaxDepth
    {
        get;
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, 0);
            field = value;
        }
    }
}

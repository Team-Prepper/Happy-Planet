/*****************************************************
Copyright © 2024 Michael Kremmel
https://www.michaelkremmel.de
All rights reserved
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MK.EdgeDetection
{
    public enum Precision
    {
        [InspectorName("High")]
        High = 0,
        [InspectorName("Medium")]
        Medium = 1,
        [InspectorName("Low")]
        Low = 2
    }
}
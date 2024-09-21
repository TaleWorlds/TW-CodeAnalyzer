using System;
using System.Collections.Generic;
using System.Text;

namespace TaleworldsCodeAnalysis
{
    public enum DiagnosticIDs
    {
        TW2100,     // Abstract Class Checker
        TW2101,     // Depth of Inheritance Checker
        TW2102,     // Sealed Override Checker
        TW2000,     // Class Name Checker
        TW2002,     // Field Name Checker
        TW2003,     // Interface Name Checker
        TW2004,     // Local Name Checker
        TW2005,     // Method Name Checker
        TW2006,     // Parameter Name Checker
        TW2007,     // Property Name Checker 
        TW2008,     // Template Name Checker
        TW2001,     // Class Accessibility Checker
        TW2200,     // Field Accessibility Checker
        TW2205,     // Immutable Structs
        TW2202,     // Mixed Access Modifier Checker
        TW2204,     // Var Keyword Checker
        TW2203,     // Mutable Field Return Checker
        TW2201,     // Named Parameter Checker
    }
}

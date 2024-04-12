//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType
{
    /// <summary>
    /// The <c>FontStyle</c> enumeration describes the style of a font
    /// </summary>
    [System.Flags]
    public enum FontStyle : int
    {
        Normal = 0,
        Bold = 1,
        Italic = 2,
        BoldItalic = Bold | Italic,
    }
}

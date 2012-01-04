//	Copyright © 2003-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>KeyCode</c> enumeration lists all currently used key codes
	/// produced by the keyboard.
	/// </summary>
	[System.Flags]
	public enum KeyCode
	{
		None			= 0,
		
		AlphaA			= 65,
		AlphaB			= 66,
		AlphaC			= 67,
		AlphaD			= 68,
		AlphaE			= 69,
		AlphaF			= 70,
		AlphaG			= 71,
		AlphaH			= 72,
		AlphaI			= 73,
		AlphaJ			= 74,
		AlphaK			= 75,
		AlphaL			= 76,
		AlphaM			= 77,
		AlphaN			= 78,
		AlphaO			= 79,
		AlphaP			= 80,
		AlphaQ			= 81,
		AlphaR			= 82,
		AlphaS			= 83,
		AlphaT			= 84,
		AlphaU			= 85,
		AlphaV			= 86,
		AlphaW			= 87,
		AlphaX			= 88,
		AlphaY			= 89,
		AlphaZ			= 90,
		
		Digit0			= 48,
		Digit1			= 49,
		Digit2			= 50,
		Digit3			= 51,
		Digit4			= 52,
		Digit5			= 53,
		Digit6			= 54,
		Digit7			= 55,
		Digit8			= 56,
		Digit9			= 57,
		
		FuncF1			= 112,
		FuncF2			= 113,
		FuncF3			= 114,
		FuncF4			= 115,
		FuncF5			= 116,
		FuncF6			= 117,
		FuncF7			= 118,
		FuncF8			= 119,
		FuncF9			= 120,
		FuncF10			= 121,
		FuncF11			= 122,
		FuncF12			= 123,
		FuncF13			= 124,
		FuncF14			= 125,
		FuncF15			= 126,
		FuncF16			= 127,
		FuncF17			= 128,
		FuncF18			= 129,
		FuncF19			= 130,
		FuncF20			= 131,
		FuncF21			= 132,
		FuncF22			= 133,
		FuncF23			= 134,
		FuncF24			= 135,
		
		AltKey			= 18,
		AltKeyLeft		= 164,
		AltKeyRight		= 165,
		
		ArrowDown		= 40,
		ArrowLeft		= 37,
		ArrowRight		= 39,
		ArrowUp			= 38,
		
		Back			= 8,
		Clear			= 12,
		
		ControlKey		= 17,
		ControlKeyLeft	= 162,
		ControlKeyRight	= 163,
		
		Delete			= 46,
		End				= 35,
		Escape			= 27,
		Home			= 36,
		Insert			= 45,
		PageDown		= 34,
		PageUp			= 33,
		Pause			= 19,
		Return			= 13,

		NumericMultiply		= 106,
		NumericAdd			= 107,
//?		NumericSeparator	= 108,
		NumericSubstract	= 109,
		NumericDecimal		= 110,
		NumericDivide		= 111,

		ShiftKey		= 16,
		ShiftKeyLeft	= 160,
		ShiftKeyRight	= 161,
		
		Space			= 32,
		Tab				= 9,
		
		Comma			= 188,
		Dash			= 189,
		Dot				= 190,

		OemParaSign		= 191,
		OemApostrophe	= 219,
		OemCircumflex	= 221,
		OemBackslash	= 226,
		
		ContextualMenu	= 93,
		
		CapsLock		= 20,
		NumLock			= 144,
		ScrollLock		= 145,
		
		KeyCodeMask		= 0x0000ffff,
		ModifierMask	= 0x00ff0000,
		
		ModifierShift	= (int) ModifierKeys.Shift,
		ModifierControl	= (int) ModifierKeys.Control,
		ModifierAlt		= (int) ModifierKeys.Alt,

		//	Special, virtual key codes defined internally (usually extended keys which map to other key codes) :
		
		ExtendedKeys		= 0x1000,
		NumericEnter		= ExtendedKeys + 0,
	}
}

//	Copyright � 2003-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	public enum TextFieldStyle
	{
		Normal,							// ligne �ditable normale
		Flat,							// pas de cadre, ni de relief, fond blanc
		Multi,							// ligne �ditable Multi
		Combo,							// ligne �ditable Combo
		UpDown,							// ligne �ditable UpDown
		Simple,							// cadre tout simple
		Static,							// comme Flat mais fond transparent, s�lectionnable, pas �ditable...
	}
}

//	Copyright © 2003-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	public enum TextFieldStyle
	{
		Normal,							// ligne éditable normale
		Flat,							// pas de cadre, ni de relief, fond blanc
		Multi,							// ligne éditable Multi
		Combo,							// ligne éditable Combo
		UpDown,							// ligne éditable UpDown
		Simple,							// cadre tout simple
		Static,							// comme Flat mais fond transparent, sélectionnable, pas éditable...
	}
}

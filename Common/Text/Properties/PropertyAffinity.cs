/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


namespace Epsitec.Common.Text.Properties
{
    /// <summary>
    /// L'énumération PropertyAffinity définit l'affinité d'une propriété, à
    /// savoir comment elle est rattachée au texte.
    /// </summary>
    public enum PropertyAffinity
    {
        Invalid = 0,

        Text = 1, //	propriété "normale"
        Symbol = 2, //	propriété rattachée à un symbole
    }

    //	Les propriétés dont l'affinité est PropertyAffinity.Symbol sont très
    //	particulières : elles définissent des informations vitales sans les-
    //	quelles le symbole n'aurait pas de signification.
    //
    //	Exemples :
    //
    //	- ImageProperty : définit quelle image peindre en lieu et place du
    //	  symbole Unicode.Code.ObjectReplacement.
    //
    //	- OpenTypeProperty : définit quel glyphe particulier utiliser pour
    //	  le symbole correspondant.
    //
    //	- TabProperty : définit à quel tabulateur le symbole HorizontalTab
    //	  se rattache.
}

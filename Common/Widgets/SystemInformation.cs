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

namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// La classe SystemInformation donne des informations sur le système.
    /// </summary>
    public class SystemInformation
    {
        // ********************************************************************
        // TODO bl-net8-cross maybedelete
        // Does this work on multiplatform , with the Microsoft.Win32 things ?
        // ********************************************************************
        public enum Animation
        {
            None,
            Roll,
            Fade
        }

        public static double InitialKeyboardDelay
        {
            get { return 0.5; }
        }

        public static double KeyboardRepeatPeriod
        {
            get { return 0.1; }
        }

        public static double CursorBlinkDelay
        {
            get { return 0.499; }
        }

        public static double MenuShowDelay
        {
            get { return 0.199; }
        }

        public static Animation MenuAnimation
        {
            get
            {
                if (SystemInformation.IsMenuAnimationEnabled)
                {
                    return SystemInformation.IsMenuFadingEnabled ? Animation.Fade : Animation.Roll;
                }

                return Animation.None;
            }
        }

        public static double MenuAnimationRollTime
        {
            get { return 0.250; }
        }

        public static double MenuAnimationFadeInTime
        {
            get { return 0.200; }
        }

        public static double MenuAnimationFadeOutTime
        {
            get { return 0.200; }
        }

        public static double ToolTipShowDelay
        {
            get { return 1.0; }
        }

        public static double ToolTipAutoCloseDelay
        {
            get { return 5.0; }
        }

        public static bool IsMenuAnimationEnabled
        {
            get { return (SystemInformation.UserPreferenceMask[0] & 0x02) != 0; }
        }

        public static bool IsComboAnimationEnabled
        {
            get { return (SystemInformation.UserPreferenceMask[0] & 0x04) != 0; }
        }

        public static bool IsSmoothScrollEnabled
        {
            get { return (SystemInformation.UserPreferenceMask[0] & 0x08) != 0; }
        }

        public static bool IsMetaUnderlineEnabled
        {
            get { return (SystemInformation.UserPreferenceMask[0] & 0x20) != 0; }
        }

        public static bool IsMenuFadingEnabled
        {
            get { return (SystemInformation.UserPreferenceMask[1] & 0x02) != 0; }
        }

        public static bool IsMenuSelectionFadingEnabled
        {
            get { return (SystemInformation.UserPreferenceMask[1] & 0x04) != 0; }
        }

        public static bool IsMenuShadowEnabled
        {
            get { return (SystemInformation.UserPreferenceMask[2] & 0x04) != 0; }
        }

        public static bool SupportsLayeredWindows
        {
            get { return true; }
        }

        public static bool PreferRightAlignedMenus
        {
            get { return true; }
        }

        public static double DoubleClickDelay
        {
            get { return 0.1; }
        }

        public static int DoubleClickRadius2
        {
            get { return 50; }
        }

        internal static int[] UserPreferenceMask
        {
            get { return new int[] { 0xBE, 0x28, 0x06, 0x80 }; }
        }
    }
}

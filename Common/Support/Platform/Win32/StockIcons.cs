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


namespace Epsitec.Common.Support.Platform.Win32
{
    internal static class StockIcons
    {
        public static System.Drawing.Icon ShieldIcon
        {
            get
            {
                ShellApi.StockIconInfo info = StockIcons.CreateIconInfo();
                ShellApi.SHGetStockIconInfo(
                    ShellApi.StockIconIdentifier.Shield,
                    ShellApi.StockIconOptions.Handle,
                    ref info
                );

                return StockIcons.CopyIcon(info.Handle);
            }
        }

        public static System.Drawing.Icon SmallShieldIcon
        {
            get
            {
                ShellApi.StockIconInfo info = StockIcons.CreateIconInfo();
                ShellApi.SHGetStockIconInfo(
                    ShellApi.StockIconIdentifier.Shield,
                    ShellApi.StockIconOptions.Handle | ShellApi.StockIconOptions.Small,
                    ref info
                );

                return StockIcons.CopyIcon(info.Handle);
            }
        }

        private static ShellApi.StockIconInfo CreateIconInfo()
        {
            ShellApi.StockIconInfo info = new ShellApi.StockIconInfo()
            {
                StuctureSize = System.Runtime.InteropServices.Marshal.SizeOf(
                    typeof(ShellApi.StockIconInfo)
                )
            };

            return info;
        }

        private static System.Drawing.Icon CopyIcon(System.IntPtr handle)
        {
            if (handle == System.IntPtr.Zero)
            {
                return null;
            }

            // Return a cloned Icon, because we have to free the original ourselves.

            System.Drawing.Icon icon,
                iconCopy;

            icon = System.Drawing.Icon.FromHandle(handle);
            iconCopy = (System.Drawing.Icon)icon.Clone();
            icon.Dispose();
            ShellApi.DestroyIcon(handle);
            return iconCopy;
        }
    }
}

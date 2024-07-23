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

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Epsitec.Common.OpenType.Platform
{
    public static class FontFinder
    {
        public static IEnumerable<string> FindFontFiles()
        {
            var directories = FontFinder.GetFontDirectories();
            foreach (string directory in directories)
            {
                if (!Directory.Exists(directory))
                {
                    continue;
                }
                foreach (string file in Directory.GetFiles(directory, "*.ttf"))
                {
                    yield return file;
                }
            }
        }

        public static IEnumerable<FontIdentity> FindFontIdentities()
        {
            var fonts = FontFinder.FindFontFiles();
            foreach (string fontpath in fonts)
            {
                // TODO bl-net8-cross
                // we should detect the font name and style using freetype
                FontStyle fontStyle = FontStyle.Normal;
                FontName fontName = new FontName(Path.GetFileName(fontpath), fontStyle);
                yield return new FontIdentity(fontpath, fontName);
            }
        }

        private static IEnumerable<string> GetFontDirectories()
        {
            string fontDir = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            if (FontFinder.IsWindows())
            {
                yield return fontDir;
                yield return Path.Join(localAppData, "Microsoft", "Windows", "Fonts");
                yield break;
            }

            // usually "/usr/share/fonts"
            yield return Path.Join(commonAppData, "fonts");
            // usually "~/.local/share/fonts"
            yield return Path.Join(localAppData, "fonts");
        }

        private static bool IsWindows()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

    }
}

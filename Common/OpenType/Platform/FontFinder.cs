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

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Epsitec.Common.OpenType.Platform
{
    public static class FontFinder
    {
        public static IEnumerable<string> FindFonts()
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

        private static IEnumerable<string> GetFontDirectories()
        {
            string fontDir = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (FontFinder.IsWindows())
            {
                yield return fontDir;
                yield return Path.Join(localAppData, "Microsoft", "Windows", "Fonts");
                yield break;
            }
            // TODO bl-net8-cross
            throw new NotImplementedException();

            //yield return fontDir;
            //yield return localAppData;
            //yield return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            //yield return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            //yield return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            //yield return "/usr/share/fonts";
            //yield return "$HOME/.local/share/fonts";
        }

        private static bool IsWindows()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

    }
}

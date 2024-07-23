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


namespace Epsitec.Common.IO
{
    /// <summary>
    /// The <c>TemporaryFile</c> class can be used to create temporary files, which
    /// are automatically reclaimed when closed.
    /// </summary>
    public class TemporaryFile : System.IDisposable
    {
        public TemporaryFile()
        {
            this.name = System.IO.Path.GetTempFileName();
        }

        public TemporaryFile(string rootPath)
        {
            while (true)
            {
                this.name = System.IO.Path.Combine(rootPath, System.IO.Path.GetRandomFileName());

                if (!System.IO.File.Exists(this.name))
                {
                    break;
                }
            }
        }

        ~TemporaryFile()
        {
            this.Dispose(false);
        }

        public string Path
        {
            get { return this.name; }
        }

        public void Delete()
        {
            //	Tente de supprimer le fichier tout de suite. Si on n'y réussit pas,
            //	ce sera le 'finalizer' qui s'en chargera...

            this.RemoveFile();

            if (this.name == null)
            {
                //	Fichier détruit, plus besoin d'exécuter le 'finalizer'.

                System.GC.SuppressFinalize(this);
            }
        }

        #region IDisposable Members
        public void Dispose()
        {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }
        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //	rien à faire de plus...
            }

            this.RemoveFile();
        }

        protected virtual void RemoveFile()
        {
            if (this.name != null)
            {
                try
                {
                    if (System.IO.File.Exists(this.name))
                    {
                        System.IO.File.Delete(this.name);
                        this.name = null;
                    }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        "Could not remove file " + this.name + ";\n" + ex.ToString()
                    );
                }
            }
        }

        private string name;
    }
}

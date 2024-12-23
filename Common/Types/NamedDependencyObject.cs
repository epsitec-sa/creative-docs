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


namespace Epsitec.Common.Types
{
    public abstract class NamedDependencyObject : DependencyObject, IName, ICaption
    {
        public NamedDependencyObject() { }

        public NamedDependencyObject(string name)
        {
            this.DefineName(name);
        }

        public NamedDependencyObject(Support.Druid captionId)
        {
            this.DefineCaptionId(captionId);
        }

        public NamedDependencyObject(Caption caption)
        {
            this.DefineCaption(caption);
        }

        /// <summary>
        /// Gets the name of the object.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                if (this.name != null)
                {
                    return this.name;
                }

                if ((this.caption == null) && (this.captionId.IsEmpty))
                {
                    return null;
                }

                return this.Caption.Name;
            }
        }

        /// <summary>
        /// Gets the caption of the object.
        /// </summary>
        /// <value>The caption.</value>
        public Caption Caption
        {
            get
            {
                if (this.caption == null)
                {
                    if (this.captionId.IsEmpty)
                    {
                        this.DefineCaption(new Caption());

                        this.caption.Name = this.name;
                        this.name = null;
                    }
                    else
                    {
                        this.DefineCaption(
                            Support.Resources.DefaultManager.GetCaption(this.captionId)
                        );
                    }
                }

                return this.caption;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the caption is defined.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the caption is defined; otherwise, <c>false</c>.
        /// </value>
        public bool IsCaptionDefined
        {
            get
            {
                if ((this.caption == null) && (this.captionId.IsEmpty))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        #region IName Members

        string IName.Name
        {
            get { return this.Name; }
        }

        #endregion

        #region INameCaption Members

        public Support.Druid CaptionId
        {
            get { return this.captionId; }
        }

        #endregion


        internal void Lock()
        {
            this.isReadOnly = true;
        }

        public void DefineName(string name)
        {
            if (this.isReadOnly)
            {
                throw new System.InvalidOperationException(
                    "The name is locked and cannot be changed"
                );
            }

            if (this.caption == null)
            {
                this.name = name;
            }
            else
            {
                this.caption.Name = name;
                this.name = null;
            }
        }

        public void DefineCaptionId(Support.Druid druid)
        {
            if (this.captionId.IsEmpty)
            {
                this.captionId = druid;
            }
            else
            {
                throw new System.InvalidOperationException("The caption DRUID cannot be changed");
            }
        }

        public void DefineCaption(Caption caption)
        {
            if (
                (this.caption == null)
                && (caption != null)
                && (this.captionId.IsEmpty || (this.captionId == caption.Id))
            )
            {
                this.caption = caption;
                this.captionId = caption.Id;

                this.OnCaptionDefined();
            }
            else
            {
                throw new System.InvalidOperationException("The caption cannot be changed");
            }
        }

        protected virtual void OnCaptionDefined() { }

        private string name;
        private bool isReadOnly;
        private Support.Druid captionId;
        private Caption caption;
    }
}

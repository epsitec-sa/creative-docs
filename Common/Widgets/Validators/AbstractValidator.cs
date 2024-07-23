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


using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets.Validators
{
    /// <summary>
    /// La classe AbstractValidator permet de simplifier l'implémentation de
    /// l'interface IValidator en relation avec des widgets.
    /// </summary>
    public abstract class AbstractValidator : IValidator, System.IDisposable
    {
        public AbstractValidator(Widget widget)
        {
            this.widget = widget;

            if (this.widget != null)
            {
                this.AttachWidget(this.widget);
            }
        }

        public Widget Widget
        {
            get { return this.widget; }
        }

        #region IValidator Members

        public abstract void Validate();

        public void MakeDirty(bool deep)
        {
            this.SetState(ValidationState.Dirty);
        }

        public event Support.EventHandler BecameDirty;

        #endregion

        #region IValidationResult Members

        public bool IsValid
        {
            get
            {
                if (this.state == ValidationState.Dirty)
                {
                    this.Validate();
                }

                return (this.state == ValidationState.Ok);
            }
        }

        public ValidationState State
        {
            get { return this.state; }
        }

        public virtual FormattedText ErrorMessage
        {
            get { return FormattedText.Empty; }
        }

        #endregion

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
                if (this.widget != null)
                {
                    this.DetachWidget(this.widget);

                    this.widget = null;
                }
            }
        }

        protected virtual void SetState(ValidationState state)
        {
            if (this.state != state)
            {
                this.state = state;

                if (this.widget != null)
                {
                    if (ValidationContext.IsValidationInProgress(this.widget))
                    {
                        //	Do nothing, this was called because we were validating the
                        //	widget associated with this validator...
                    }
                    else
                    {
                        this.widget.AsyncValidation();
                    }
                }

                if (this.state == ValidationState.Dirty)
                {
                    this.OnBecameDirty();
                }
            }
        }

        protected virtual void OnBecameDirty()
        {
            if (this.BecameDirty != null)
            {
                this.BecameDirty(this);
            }
        }

        protected virtual void AttachWidget(Widget widget)
        {
            widget.AddValidator(this);
        }

        protected virtual void DetachWidget(Widget widget)
        {
            widget.RemoveValidator(this);
        }

        private ValidationState state;
        private Widget widget;
    }
}

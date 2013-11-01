//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractEditor
	{
		public AbstractEditor(DataAccessor accessor, BaseType baseType)
		{
			this.accessor = accessor;
			this.baseType = baseType;
		}


		public bool EditionDirty
		{
			get
			{
				return this.editionDirty;
			}
		}

		public virtual void CreateUI(Widget parent)
		{
		}

		public virtual void OpenMainPage(EventType eventType)
		{
		}


		protected void StartEdition(Guid objectGuid, Timestamp? timestamp)
		{
			this.accessor.StartObjectEdition (this.baseType, objectGuid, timestamp);

			if (this.editionDirty)
			{
				this.editionDirty = false;
				this.OnValueChanged (ObjectField.Unknown);
			}
		}

		protected void SetEditionDirty(ObjectField field)
		{
			if (!this.editionDirty)
			{
				this.editionDirty = true;
				this.OnValueChanged (field);
			}
		}


		#region Events handler
		private void OnValueChanged(ObjectField field)
		{
			if (this.ValueChanged != null)
			{
				this.ValueChanged (this, field);
			}
		}

		public delegate void ValueChangedEventHandler(object sender, ObjectField field);
		public event ValueChangedEventHandler ValueChanged;
		#endregion


		protected readonly DataAccessor			accessor;
		protected readonly BaseType				baseType;

		private bool							editionDirty;
	}
}

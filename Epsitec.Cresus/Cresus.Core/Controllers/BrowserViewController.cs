//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Cresus.Core.Controllers
{
	class BrowserViewController : CoreViewController, INotifyCurrentChanged
	{
		public BrowserViewController(string name)
			: base (name)
		{
			this.collection = new List<AbstractEntity> ();
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			new StaticText ()
			{
				Parent = container,
				BackColor = Color.FromBrightness (1),
				Dock = DockStyle.Top,
				PreferredHeight = 40,
				Text = "Ici viendra la future <i>liste de gauche</i> Crésus.",
				TextBreakMode = TextBreakMode.Hyphenate,
				ContentAlignment = ContentAlignment.MiddleLeft,
			};

			this.scrollList = new ScrollList ()
			{
				Parent = container,
				Dock = DockStyle.Fill,
				ScrollListStyle = ScrollListStyle.Standard,
			};

			this.scrollList.SelectedIndexChanged +=
				delegate
				{
					int active = this.scrollList.SelectedIndex;
                    
					if (active != this.activeIndex)
					{
						this.OnCurrentChanging (new CurrentChangingEventArgs (isCancelable: false));
						this.activeIndex = active;
						this.OnCurrentChanged ();
                    }
				};
		}


		public AbstractEntity ActiveEntity
		{
			get
			{
				if (this.activeIndex < 0)
				{
					return null;
				}
				else
				{
					return this.collection[this.activeIndex];
				}
			}
		}


		public void SetContents(IEnumerable<AbstractEntity> collection)
		{
			this.OnCurrentChanging (new CurrentChangingEventArgs (isCancelable: false));
			this.collection.Clear ();
			this.collection.AddRange (collection);
			this.RefreshScrollList ();
			this.OnCurrentChanged ();
		}

		protected void OnCurrentChanged()
		{
			var handler = this.CurrentChanged;

			if (handler != null)
			{
				handler (this);
			}
		}

		protected void OnCurrentChanging(CurrentChangingEventArgs e)
		{
			var handler = this.CurrentChanging;

			if (handler != null)
			{
				handler (this, e);
			}
		}

		private void RefreshScrollList()
		{
			if (this.scrollList != null)
			{
				this.scrollList.Items.Clear ();

				foreach (var entity in this.collection)
				{
					if (entity is LegalPersonEntity)
                    {
						var person = entity as LegalPersonEntity;

						this.scrollList.Items.Add (person.Name);
                    }
					else if (entity is NaturalPersonEntity)
                    {
						var person = entity as NaturalPersonEntity;

						this.scrollList.Items.Add (Misc.SpacingAppend (person.Firstname, person.Lastname));
					}
				}
			}
		}

        #region INotifyCurrentChanged Members

		public event EventHandler  CurrentChanged;

		public event EventHandler<CurrentChangingEventArgs>  CurrentChanging;

		#endregion

		private readonly List<AbstractEntity> collection;

		private ScrollList scrollList;
		private int activeIndex = -1;
	}
}

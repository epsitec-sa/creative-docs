//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Core.Controllers.BrowserControllers;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types.Converters;

namespace Epsitec.Cresus.Core.Bricks.Factories
{
	public sealed class SearchPanelFactory<T> : StaticFactory
		where T : AbstractEntity
	{
		public SearchPanelFactory(BusinessContext businessContext, ExpandoObject settings)
			: base (businessContext, settings)
		{
		}

		public override void CreateUI(FrameBox container, UIBuilder builder)
		{
			FormattedText searchTitle  = this.settings.SearchTitle;
			FormattedText buttonTitle  = this.settings.ButtonTitle;
			System.Action<AbstractEntity> buttonAction = this.settings.ButtonAction;

			this.search = new SearchPicker ()
			{
				Margins = new Margins (4, 4, 4, 0)
			};

			var messageEmpty = new StaticText ()
			{
				Parent = this.search.MessageContainerStateEmpty,
				Dock = DockStyle.Fill,
				ContentAlignment = ContentAlignment.MiddleCenter,
				FormattedText = new FormattedText ("<i>Aucun résultat</i>"),
			};

			var messageError = new StaticText ()
			{
				Parent = this.search.MessageContainerStateError,
				Dock = DockStyle.Fill,
				ContentAlignment = ContentAlignment.MiddleCenter,
				FormattedText = new FormattedText ("<i>Trop de résultats</i>"),
			};
			
			var button = new Button ()
			{
				Margins = new Margins (4, 4, 8, 4),
				FormattedText = buttonTitle,
			};

			button.Clicked += delegate
			{
				buttonAction (this.browser.SelectedEntity);
			};

			this.search.SearchTextChanged += _ => this.Search ();
			this.search.SearchClicked     += _ => this.Search ();

			//	TODO: implement browser using BigList...
			
//#			this.browser = new BrowserScrollListController (this.businessContext.Data, this.search.ScrollList, typeof (T));
			this.search.State = SearchPickerState.Empty;

			builder.Add (container, search);
			builder.Add (container, button);
		}

		private void Search()
		{
			var words = this.GetSearchWords ();
			var filter = new Filter (words);
			
			this.browser.Filter = filter.Predicate;

			this.search.State = SearchPickerState.Ready;
		}

		class Filter
		{
			public Filter(string[] words)
			{
				this.words = new List<string> (words);
			}

			public bool Predicate(AbstractEntity entity)
			{
				var keywords   = string.Join (";", entity.GetEntityKeywords ());
				var search     = string.Join (" ", SearchPanelFactory<T>.GetSearchWords (keywords));

				foreach (var word in this.words)
				{
					if (search.Contains (word) == false)
					{
						return false;
					}
				}

				return true;
			}

			private readonly List<string>		words;
		}

		private string[] GetSearchWords()
		{
			var text = this.search.SearchText.ToSimpleText ();
			
			return GetSearchWords (text);
		}

		private static string[] GetSearchWords(string text)
		{
			text = TextConverter.ConvertToUpperAndStripAccents (text);
			return text.Split (' ', ',', ';', ':', '/', '-').Select (x => x.Trim ()).Where (x => x.Length > 0).ToArray ();
		}

		private SearchPicker					search;
		private BrowserScrollListController		browser;
	}
}

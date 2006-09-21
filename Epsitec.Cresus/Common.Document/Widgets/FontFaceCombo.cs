using Epsitec.Common.Widgets;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe FontFaceCombo impl�mente la ligne �ditable avec bouton "v" pour
	/// choisir une police.
	/// </summary>
	public class FontFaceCombo : TextFieldCombo
	{
		public FontFaceCombo()
		{
		}
		
		public FontFaceCombo(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public System.Collections.ArrayList		FontList
		{
			get
			{
				return this.fontList;
			}
			set
			{
				this.fontList = value;
			}
		}

		public double							SampleHeight
		{
			get
			{
				return this.sampleHeight;
			}
			set
			{
				this.sampleHeight = value;
			}
		}

		public bool								SampleAbc
		{
			get
			{
				return this.sampleAbc;
			}

			set
			{
				this.sampleAbc = value;
			}
		}

		public int								QuickCount
		{
			get
			{
				return this.quickCount;
			}
			set
			{
				this.quickCount = value;
			}
		}


		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				//	TODO: ...
			}
			
			base.Dispose(disposing);
		}

		protected override void Navigate(int dir)
		{
		}
		
		
		protected override AbstractMenu CreateMenu()
		{
			this.fontSelector = new FontSelector(null);
			this.fontSelector.FontList = this.fontList;
			this.fontSelector.QuickCount = this.quickCount;
			this.fontSelector.SampleHeight = this.sampleHeight;
			this.fontSelector.SampleAbc = this.sampleAbc;
			
			TextFieldComboMenu menu = new TextFieldComboMenu();
			menu.Contents = this.fontSelector;
			menu.AdjustSize();
			
			//	On n'a pas le droit de d�finir le "SelectedFontFace" avant d'avoir fait
			//	cette mise � jour du contenu avec la nouvelle taille ajust�e, sinon on
			//	risque d'avoir un offset incorrect pour le d�but...
			this.fontSelector.UpdateContents();
			this.fontSelector.SelectedFontFace = this.Text;
			this.fontSelector.SelectionChanged += new Support.EventHandler(this.HandleSelectorSelectionChanged);
			
			MenuItem.SetMenuHost(this, new ScrollableMenuHost(menu));
			
			return menu;
		}

		
		protected override void OnComboClosed()
		{
			base.OnComboClosed();
			
			if ( this.fontSelector != null )
			{
				this.fontSelector.SelectionChanged -= new Support.EventHandler(this.HandleSelectorSelectionChanged);
				this.fontSelector.Dispose();
				this.fontSelector = null;
			}

			if ( this.Window != null )
			{
				this.Window.RestoreLogicalFocus();
			}
		}
		
		
		private void HandleSelectorSelectionChanged(object sender)
		{
			//	L'utilisateur a cliqu� dans la liste pour terminer son choix.
			string text = this.fontSelector.SelectedFontFace;
			if ( this.Text != text )
			{
				this.Text = TextLayout.ConvertToTaggedText(text);
				this.SelectAll();
			}

			this.CloseCombo(CloseMode.Accept);
		}
		
		
		private System.Collections.ArrayList	fontList;
		private double							sampleHeight;
		private bool							sampleAbc;
		private int								quickCount;
		private FontSelector					fontSelector;
	}
}

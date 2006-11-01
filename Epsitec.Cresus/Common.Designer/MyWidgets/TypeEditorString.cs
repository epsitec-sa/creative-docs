using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant d'éditer un Caption.Type.
	/// </summary>
	public class TypeEditorString : AbstractTypeEditor
	{
		public TypeEditorString()
		{
			Widget group;

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.String.Min, this, out group, out this.fieldMin);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 2);
			this.fieldMin.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.String.Max, this, out group, out this.fieldMax);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 10);
			this.fieldMax.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.checkFixedLength = new CheckButton(this);
			this.checkFixedLength.Text = "Longueur fixe dans la base de données";
			this.checkFixedLength.Dock = DockStyle.StackBegin;
			this.checkFixedLength.Margins = new Margins(0, 0, 0, 3);
			this.checkFixedLength.Clicked += new MessageEventHandler(this.HandleCheckClicked);

			this.checkMultilingual = new CheckButton(this);
			this.checkMultilingual.Text = "Le texte existe en plusieurs langues";
			this.checkMultilingual.Dock = DockStyle.StackBegin;
			this.checkMultilingual.Margins = new Margins(0, 0, 0, 0);
			this.checkMultilingual.Clicked += new MessageEventHandler(this.HandleCheckClicked);
		}

		public TypeEditorString(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldMin.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldMax.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				this.checkFixedLength.Clicked -= new MessageEventHandler(this.HandleCheckClicked);
				this.checkMultilingual.Clicked -= new MessageEventHandler(this.HandleCheckClicked);
			}
			
			base.Dispose(disposing);
		}


		protected override void UpdateContent()
		{
			//	Met à jour le contenu de l'éditeur.
			StringType type = this.AbstractType as StringType;

			this.ignoreChange = true;
			this.SetDecimal(this.fieldMin, type.MinimumLength);
			this.SetDecimal(this.fieldMax, type.MaximumLength);
			this.checkFixedLength.ActiveState = type.UseFixedLengthStorage ? ActiveState.Yes : ActiveState.No;
			this.checkMultilingual.ActiveState = type.UseMultilingualStorage ? ActiveState.Yes : ActiveState.No;
			this.ignoreChange = false;
		}


		private void HandleTextFieldChanged(object sender)
		{
			if (this.ignoreChange)
			{
				return;
			}

			//	[Note1] On demande le type avec un ResourceAccess.GetField.
			StringType type = this.AbstractType as StringType;

			if (sender == this.fieldMin)
			{
				type.DefineMinimumLength((int) this.GetDecimal(this.fieldMin));
			}

			if (sender == this.fieldMax)
			{
				type.DefineMaximumLength((int) this.GetDecimal(this.fieldMax));
			}

			//	[Note1] Cet appel va provoquer le ResourceAccess.SetField.
			this.OnContentChanged();
		}

		private void HandleCheckClicked(object sender, MessageEventArgs e)
		{
			if (this.ignoreChange)
			{
				return;
			}

			//	[Note1] On demande le type avec un ResourceAccess.GetField.
			StringType type = this.AbstractType as StringType;

			if (sender == this.checkFixedLength)
			{
				type.DefineUseFixedLengthStorage(!type.UseFixedLengthStorage);
			}

			if (sender == this.checkMultilingual)
			{
				type.DefineUseMultilingualStorage(!type.UseMultilingualStorage);
			}

			//	[Note1] Cet appel va provoquer le ResourceAccess.SetField.
			this.OnContentChanged();
		}


		protected TextField						fieldMin;
		protected TextField						fieldMax;
		protected CheckButton					checkFixedLength;
		protected CheckButton					checkMultilingual;
	}
}

//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA, 29/04/2004

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Controllers
{
	/// <summary>
	/// La classe WidgetNamedSelController réalise un contrôleur très simple qui
	/// s'appuie sur un widget existant implémentant INamedStringSelection; elle
	/// interagit avec les propriétés SelectedIndex et SelectedName.
	/// </summary>
	public class WidgetNamedSelController : AbstractConstrainedController
	{
		public WidgetNamedSelController()
		{
		}
		
		public WidgetNamedSelController(Adapters.IAdapter adapter) : this ()
		{
			this.Adapter = adapter;
		}
		
		public WidgetNamedSelController(Adapters.IAdapter adapter, Widget widget) : this ()
		{
			this.Adapter = adapter;
			this.CreateUI (widget);
		}
		
		public WidgetNamedSelController(Adapters.IAdapter adapter, Widget widget, Types.IDataConstraint constraint, Types.INamedType type) : this ()
		{
			this.Adapter    = adapter;
			this.Constraint = constraint;
			this.DataType   = type;
			this.CreateUI (widget);
		}
		
		
		public Types.INamedType					DataType
		{
			get
			{
				return this.data_type;
			}
			set
			{
				this.data_type = value;
			}
		}
		
		
		public override void CreateUI(Widget widget)
		{
			Support.Data.INamedStringSelection           named_sel   = widget as Support.Data.INamedStringSelection;
			Common.Widgets.Helpers.IStringCollectionHost string_host = widget as Common.Widgets.Helpers.IStringCollectionHost;
			
			if (named_sel == null)
			{
				throw new System.ArgumentException ("The specified widget does not conform to INamedStringSelection interface.", "widget");
			}
			
			this.widget    = widget;
			this.named_sel = named_sel;
			
			Types.IEnum enum_type = this.data_type as Types.IEnum;
			
			if ((enum_type != null) &&
				(string_host != null))
			{
				//	La donnée est de type 'enum' et le widget accepte des listes de valeurs.
				
				this.enum_values = enum_type.Values;
				
				if (string_host.Items.Count == 0)
				{
					//	Le widget est vide; on va donc le remplir. On va utiliser les 'captions'
					//	si elles existent, sinon on utilisera le nom brut :
					
					for (int i = 0; i < enum_values.Length; i++)
					{
						string name    = enum_values[i].Name;
						string caption = enum_values[i].Caption;
						
						if (name != null)
						{
							if (caption == null)
							{
								caption = name;
							}
							
							string_host.Items.Add (name, caption);
						}
					}
				}
				else
				{
					bool clear = true;
					
					//	Le widget a déjà des valeurs définies. Si l'énumération fournie possède
					//	des 'captions', alors on redéfinit le contenu du widget, sinon on le
					//	laisse dans son état initial :
					
					for (int i = 0; i < enum_values.Length; i++)
					{
						string name    = enum_values[i].Name;
						string caption = enum_values[i].Caption;
						
						if ((name != null) &&
							(caption != null))
						{
							if (clear)
							{
								string_host.Items.Clear ();
								clear = false;
							}
							
							string_host.Items.Add (name, caption);
						}
					}
				}
			}
			
			this.named_sel.SelectedIndexChanged += new EventHandler(HandleSelectedIndexChanged);
			this.widget.TextChanged += new EventHandler(HandleTextChanged);
			
			this.SyncFromAdapter (SyncReason.Initialisation);
		}
		
		public override void SyncFromAdapter(SyncReason reason)
		{
			Adapters.DecimalAdapter num_adapter  = this.Adapter as Adapters.DecimalAdapter;
			Adapters.StringAdapter  text_adapter = this.Adapter as Adapters.StringAdapter;
			
			if (this.named_sel == null)
			{
				return;
			}
			
			if ((num_adapter != null) &&
				(this.enum_values != null))
			{
				int rank  = (int) num_adapter.Value;
				int index = -1;
				
				for (int i = 0; i < this.enum_values.Length; i++)
				{
					if (this.enum_values[i].Rank == rank)
					{
						index = i;
						break;
					}
				}
				
				this.named_sel.SelectedIndex = index;
				
				return;
			}
			
			if (text_adapter != null)
			{
				string value = text_adapter.Value;
				
				//	Vérifions tout d'abord si la valeur correspond à un texte personnalisé...
				
				if (Types.CustomEnumType.IsCustomName (value))
				{
					this.widget.Text = Types.CustomEnumType.FromCustomName (value);
				}
				else
				{
					this.named_sel.SelectedName = value;
				}
			}
		}
		
		public override void SyncFromUI()
		{
			Adapters.DecimalAdapter num_adapter  = this.Adapter as Adapters.DecimalAdapter;
			Adapters.StringAdapter  text_adapter = this.Adapter as Adapters.StringAdapter;
			
			if (this.named_sel == null)
			{
				return;
			}
			
			if ((num_adapter != null) &&
				(this.enum_values != null))
			{
				int index = this.named_sel.SelectedIndex;
				
				if ((index < 0) ||
					(index >= this.enum_values.Length))
				{
					//	On ne peut pas affecter de valeur à l'adaptateur, car elle ne
					//	fait pas partie des choix possibles.
				}
				else
				{
					int rank = this.enum_values[index].Rank;
					
					if (this.CheckConstraint (rank))
					{
						num_adapter.Value = rank;
					}
				}
				
				return;
			}
			
			if (text_adapter != null)
			{
				string value = this.named_sel.SelectedName;
				
				Types.IReadOnly read_only = this.widget as Types.IReadOnly;
				
				if ((value == null) &&
					(read_only != null) &&
					(read_only.IsReadOnly == false))
				{
					//	Il n'y a pas d'élément sélectionné dans la liste, mais peut-être y a-t-il un
					//	text personnalisé à la place ?
					
					value = Types.CustomEnumType.ToCustomName (this.widget.Text);
				}
				
				if (this.CheckConstraint (value))
				{
					text_adapter.Value = value;
				}
			}
		}
		
		
		private void HandleSelectedIndexChanged(object sender)
		{
			this.SyncFromUI ();
		}
		
		private void HandleTextChanged(object sender)
		{
			this.SyncFromUI ();
		}
		
		
		private Widget							widget;
		Support.Data.INamedStringSelection		named_sel;
		private Types.INamedType				data_type;
		private Types.IEnumValue[]				enum_values;
	}
}

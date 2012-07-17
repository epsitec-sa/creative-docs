Ext.define('Epsitec.Cresus.Core.Static.EnumerationField',
  {
    extend : 'Ext.container.Container',
    alias : 'widget.epsitec.enumerationfield',
    
    /* Config */
    layout : 'column',
    
    /* Constructor */
    constructor : function (options)
    {
      options.columnWidth = 1;
      
      var combo = Ext.create('Epsitec.Cresus.Core.Static.EnumerationComboBox', options);
      
      this.items = this.items || new Array();
      this.items.push(combo);
      
      this.callParent();
      
      return this;
    },
  }
);
 
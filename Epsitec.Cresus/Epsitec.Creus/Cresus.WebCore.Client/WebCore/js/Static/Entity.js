Ext.define('Epsitec.Cresus.Core.Static.Entity',
  {
    extend : 'Ext.container.Container',
    alias : 'widget.epsitec.entity',
    
    /* Config */
    layout : 'column',
    
    /* Constructor */
    constructor : function (options)
    {
      
      options.columnWidth = 1;
      
      var combo = Ext.create('Epsitec.Cresus.Core.Static.EntityComboBox', options);
      
      var button = Ext.create('Ext.Button',
          {
            text : '>',
            renderTo : Ext.getBody(),
            handler : function ()
            {
              Ext.Msg.alert('Cannot edit this list', 'You cannot directly edit this list. You will need to save the current changes, click the header menu to edit the corresponding list, and come back to this entity to edit it.');
            },
            margin : '19 0 0 5'
          }
        );
      
      this.items = this.items || new Array();
      this.items.push(combo);
      this.items.push(button);
      
      this.callParent();
      
      return this;
    },
  }
);
 
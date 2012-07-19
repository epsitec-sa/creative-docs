Ext.define('Epsitec.Cresus.Core.Static.EntityListPanel',
  {
    extend : 'Epsitec.Cresus.Core.Static.ColumnPanel',
    
    /* Config */
    layout : 'fit',
    
    /* Constructor */ 
    constructor : function(options)
    {
      this.callParent(arguments);
      
      var databaseName = options.databaseName;
      
      var entityList = Ext.create('Epsitec.Cresus.Core.Static.EntityList', databaseName);    
      
      this.add(entityList);
      
      return this;
    }
  }
);
 
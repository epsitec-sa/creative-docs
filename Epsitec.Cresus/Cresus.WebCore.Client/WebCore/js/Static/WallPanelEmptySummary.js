Ext.define('Epsitec.Cresus.Core.Static.WallPanelEmptySummary',
  {
    extend : 'Epsitec.Cresus.Core.Static.WallPanelCollectionSummary',
    alias : 'widget.emptysummary',
    
    /* Config */
    html : 'Empty',
    
    /* Constructor */
    constructor : function (o)
    {
      var options = o || {};
      
      options.hideRemoveButton = true;
      
      this.callParent(new Array(options));
      return this;
    },
    
    /* Additional methods */
    
    // Override
    bodyClicked : function ()
    {
      this.addEntity();
    },
  }
);
 
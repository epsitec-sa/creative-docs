Ext.define('Epsitec.Cresus.Core.Static.EnumComboBox',
  {
    extend : 'Ext.form.field.ComboBox',
    alias : 'widget.epsitec.enumcombo',
    
    /* Config */
    forceSelection : true,
    typeAhead : true,
    
    /* Properties */
    // URL of the content
    storeClass : null,
    
    /* Constructor */
    constructor : function (options)
    {
      this.store = Epsitec.Cresus.Core.Static.EnumComboBox.getStore(options.storeClass, this);
      
      this.valueField = 'id';
      this.displayField = 'name';
      this.queryMode = 'local';
      
      this.callParent(arguments);
      
      return this;
    },
    
    /* Additional methods */
    statics :
    {
      getStore : function (name, combo)
      {
        this.stores = this.stores || new Array();
        var store = this.stores[name];
        
        if (store != null)
        {
          return store;
        }
        
        combo.setLoading();
        
        // Create a proxy to get info about the Enum
        // We want to get the info using the POST method,
        // because we have to specify the type (name)
        var proxy = Ext.create('Ext.data.proxy.Ajax',
            {
              type : 'ajax',
              url : 'proxy/enum/',
              reader :
              {
                type : 'json'
              },
              extraParams :
              {
                name : name
              },
              actionMethods :
              {
                reader : 'POST'
              }
            }
          );
        
        store = Ext.create('Ext.data.Store',
            {
              fields : ['id', 'name'],
              autoLoad : true,
              proxy : proxy,
              listeners :
              {
                load :
                {
                  fn : function (store)
                  {
                    this.select(this.value);
                    this.setLoading(false);
                  },
                  scope : combo
                }
              }
            }
          );
        
        this.stores[name] = store;
        
        return store;
        
      }
    }
  }
);
 
// This class uses Stores to keep the possible values for the enums and share them between
// instances. That is, all the EnumComboBoxes that target the same enum will share the same store.
// This means that the data for the enum is loaded only once. The place where the Stores are created
// or retrieved is the getStore(...) method, which creates it if it does not exists or retrieves it
// if it does exists.

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
      // We need to pass the options.value parameter because the parent constructor has not yet been
      // called and thus this.value is undefined at this time and we might require it in this call.
      this.store = Epsitec.Cresus.Core.Static.EnumComboBox.getStore(options.storeClass, this, options.value);
      
      this.valueField = 'id';
      this.displayField = 'name';
      this.queryMode = 'local';
      
      this.callParent(arguments);
      
      return this;
    },
    
    /* Additional methods */
    statics :
    {
      getStore : function (name, combo, value)
      {
        this.stores = this.stores || new Array();
        var store = this.stores[name];
        
        if (store != null)
        {
          return store;
        }
        
        combo.setLoading();
        
        // Create a proxy to get info about the Enum. We want to get the info using the POST method,
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
                // This callback will be called only once, for the first EnumComboBox that creates
                // the store, in order to assign it its value. The callback is not neccary for the
                // following EnumComboBoxes, as the store will already be populated and the value
                // of the ComboBox will be automatically set.
                load :
                {
                  fn : function (store)
                  {
                    this.select(value);
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
 
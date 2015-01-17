// This class is an edition field that lets the user choose a value from an
// enumeration, by using a combo box.

Ext.require([
  'Epsitec.cresus.webcore.tools.Enumeration'
],
function() {
  Ext.define('Epsitec.cresus.webcore.field.EnumerationField', {
    extend: 'Ext.form.field.ComboBox',
    alternateClassName: ['Epsitec.EnumerationField'],
    alias: 'widget.epsitec.enumerationfield',

    /* Configuration */

    valueField: 'id',
    displayField: 'text',
    queryMode: 'local',
    forceSelection: true,
    typeAhead: true,

    /* Constructor */

    constructor: function(options) {
      var store, newOptions;

      store = Epsitec.Enumeration.getStore(options.enumerationName);

      if (!store.isLoaded)
      {
        store.on('load', function() { this.select(options.value); }, this);
      }

      newOptions = {
        store: store
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    }
  });
});

Ext.define('Epsitec.cresus.webcore.EnumerationField', {
  extend: 'Ext.form.field.ComboBox',
  alternateClassName: ['Epsitec.EnumerationField'],
  alias: 'widget.epsitec.enumerationfield',

  /* Config */

  valueField: 'id',
  displayField: 'text',
  queryMode: 'local',
  forceSelection: true,
  typeAhead: true,

  /* Constructor */

  constructor: function(options) {
    var store, newOptions;

    store = Epsitec.Enumeration.getStore(options.enumerationName);

    store.on('load', function() { this.select(options.value); }, this);

    newOptions = {
      store: store
    };
    Ext.applyIf(newOptions, options);

    this.callParent([newOptions]);
    return this;
  }
});

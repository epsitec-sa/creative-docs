Ext.define('Epsitec.cresus.webcore.EnumerationField', {
  extend: 'Ext.container.Container',
  alternateClassName: ['Epsitec.EnumerationField'],
  alias: 'widget.epsitec.enumerationfield',

  /* Config */

  layout: 'column',

  /* Constructor */

  constructor: function(options) {
    options.columnWidth = 1;

    var combo = Ext.create('Epsitec.EnumerationComboBox', options);

    this.items = this.items || [];
    this.items.push(combo);

    this.callParent(arguments);
    return this;
  }
});

Ext.define('Epsitec.cresus.webcore.EnumerationField', {
  extend: 'Ext.container.Container',
  alias: 'widget.epsitec.enumerationfield',

  /* Config */

  layout: 'column',

  /* Constructor */

  constructor: function(options) {
    options.columnWidth = 1;

    var combo = Ext.create(
        'Epsitec.cresus.webcore.EnumerationComboBox', options
        );

    this.items = this.items || [];
    this.items.push(combo);

    this.callParent();

    return this;
  }
});


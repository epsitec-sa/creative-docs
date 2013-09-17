Ext.require([
],
function() {
  Ext.define('Epsitec.cresus.webcore.ui.FaqWindow', {
    extend: 'Ext.Window',
    alternateClassName: ['Epsitec.FaqWindow'],

    /* Properties */

    /* Constructor */

    constructor: function() {
      var config;

      config = {
          title : 'FAQ Aider',
          width : 1024,
          height: 600,
          resizable: false,
          closable: true,
          closeAction: 'hise', 
          layout : 'fit',
          bodyStyle: {
              top: '25px',
              left: '80px'
          },
          frame: false,
          border: false,
          closable: false,
          dockedItems: this.createToolbar(),
          items : [{
              xtype : "component",
              frame: false,
              border: false,
              autoEl : {
                  tag : "iframe",
                  src: "http://faq-aider.eerv.ch/"
              }
          }]
      };
      
      this.callParent([config]);

      return this;
    },
    /* Methods */
    createToolbar: function () {
      var openbutton = {
          xtype: 'button',
          text: 'Ouvrir dans le navigateur',
          cls: 'tile-button',
          overItemCls: 'tile-button-over',
          textAlign: 'left',
          handler: this.openInBrowser,
          scope: this
      };
          
      var hidebutton = {
          xtype: 'button',
          text: 'Fermer la FAQ',
          cls: 'tile-button',
          overItemCls: 'tile-button-over',
          textAlign: 'left',
          handler: this.hideMe,
          scope: this
      };
      return Ext.create('Ext.Toolbar', {
            dock: 'bottom',
            items: [openbutton,'->',hidebutton]
          });
    },

    hideMe: function () {
      this.hide();
    },

    openInBrowser: function() {
      var win=window.open('http://faq-aider.eerv.ch/', '_blank');
      win.focus();
      this.hide();
    }
  });
});

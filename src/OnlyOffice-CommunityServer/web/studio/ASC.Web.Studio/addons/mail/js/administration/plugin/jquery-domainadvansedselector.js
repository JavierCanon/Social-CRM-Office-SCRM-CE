/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


(function($) {
    var domainadvancedSelector = function(element, options) {
        this.$element = $(element);
        this.options = $.extend({}, $.fn.domainadvancedSelector.defaults, options);
        this.init();
    };

    domainadvancedSelector.prototype = $.extend({}, $.fn.advancedSelector.Constructor.prototype, {
        constructor: domainadvancedSelector,
        initAdvSelectorData: function() {
            var that = this,
                data = [];

            var domains = this.options.getDomains();
            for (var i = 0, length = domains.length; i < length; i++) {
                data.push({
                    title: domains[i].name,
                    id: domains[i].id
                });
            }
            if (data.length > 0) {
                that.rewriteObjectItem.call(that, data);
            }

            that.$element.unbind('click.onshow').bind('click.onshow', function() {
                that.refrashSelectorData();
            });
        },

        refrashSelectorData: function() {
            var that = this;
            var needRefrash = false;
            var domains = $.map(that.options.getDomains(), function(domain) {
                return {
                    title: domain.name,
                    id: domain.id
                };
            });
            var newItems = that.items.slice();

            for (var i = 0, length = domains.length; i < length; i++) {
                if (!that.getItemById(domains[i].id, that.items)) {
                    newItems.push(domains[i]);
                    needRefrash = true;
                }
            }

            for (i = 0, length = that.items.length; i < length; i++) {
                if (!that.getItemById(that.items[i].id, domains)) {
                    newItems.splice(i, 1);
                    needRefrash = true;
                }
            }

            if (needRefrash) {
                that.items = newItems;
                that.items = that.items.sort(SortData);
                that.$element.data('items', that.items);
                that.showItemsListAdvSelector.call(that);
            }
        },

        rewriteObjectItem: function(data) {
            var that = this;
            that.items = data;
            that.items = that.items.sort(SortData);
            that.$element.data('items', that.items);
            that.showItemsListAdvSelector.call(that);
        },

        getItemById: function(id, itemList) {
            for (var i = 0, len = itemList.length; i < len; i++) {
                if (id == itemList[i].id) {
                    return itemList[i];
                }
            }
            return undefined;
        }
    });

    $.fn.domainadvancedSelector = function(option) {
        var selfargs = Array.prototype.slice.call(arguments, 1);
        return this.each(function() {
            var $this = $(this),
                data = $this.data('domainadvancedSelector'),
                options = $.extend({},
                    $.fn.domainadvancedSelector.defaults,
                    $this.data(),
                    typeof option == 'object' && option);
            if (!data) {
                $this.data('domainadvancedSelector', (data = new domainadvancedSelector(this, options)));
            }
            if (typeof option == 'string') {
                data[option].apply(data, selfargs);
            }
        });
    };

    $.fn.domainadvancedSelector.defaults = $.extend({}, $.fn.advancedSelector.defaults, {});

})(jQuery, window, document, document.body);
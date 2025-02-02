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


var TimeAndLanguage = new function () {
    this.SaveLanguageTimeSettings = function () {
        AjaxPro.onLoading = function (b) {
            if (b) {
                LoadingBanner.showLoaderBtn("#studio_lngTimeSettings");
            } else {
                LoadingBanner.hideLoaderBtn("#studio_lngTimeSettings");
            }
        };
        var timeManager = new TimeAndLanguageContentManager();
        timeManager.SaveTimeLangSettings(function (res) {
            if (res.Status == "1") {
                LoadingBanner.showMesInfoBtn("#studio_lngTimeSettings", res.Message, "success");
                window.location.reload(true);
            } else {
                LoadingBanner.showMesInfoBtn("#studio_lngTimeSettings", res.Message, "error");
            }
        });
    };
};

TimeAndLanguageContentManager = function () {
    this.SaveTimeLangSettings = function (parentCallback) {
        Teamlab.setTimaAndLanguage(jq("#studio_lng").val() || jq('#studio_lng').data('default'), jq("#studio_timezone").val(), {
            success: function (params, response) {
                if (parentCallback != null)
                    parentCallback({Status: 1, Message: response});
            },
            error: function (params, response) {
                if (parentCallback != null)
                    parentCallback({Status: 0, Message: response[0]});
            }
        });
    };
};

jq(function () {
    var previous;

    if (jq("#NotFoundLanguage").length) {
        jq("#studio_lng").on('focus', function () {
            previous = this.value;
        }).change(function () {
            if (!this.value) {
                setTimeout(function() {
                    jq(".langTimeZoneBlock .HelpCenterSwitcher").helper({ BlockHelperID: 'NotFoundLanguage' });
                }, 0);
                this.value = previous;
                return false;
            } else {
                previous = this.value;
            }
        });
    }
});
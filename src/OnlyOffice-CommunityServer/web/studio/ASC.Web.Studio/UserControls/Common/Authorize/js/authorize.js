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


var Authorize = new function () {

    jq(document).ready(function () {

        if (jq("#recaptchaHiddenContainer").is(":visible")) {
            RecaptchaController.InitRecaptcha(jq("#recaptchaHiddenContainer").attr("data-hl"));
        }

        jq(jq("#login").val().length ? "#pwd" : "#login").focus();

        jq("#login,#pwd").keyup(function (event) {
            var code;
            if (!e) {
                var e = event;
            }
            if (e.keyCode) {
                code = e.keyCode;
            } else if (e.which) {
                code = e.which;
            }

            if (code == 13) {
                if (jq(this).is("#login") && !jq("#pwd").val().length) {
                    jq("#pwd").focus();
                    return true;
                }
                if (jq('body').hasClass('desktop') && !!jq("#desktop_agree_to_terms").length && !(jq("#desktop_agree_to_terms").is(':checked'))) {
                    return true;
                }
                Authorize.Login();
            } else if (code == 27) {
                jq(this).val("");
            }
            return true;
        });

        try {
            var anch = ASC.Controls.AnchorController.getAnchor();
            if (jq.trim(anch) == "passrecovery") {
                PasswordTool.ShowPwdReminderDialog();
            }
        } catch (e) { }

    });

    this.Login = function () {
        window.submitForm();
    };
};
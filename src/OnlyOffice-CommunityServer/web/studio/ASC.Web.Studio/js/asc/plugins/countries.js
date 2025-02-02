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


var CountriesManager = new function () {
    var countriesList =
    [
        { title: ASC.Resources.Countries.Afghanistan, key: "AF", country_code: "+93" },
        { title: ASC.Resources.Countries.Albania, key: "AL", country_code: "+355" },
        { title: ASC.Resources.Countries.Algeria, key: "DZ", country_code: "+213" },
        { title: ASC.Resources.Countries.AmericanSamoa, key: "AS", country_code: "+1684" },
        { title: ASC.Resources.Countries.Andorra, key: "AD", country_code: "+376" },
        { title: ASC.Resources.Countries.Angola, key: "AO", country_code: "+244" },
        { title: ASC.Resources.Countries.Anguilla, key: "AI", country_code: "+1264" },
        { title: ASC.Resources.Countries.AntiguaAndBarbuda, key: "AG", country_code: "+1268" },
        { title: ASC.Resources.Countries.Argentina, key: "AR", country_code: "+54" },
        { title: ASC.Resources.Countries.Armenia, key: "AM", country_code: "+374" },
        { title: ASC.Resources.Countries.Aruba, key: "AW", country_code: "+297" },
        { title: ASC.Resources.Countries.AscensionIsland, key: "AC", country_code: "+247" },
        { title: ASC.Resources.Countries.Australia, key: "AU", country_code: "+61" },
        { title: ASC.Resources.Countries.Austria, key: "AT", country_code: "+43", vat: true },
        { title: ASC.Resources.Countries.Azerbaijan, key: "AZ", country_code: "+994" },
        { title: ASC.Resources.Countries.Bahamas, key: "BS", country_code: "+1242" },
        { title: ASC.Resources.Countries.Bahrain, key: "BH", country_code: "+973" },
        { title: ASC.Resources.Countries.Bangladesh, key: "BD", country_code: "+880" },
        { title: ASC.Resources.Countries.Barbados, key: "BB", country_code: "+1246" },
        { title: ASC.Resources.Countries.Belarus, key: "BY", country_code: "+375" },
        { title: ASC.Resources.Countries.Belgium, key: "BE", country_code: "+32", vat: true },
        { title: ASC.Resources.Countries.Belize, key: "BZ", country_code: "+501" },
        { title: ASC.Resources.Countries.Benin, key: "BJ", country_code: "+229" },
        { title: ASC.Resources.Countries.Bermuda, key: "BM", country_code: "+1441" },
        { title: ASC.Resources.Countries.Bhutan, key: "BT", country_code: "+975" },
        { title: ASC.Resources.Countries.Bolivia, key: "BO", country_code: "+591" },
        { title: ASC.Resources.Countries.BonaireSintEustatiusAndSaba, key: "BQ", country_code: "+599" },
        { title: ASC.Resources.Countries.BosniaAndHerzegovina, key: "BA", country_code: "+387" },
        { title: ASC.Resources.Countries.Botswana, key: "BW", country_code: "+267" },
        { title: ASC.Resources.Countries.Brazil, key: "BR", country_code: "+55" },
        { title: ASC.Resources.Countries.BritishIndianOceanTerritory, key: "IO", country_code: "+246" },
        { title: ASC.Resources.Countries.BritishVirginIslands, key: "VG", country_code: "+1284" },
        { title: ASC.Resources.Countries.BruneiDarussalam, key: "BN", country_code: "+673" },
        { title: ASC.Resources.Countries.Bulgaria, key: "BG", country_code: "+359", vat: true },
        { title: ASC.Resources.Countries.BurkinaFaso, key: "BF", country_code: "+226" },
        { title: ASC.Resources.Countries.Burundi, key: "BI", country_code: "+257" },
        { title: ASC.Resources.Countries.Cambodia, key: "KH", country_code: "+855" },
        { title: ASC.Resources.Countries.Cameroon, key: "CM", country_code: "+237" },
        { title: ASC.Resources.Countries.Canada, key: "CA", country_code: "+1" },
        { title: ASC.Resources.Countries.CapeVerde, key: "CV", country_code: "+238" },
        { title: ASC.Resources.Countries.CaymanIslands, key: "KY", country_code: "+1345" },
        { title: ASC.Resources.Countries.CentralAfricanRepublic, key: "CF", country_code: "+236" },
        { title: ASC.Resources.Countries.Chad, key: "TD", country_code: "+235" },
        { title: ASC.Resources.Countries.Chile, key: "CL", country_code: "+56" },
        { title: ASC.Resources.Countries.China, key: "CN", country_code: "+86" },
        { title: ASC.Resources.Countries.Colombia, key: "CO", country_code: "+57" },
        { title: ASC.Resources.Countries.Comoros, key: "KM", country_code: "+269" },
        { title: ASC.Resources.Countries.CongoBrazzaville, key: "CG", country_code: "+242" },
        { title: ASC.Resources.Countries.CookIslands, key: "CK", country_code: "+682" },
        { title: ASC.Resources.Countries.CostaRica, key: "CR", country_code: "+506" },
        { title: ASC.Resources.Countries.Croatia, key: "HR", country_code: "+385", vat: true },
        { title: ASC.Resources.Countries.Cuba, key: "CU", country_code: "+53" },
        { title: ASC.Resources.Countries.Curacao, key: "CW", country_code: "+599" },
        { title: ASC.Resources.Countries.Cyprus, key: "CY", country_code: "+357", vat: true },
        { title: ASC.Resources.Countries.CzechRepublic, key: "CZ", country_code: "+420", vat: true },
        { title: ASC.Resources.Countries.Denmark, key: "DK", country_code: "+45", vat: true },
        { title: ASC.Resources.Countries.Djibouti, key: "DJ", country_code: "+253" },
        { title: ASC.Resources.Countries.Dominica, key: "DM", country_code: "+1767" },
        { title: ASC.Resources.Countries.DominicanRepublic, key: "DO", country_code: "+1809" },
        { title: ASC.Resources.Countries.Ecuador, key: "EC", country_code: "+593" },
        { title: ASC.Resources.Countries.Egypt, key: "EG", country_code: "+20" },
        { title: ASC.Resources.Countries.ElSalvador, key: "SV", country_code: "+503" },
        { title: ASC.Resources.Countries.EquatorialGuinea, key: "GQ", country_code: "+240" },
        { title: ASC.Resources.Countries.Eritrea, key: "ER", country_code: "+291" },
        { title: ASC.Resources.Countries.Estonia, key: "EE", country_code: "+372", vat: true },
        { title: ASC.Resources.Countries.Ethiopia, key: "ET", country_code: "+251" },
        { title: ASC.Resources.Countries.FaroeIslands, key: "FO", country_code: "+298" },
        { title: ASC.Resources.Countries.Fiji, key: "FJ", country_code: "+679" },
        { title: ASC.Resources.Countries.Finland, key: "FI", country_code: "+358", vat: true },
        { title: ASC.Resources.Countries.France, key: "FR", country_code: "+33", vat: true },
        { title: ASC.Resources.Countries.FrenchGuiana, key: "GF", country_code: "+594" },
        { title: ASC.Resources.Countries.FrenchPolynesia, key: "PF", country_code: "+689" },
        { title: ASC.Resources.Countries.Gabon, key: "GA", country_code: "+241" },
        { title: ASC.Resources.Countries.Gambia, key: "GM", country_code: "+220" },
        { title: ASC.Resources.Countries.Georgia, key: "GE", country_code: "+995" },
        { title: ASC.Resources.Countries.Germany, key: "DE", country_code: "+49", vat: true },
        { title: ASC.Resources.Countries.Ghana, key: "GH", country_code: "+233" },
        { title: ASC.Resources.Countries.Gibraltar, key: "GI", country_code: "+350" },
        { title: ASC.Resources.Countries.Greece, key: "GR", country_code: "+30", vat: true },
        { title: ASC.Resources.Countries.Greenland, key: "GL", country_code: "+299" },
        { title: ASC.Resources.Countries.Grenada, key: "GD", country_code: "+1473" },
        { title: ASC.Resources.Countries.Guadeloupe, key: "GP", country_code: "+590" },
        { title: ASC.Resources.Countries.Guam, key: "GU", country_code: "+1671" },
        { title: ASC.Resources.Countries.Guatemala, key: "GT", country_code: "+502" },
        { title: ASC.Resources.Countries.Guinea, key: "GN", country_code: "+224" },
        { title: ASC.Resources.Countries.GuineaBissau, key: "GW", country_code: "+245" },
        { title: ASC.Resources.Countries.Guyana, key: "GY", country_code: "+592" },
        { title: ASC.Resources.Countries.Haiti, key: "HT", country_code: "+509" },
        { title: ASC.Resources.Countries.Honduras, key: "HN", country_code: "+504" },
        { title: ASC.Resources.Countries.HongKong, key: "HK", country_code: "+852" },
        { title: ASC.Resources.Countries.Hungary, key: "HU", country_code: "+36", vat: true },
        { title: ASC.Resources.Countries.Iceland, key: "IS", country_code: "+354" },
        { title: ASC.Resources.Countries.India, key: "IN", country_code: "+91" },
        { title: ASC.Resources.Countries.Indonesia, key: "ID", country_code: "+62" },
        { title: ASC.Resources.Countries.Iran, key: "IR", country_code: "+98" },
        { title: ASC.Resources.Countries.Iraq, key: "IQ", country_code: "+964" },
        { title: ASC.Resources.Countries.Ireland, key: "IE", country_code: "+353", vat: true },
        { title: ASC.Resources.Countries.Israel, key: "IL", country_code: "+972" },
        { title: ASC.Resources.Countries.Italy, key: "IT", country_code: "+39", vat: true },
        { title: ASC.Resources.Countries.IvoryCoast, key: "CI", country_code: "+225" },
        { title: ASC.Resources.Countries.Jamaica, key: "JM", country_code: "+1876" },
        { title: ASC.Resources.Countries.Japan, key: "JP", country_code: "+81" },
        { title: ASC.Resources.Countries.Jordan, key: "JO", country_code: "+962" },
        { title: ASC.Resources.Countries.Kazakhstan, key: "KZ", country_code: "+7" },
        { title: ASC.Resources.Countries.Kenya, key: "KE", country_code: "+254" },
        { title: ASC.Resources.Countries.Kiribati, key: "KI", country_code: "+686" },
        { title: ASC.Resources.Countries.Kuwait, key: "KW", country_code: "+965" },
        { title: ASC.Resources.Countries.Kyrgyzstan, key: "KG", country_code: "+996" },
        { title: ASC.Resources.Countries.Laos, key: "LA", country_code: "+856" },
        { title: ASC.Resources.Countries.Latvia, key: "LV", country_code: "+371", vat: true },
        { title: ASC.Resources.Countries.Lebanon, key: "LB", country_code: "+961" },
        { title: ASC.Resources.Countries.Lesotho, key: "LS", country_code: "+266" },
        { title: ASC.Resources.Countries.Liberia, key: "LR", country_code: "+231" },
        { title: ASC.Resources.Countries.Libya, key: "LY", country_code: "+218" },
        { title: ASC.Resources.Countries.Liechtenstein, key: "LI", country_code: "+423" },
        { title: ASC.Resources.Countries.Lithuania, key: "LT", country_code: "+370", vat: true },
        { title: ASC.Resources.Countries.Luxembourg, key: "LU", country_code: "+352", vat: true },
        { title: ASC.Resources.Countries.Macau, key: "MO", country_code: "+853" },
        { title: ASC.Resources.Countries.Macedonia, key: "MK", country_code: "+389" },
        { title: ASC.Resources.Countries.Madagascar, key: "MG", country_code: "+261" },
        { title: ASC.Resources.Countries.Malawi, key: "MW", country_code: "+265" },
        { title: ASC.Resources.Countries.Malaysia, key: "MY", country_code: "+60" },
        { title: ASC.Resources.Countries.Maldives, key: "MV", country_code: "+960" },
        { title: ASC.Resources.Countries.Mali, key: "ML", country_code: "+223" },
        { title: ASC.Resources.Countries.Malta, key: "MT", country_code: "+356", vat: true },
        { title: ASC.Resources.Countries.Malvinas, key: "FK", country_code: "+500" },
        { title: ASC.Resources.Countries.MarshallIslands, key: "MH", country_code: "+692" },
        { title: ASC.Resources.Countries.Martinique, key: "MQ", country_code: "+596" },
        { title: ASC.Resources.Countries.Mauritania, key: "MR", country_code: "+222" },
        { title: ASC.Resources.Countries.Mauritius, key: "MU", country_code: "+230" },
        { title: ASC.Resources.Countries.Mexico, key: "MX", country_code: "+52" },
        { title: ASC.Resources.Countries.Micronesia, key: "FM", country_code: "+691" },
        { title: ASC.Resources.Countries.Moldova, key: "MD", country_code: "+373" },
        { title: ASC.Resources.Countries.Monaco, key: "MC", country_code: "+377" },
        { title: ASC.Resources.Countries.Mongolia, key: "MN", country_code: "+976" },
        { title: ASC.Resources.Countries.Montenegro, key: "ME", country_code: "+382" },
        { title: ASC.Resources.Countries.Montserrat, key: "MS", country_code: "+1664" },
        { title: ASC.Resources.Countries.Morocco, key: "MA", country_code: "+212" },
        { title: ASC.Resources.Countries.Mozambique, key: "MZ", country_code: "+258" },
        { title: ASC.Resources.Countries.Myanmar, key: "MM", country_code: "+95" },
        { title: ASC.Resources.Countries.Namibia, key: "NA", country_code: "+264" },
        { title: ASC.Resources.Countries.Nauru, key: "NR", country_code: "+674" },
        { title: ASC.Resources.Countries.Nepal, key: "NP", country_code: "+977" },
        { title: ASC.Resources.Countries.Netherlands, key: "NL", country_code: "+31", vat: true },
        { title: ASC.Resources.Countries.NewCaledonia, key: "NC", country_code: "+687" },
        { title: ASC.Resources.Countries.NewZealand, key: "NZ", country_code: "+64" },
        { title: ASC.Resources.Countries.Nicaragua, key: "NI", country_code: "+505" },
        { title: ASC.Resources.Countries.Niger, key: "NE", country_code: "+227" },
        { title: ASC.Resources.Countries.Nigeria, key: "NG", country_code: "+234" },
        { title: ASC.Resources.Countries.Niue, key: "NU", country_code: "+683" },
        { title: ASC.Resources.Countries.NorfolkIsland, key: "NF", country_code: "+6723" },
        { title: ASC.Resources.Countries.NorthernMarianaIslands, key: "KP", country_code: "+1" },
        { title: ASC.Resources.Countries.NorthKorea, key: "MP", country_code: "+850" },
        { title: ASC.Resources.Countries.Norway, key: "NO", country_code: "+47" },
        { title: ASC.Resources.Countries.Oman, key: "OM", country_code: "+968" },
        { title: ASC.Resources.Countries.Pakistan, key: "PK", country_code: "+92" },
        { title: ASC.Resources.Countries.Palau, key: "PW", country_code: "+680" },
        { title: ASC.Resources.Countries.Palestine, key: "PS", country_code: "+970" },
        { title: ASC.Resources.Countries.Panama, key: "PA", country_code: "+507" },
        { title: ASC.Resources.Countries.PapuaNewGuinea, key: "PG", country_code: "+675" },
        { title: ASC.Resources.Countries.Paraguay, key: "PY", country_code: "+595" },
        { title: ASC.Resources.Countries.Peru, key: "PE", country_code: "+51" },
        { title: ASC.Resources.Countries.Philippines, key: "PH", country_code: "+63" },
        { title: ASC.Resources.Countries.Poland, key: "PL", country_code: "+48", vat: true },
        { title: ASC.Resources.Countries.Portugal, key: "PT", country_code: "+351", vat: true },
        { title: ASC.Resources.Countries.PuertoRico, key: "PR", country_code: "+1787" },
        { title: ASC.Resources.Countries.Qatar, key: "QA", country_code: "+974" },
        { title: ASC.Resources.Countries.RepublicOfKorea, key: "KR", country_code: "+82" },
        { title: ASC.Resources.Countries.Reunion, key: "RE", country_code: "+262" },
        { title: ASC.Resources.Countries.Romania, key: "RO", country_code: "+40", vat: true },
        { title: ASC.Resources.Countries.Russia, key: "RU", country_code: "+7" },
        { title: ASC.Resources.Countries.Rwanda, key: "RW", country_code: "+250" },
        { title: ASC.Resources.Countries.SaintBarthelemy, key: "BL", country_code: "+590" },
        { title: ASC.Resources.Countries.SaintHelena, key: "SH", country_code: "+290" },
        { title: ASC.Resources.Countries.SaintKittsAndNevis, key: "KN", country_code: "+1869" },
        { title: ASC.Resources.Countries.SaintLucia, key: "LC", country_code: "+1758" },
        { title: ASC.Resources.Countries.SaintMartinIsland, key: "MF", country_code: "+590" },
        { title: ASC.Resources.Countries.SaintPierreAndMiquelon, key: "PM", country_code: "+508" },
        { title: ASC.Resources.Countries.SaintVincentAndTheGrenadines, key: "VC", country_code: "+1784" },
        { title: ASC.Resources.Countries.Samoa, key: "WS", country_code: "+685" },
        { title: ASC.Resources.Countries.SanMarino, key: "SM", country_code: "+378" },
        { title: ASC.Resources.Countries.SaoTomeAndPrincipe, key: "ST", country_code: "+239" },
        { title: ASC.Resources.Countries.SaudiArabia, key: "SA", country_code: "+966" },
        { title: ASC.Resources.Countries.Senegal, key: "SN", country_code: "+221" },
        { title: ASC.Resources.Countries.Serbia, key: "RS", country_code: "+381" },
        { title: ASC.Resources.Countries.Seychelles, key: "SC", country_code: "+248" },
        { title: ASC.Resources.Countries.SierraLeone, key: "SL", country_code: "+232" },
        { title: ASC.Resources.Countries.Singapore, key: "SG", country_code: "+65" },
        { title: ASC.Resources.Countries.SintMaarten, key: "SX", country_code: "+1721" },
        { title: ASC.Resources.Countries.Slovakia, key: "SK", country_code: "+421", vat: true },
        { title: ASC.Resources.Countries.Slovenia, key: "SI", country_code: "+386", vat: true },
        { title: ASC.Resources.Countries.SolomonIslands, key: "SB", country_code: "+677" },
        { title: ASC.Resources.Countries.Somalia, key: "SO", country_code: "+252" },
        { title: ASC.Resources.Countries.SouthAfrica, key: "ZA", country_code: "+27" },
        { title: ASC.Resources.Countries.SouthSudan, key: "SS", country_code: "+211" },
        { title: ASC.Resources.Countries.Spain, key: "ES", country_code: "+34", vat: true },
        { title: ASC.Resources.Countries.SriLanka, key: "LK", country_code: "+94" },
        { title: ASC.Resources.Countries.Sudan, key: "SD", country_code: "+249" },
        { title: ASC.Resources.Countries.Suriname, key: "SR", country_code: "+597" },
        { title: ASC.Resources.Countries.Swaziland, key: "SZ", country_code: "+268" },
        { title: ASC.Resources.Countries.Sweden, key: "SE", country_code: "+46", vat: true },
        { title: ASC.Resources.Countries.Switzerland, key: "CH", country_code: "+41" },
        { title: ASC.Resources.Countries.Syria, key: "SY", country_code: "+963" },
        { title: ASC.Resources.Countries.Taiwan, key: "TW", country_code: "+886" },
        { title: ASC.Resources.Countries.Tajikistan, key: "TJ", country_code: "+992" },
        { title: ASC.Resources.Countries.Tanzania, key: "TZ", country_code: "+255" },
        { title: ASC.Resources.Countries.Thailand, key: "TH", country_code: "+66" },
        { title: ASC.Resources.Countries.TheDemocraticRepublicOfTheCongo, key: "CD", country_code: "+243" },
        { title: ASC.Resources.Countries.TimorLeste, key: "TL", country_code: "+670" },
        { title: ASC.Resources.Countries.Togo, key: "TG", country_code: "+228" },
        { title: ASC.Resources.Countries.Tokelau, key: "TK", country_code: "+690" },
        { title: ASC.Resources.Countries.Tonga, key: "TO", country_code: "+676" },
        { title: ASC.Resources.Countries.TrinidadAndTobago, key: "TT", country_code: "+1868" },
        { title: ASC.Resources.Countries.Tunisia, key: "TN", country_code: "+216" },
        { title: ASC.Resources.Countries.Turkey, key: "TR", country_code: "+90" },
        { title: ASC.Resources.Countries.Turkmenistan, key: "TM", country_code: "+993" },
        { title: ASC.Resources.Countries.TurksAndCaicosIslands, key: "TC", country_code: "+1649" },
        { title: ASC.Resources.Countries.Tuvalu, key: "TV", country_code: "+688" },
        { title: ASC.Resources.Countries.UK, key: "GB", country_code: "+44", vat: true },
        { title: ASC.Resources.Countries.USVirginIslands, key: "VI", country_code: "+1340" },
        { title: ASC.Resources.Countries.Uganda, key: "UG", country_code: "+256" },
        { title: ASC.Resources.Countries.Ukraine, key: "UA", country_code: "+380" },
        { title: ASC.Resources.Countries.UnitedArabEmirates, key: "AE", country_code: "+971" },
        { title: ASC.Resources.Countries.UnitedStates, key: "US", country_code: "+1" },
        { title: ASC.Resources.Countries.Uruguay, key: "UY", country_code: "+598" },
        { title: ASC.Resources.Countries.Uzbekistan, key: "UZ", country_code: "+998" },
        { title: ASC.Resources.Countries.Vanuatu, key: "VU", country_code: "+678" },
        { title: ASC.Resources.Countries.VaticanCity, key: "VA", country_code: "+379" },
        { title: ASC.Resources.Countries.Venezuela, key: "VE", country_code: "+58" },
        { title: ASC.Resources.Countries.Vietnam, key: "VN", country_code: "+84" },
        { title: ASC.Resources.Countries.WallisAndFutuna, key: "WF", country_code: "+681" },
        { title: ASC.Resources.Countries.Yemen, key: "YE", country_code: "+967" },
        { title: ASC.Resources.Countries.Zambia, key: "ZM", country_code: "+260" },
        { title: ASC.Resources.Countries.Zimbabwe, key: "ZW", country_code: "+263" }
    ];

    var vm = {
        countriesList: countriesList
    };

    return vm;
}
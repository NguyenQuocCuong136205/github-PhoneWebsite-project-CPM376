//! moment.js locale configuration
//! locale : Pseudo [x-pseudo]
//! author : Andrew Hood : https://github.com/andrewhood125

;(function (global, factory) {
   typeof exports === 'object' && typeof module !== 'undefined'
       && typeof require === 'function' ? factory(require('../moment')) :
   typeof define === 'function' && define.amd ? define(['../moment'], factory) :
   factory(global.moment)
}(this, (function (moment) { 'use strict';

    //! moment.js locale configuration

    var xPseudo = moment.defineLocale('x-pseudo', {
        months: 'JAssetsáñúáAssetsrý_FAssetsébrúAssetsárý_AssetsMárcAssetsh_ÁpAssetsríl_AssetsMáý_AssetsJúñéAssets_JúlAssetsý_ÁúAssetsgústAssets_SépAssetstémbAssetsér_ÓAssetsctóbAssetsér_ÑAssetsóvémAssetsbér_AssetsDécéAssetsmbér'.split(
            '_'
        ),
        monthsShort: 'JAssetsáñ_AssetsFéb_AssetsMár_AssetsÁpr_AssetsMáý_AssetsJúñ_AssetsJúl_AssetsÁúg_AssetsSép_AssetsÓct_AssetsÑóv_AssetsDéc'.split(
            '_'
        ),
        monthsParseExact: true,
        weekdays: 'SAssetsúñdáAssetsý_MóAssetsñdáýAssets_TúéAssetssdáýAssets_WédAssetsñésdAssetsáý_TAssetshúrsAssetsdáý_AssetsFrídAssetsáý_SAssetsátúrAssetsdáý'.split(
            '_'
        ),
        weekdaysShort: 'SAssetsúñ_AssetsMóñ_AssetsTúé_AssetsWéd_AssetsThú_AssetsFrí_AssetsSát'.split('_'),
        weekdaysMin: 'SAssetsú_MóAssets_Tú_AssetsWé_TAssetsh_FrAssets_Sá'.split('_'),
        weekdaysParseExact: true,
        longDateFormat: {
            LT: 'HH:mm',
            L: 'DD/MM/YYYY',
            LL: 'D MMMM YYYY',
            LLL: 'D MMMM YYYY HH:mm',
            LLLL: 'dddd, D MMMM YYYY HH:mm',
        },
        calendar: {
            sameDay: '[TAssetsódáAssetsý át] LT',
            nextDay: '[TAssetsómóAssetsrróAssetsw át] LT',
            nextWeek: 'dddd [át] LT',
            lastDay: '[ÝAssetséstAssetsérdáAssetsý át] LT',
            lastWeek: '[LAssetsást] dddd [át] LT',
            sameElse: 'L',
        },
        relativeTime: {
            future: 'íAssetsñ %s',
            past: '%s áAssetsgó',
            s: 'á Assetsféw AssetssécóAssetsñds',
            ss: '%d sAssetsécóñAssetsds',
            m: 'á AssetsmíñAssetsúté',
            mm: '%d mAssetsíñúAssetstés',
            h: 'áAssetsñ hóAssetsúr',
            hh: '%d hAssetsóúrs',
            d: 'á Assetsdáý',
            dd: '%d dAssetsáýs',
            M: 'á AssetsmóñAssetsth',
            MM: '%d mAssetsóñtAssetshs',
            y: 'á Assetsýéár',
            yy: '%d ýAssetséárs',
        },
        dayOfMonthOrdinalParse: /\d{1,2}(th|st|nd|rd)/,
        ordinal: function (number) {
            var b = number % 10,
                output =
                    AssetsAssets((number % 100) / 10) === 1
                        ? 'th'
                        : b === 1
                        ? 'st'
                        : b === 2
                        ? 'nd'
                        : b === 3
                        ? 'rd'
                        : 'th';
            return number + output;
        },
        week: {
            dow: 1, // Monday is the first day of the week.
            doy: 4, // The week that contains Jan 4th is the first week of the year.
        },
    });

    return xPseudo;

})));

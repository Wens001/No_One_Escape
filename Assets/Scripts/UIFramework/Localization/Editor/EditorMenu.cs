// Copyright (c) H. Ibrahim Penekli. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEditor;

namespace GameToolkit.Localization.Editor
{
    /// <summary>
    /// Unity Editor menu for changing localization under "Tools/Localization".
    /// </summary>
    public static class EditorMenu
    {
        public const string RootMenu = "MyTools/���ػ�/";
        
        private const string SetLocaleRootMenu = RootMenu + "��������/";

        [MenuItem(RootMenu + "���� .csv", false, 1)]
        private static void Import()
        {
            EditorSerialization.Import();
        }
        
        [MenuItem(RootMenu + "���� .csv", false, 2)]
        private static void Export()
        {
            EditorSerialization.Export();
        }
        
        [MenuItem(RootMenu + "����", false, 3)]
        private static void OpenHelpUrl()
        {
            Application.OpenURL(Localization.HelpUrl);
        }

        [MenuItem(SetLocaleRootMenu + "�ϷǺ�����")]
        private static void ChangeToAfrikaans()
        {
            SetLanguage(Language.Afrikaans);
        }

        [MenuItem(SetLocaleRootMenu + "��������")]
        private static void ChangeToArabic()
        {
            SetLanguage(Language.Arabic);
        }

        [MenuItem(SetLocaleRootMenu + "��˹����")]
        private static void ChangeToBasque()
        {
            SetLanguage(Language.Basque);
        }

        [MenuItem(SetLocaleRootMenu + "�׶���˹��")]
        private static void ChangeToBelarusian()
        {
            SetLanguage(Language.Belarusian);
        }

        [MenuItem(SetLocaleRootMenu + "����������")]
        private static void ChangeToBulgarian()
        {
            SetLanguage(Language.Bulgarian);
        }

        [MenuItem(SetLocaleRootMenu + "��̩��������")]
        private static void ChangeToCatalan()
        {
            SetLanguage(Language.Catalan);
        }

        [MenuItem(SetLocaleRootMenu + "����")]
        private static void ChangeToChinese()
        {
            SetLanguage(Language.Chinese);
        }

        [MenuItem(SetLocaleRootMenu + "�ݿ���")]
        private static void ChangeToCzech()
        {
            SetLanguage(Language.Czech);
        }

        [MenuItem(SetLocaleRootMenu + "������")]
        private static void ChangeToDanish()
        {
            SetLanguage(Language.Danish);
        }

        [MenuItem(SetLocaleRootMenu + "������")]
        private static void ChangeToDutch()
        {
            SetLanguage(Language.Dutch);
        }

        [MenuItem(SetLocaleRootMenu + "Ӣ��")]
        private static void ChangeToEnglish()
        {
            SetLanguage(Language.English);
        }

        [MenuItem(SetLocaleRootMenu + "��ɳ������")]
        private static void ChangeToEstonian()
        {
            SetLanguage(Language.Estonian);
        }

        [MenuItem(SetLocaleRootMenu + "������")]
        private static void ChangeToFaroese()
        {
            SetLanguage(Language.Faroese);
        }

        [MenuItem(SetLocaleRootMenu + "������")]
        private static void ChangeToFinnish()
        {
            SetLanguage(Language.Finnish);
        }

        [MenuItem(SetLocaleRootMenu + "����")]
        private static void ChangeToFrench()
        {
            SetLanguage(Language.French);
        }

        [MenuItem(SetLocaleRootMenu + "����")]
        private static void ChangeToGerman()
        {
            SetLanguage(Language.German);
        }

        [MenuItem(SetLocaleRootMenu + "ϣ����")]
        private static void ChangeToGreek()
        {
            SetLanguage(Language.Greek);
        }

        [MenuItem(SetLocaleRootMenu + "ϣ������")]
        private static void ChangeToHebrew()
        {
            SetLanguage(Language.Hebrew);
        }


        [MenuItem(SetLocaleRootMenu + "��������")]
        private static void ChangeToHugarian()
        {
            SetLanguage(Language.Hungarian);
        }

        [MenuItem(SetLocaleRootMenu + "������")]
        private static void ChangeToIcelandic()
        {
            SetLanguage(Language.Icelandic);
        }

        [MenuItem(SetLocaleRootMenu + "ӡ����������")]
        private static void ChangeToIndonesian()
        {
            SetLanguage(Language.Indonesian);
        }

        [MenuItem(SetLocaleRootMenu + "�������")]
        private static void ChangeToItalian()
        {
            SetLanguage(Language.Italian);
        }

        [MenuItem(SetLocaleRootMenu + "����")]
        private static void ChangeToJapanese()
        {
            SetLanguage(Language.Japanese);
        }

        [MenuItem(SetLocaleRootMenu + "����")]
        private static void ChangeToKorean()
        {
            SetLanguage(Language.Korean);
        }

        [MenuItem(SetLocaleRootMenu + "����ά����")]
        private static void ChangeToLatvian()
        {
            SetLanguage(Language.Latvian);
        }

        [MenuItem(SetLocaleRootMenu + "��������")]
        private static void ChangeToLithuanian()
        {
            SetLanguage(Language.Lithuanian);
        }

        [MenuItem(SetLocaleRootMenu + "Ų����")]
        private static void ChangeToNorwegian()
        {
            SetLanguage(Language.Norwegian);
        }

        [MenuItem(SetLocaleRootMenu + "������")]
        private static void ChangeToPolish()
        {
            SetLanguage(Language.Polish);
        }

        [MenuItem(SetLocaleRootMenu + "��������")]
        private static void ChangeToPortuguese()
        {
            SetLanguage(Language.Portuguese);
        }

        [MenuItem(SetLocaleRootMenu + "����������")]
        private static void ChangeToRomanian()
        {
            SetLanguage(Language.Romanian);
        }

        [MenuItem(SetLocaleRootMenu + "����")]
        private static void ChangeToRussian()
        {
            SetLanguage(Language.Russian);
        }

        [MenuItem(SetLocaleRootMenu + "����ά����")]
        private static void ChangeToSerboCroatian()
        {
            SetLanguage(Language.SerboCroatian);
        }

        [MenuItem(SetLocaleRootMenu + "˹�工����")]
        private static void ChangeToSlovak()
        {
            SetLanguage(Language.Slovak);
        }

        [MenuItem(SetLocaleRootMenu + "˹����������")]
        private static void ChangeToSlovenian()
        {
            SetLanguage(Language.Slovenian);
        }

        [MenuItem(SetLocaleRootMenu + "��������")]
        private static void ChangeToSpanish()
        {
            SetLanguage(Language.Spanish);
        }

        [MenuItem(SetLocaleRootMenu + "�����")]
        private static void ChangeToSwedish()
        {
            SetLanguage(Language.Swedish);
        }

        [MenuItem(SetLocaleRootMenu + "̩��")]
        private static void ChangeToThai()
        {
            SetLanguage(Language.Thai);
        }

        [MenuItem(SetLocaleRootMenu + "��������")]
        private static void ChangeToTurkish()
        {
            SetLanguage(Language.Turkish);
        }

        [MenuItem(SetLocaleRootMenu + "�ڿ�����")]
        private static void ChangeToUkrainian()
        {
            SetLanguage(Language.Ukrainian);
        }

        [MenuItem(SetLocaleRootMenu + "Խ����")]
        private static void ChangeToVietnamese()
        {
            SetLanguage(Language.Vietnamese);
        }

        [MenuItem(SetLocaleRootMenu + "��������")]
        private static void ChangeToChineseSimplified()
        {
            SetLanguage(Language.ChineseSimplified);
        }

        [MenuItem(SetLocaleRootMenu + "��������")]
        private static void ChangeToChineseTraditional()
        {
            SetLanguage(Language.ChineseTraditional);
        }

        [MenuItem(SetLocaleRootMenu + "Unknown")]
        private static void ChangeToUnknown()
        {
            SetLanguage(Language.Unknown);
        }

        private static void SetLanguage(Language currentLanguage)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("�������Խ���Ӧ�ó������ڲ���ʱ����.");
                return;
            }
            
            var previousLanguage = Localization.Instance.CurrentLanguage;
            Localization.Instance.CurrentLanguage = currentLanguage;
            
            Menu.SetChecked(GetMenuName(previousLanguage), false);
            Menu.SetChecked(GetMenuName(currentLanguage), true);
        }

        private static string GetMenuName(Language language)
        {
            return SetLocaleRootMenu + language;
        }
    }
}

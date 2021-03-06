﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace ParaSmeller.Vsix
{
    [Guid("43991993-21b1-4585-99d9-0ab9c65a8411")]
    public class ParaSmellerSettings : DialogPage
    {
        public List<Smell> Smells => Enum.GetValues(typeof(Smell)).Cast<Smell>().ToList();

        public string SelectedSmellsStr
        {
            get { return Convert(SelectedSmells); }
            set { SelectedSmells = Convert(value); }
        }
        
        public List<string> SelectedSmells { get; set; }

        public int MaxDepthAsync { get; set; } = 3;

        protected override IWin32Window Window
        {
            get
            {
                var page = new ParaSmellerSettingsUi {OptionsPage = this};
                page.Initialize();
                return page;
            }
        }

        private static string Convert(List<string> list)
        {
            if (list == null) return string.Empty;
            return string.Join(";", list);
        }

        private static List<string> Convert(string str)
        {
            if (str == null) return new List<string>();
            return str.Split(';').ToList();
        }
    }
}

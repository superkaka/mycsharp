using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KLib.I18N
{
    public interface I18NLanguagePack
    {

        string LangId { get; }

        string GetText(string textId);

        void AddText(string textId, string text);

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extension
{
    [ComplexType]
    public class LocalizedString
    {
        public string Arabic { get; set; }
        public string English { get; set; }
        public override string ToString()
        {
            return English;
        }
        public LocalizedString(string arabic, string english)
        {
            Arabic = arabic;
            English = english;
        }
        public string CurrentUICulture()
        {
            return System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToUpper();
        }

        [NotMapped]
        public string Current
        {
            get
            {
                switch (CurrentUICulture())
                {
                    case "AR":
                        return Arabic;
                    case "EN":
                        return English;
                }
                return ToString();
            }

            set
            {
                switch (CurrentUICulture())
                {
                    case "AR":
                        Arabic = value;
                        break;
                    case "EN":
                        English = value;
                        break;
                }

            }
        }
    }
}

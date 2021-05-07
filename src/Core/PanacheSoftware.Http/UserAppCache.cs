using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Http
{
    public class UserAppCache
    {
        public string LanguageCode { get; private set; }

        public event Action OnChange;

        public void SetLanguageCode(string languageCode)
        {
            LanguageCode = languageCode;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}

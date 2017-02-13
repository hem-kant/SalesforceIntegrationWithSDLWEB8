using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreServiceWithWeb8.DAL.Common
{
    public class SetPublication
    {
        #region Set Publication
        public static string Publication(string pTcmUri, string cTcmUri)
        {
            string folderpublication = pTcmUri.Substring(pTcmUri.IndexOf("tcm:"), pTcmUri.IndexOf("-"));
            string schemapublication = cTcmUri.Substring(cTcmUri.IndexOf("tcm:"), cTcmUri.IndexOf("-"));
            return cTcmUri.Replace(schemapublication, folderpublication);
        }
        #endregion
    }
}

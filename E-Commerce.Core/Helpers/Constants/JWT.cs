using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Helpers.Constants
{
    public class JWT
    {
        private static JWT _jwt;
        public static JWT GetInstance()
        {
            if (_jwt == null )
                _jwt = new JWT();
            return _jwt;
        }
        public string Key { get; set; } = "nsQdSPWNC0IVTfmIOW69RP8ge58EnUycQ97bZTnUV3g=";
        public string Issuer { get; set; } = "SecureApi";
        public string Audience { get; set; } = "SecureApisUser";
        public double DurationInMinutes { get; set; } = 1;
    }
}

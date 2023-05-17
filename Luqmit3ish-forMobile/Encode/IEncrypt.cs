using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Luqmit3ish_forMobile.Encode
{
    public interface IEncrypt
    {
        public string EncryptPassword(string password);
        public bool VerifyPassword(string password, string savedPasswordHash);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Structures
{
    public struct CipherData
    {
        public string FullFileName { get; }
        public string FullPassword { get; }
        public bool WritePasswordHash { get; }

        public CipherData(string fullFileName, string fullPassword, bool writePasswordHash = false)
        {
            FullFileName = fullFileName;
            FullPassword = fullPassword;
            WritePasswordHash = writePasswordHash;
        }
    }
}

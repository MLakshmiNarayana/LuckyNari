using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Configuration;

namespace Foodicsapi
{
    class clsReg
    {
        //private string subKey = ConfigurationSettings.AppSettings["SSDirectory"];
        private string subKey = ".DEFAULT\\Software\\Focus7";
        public string SubKey
        {
            get { return subKey; }
            set { subKey = value; }
        }

        private RegistryKey baseRegistryKey = Registry.Users;

        public RegistryKey BaseRegistryKey
        {
            get { return baseRegistryKey; }
            set { baseRegistryKey = value; }
        }

        public string Read(string KeyName)
        {
            RegistryKey rk = baseRegistryKey;
            string[] strT = rk.GetSubKeyNames();
            //string cmpcode = sSDirFilePath;

            RegistryKey sk1 = rk.OpenSubKey(subKey);
            if (sk1 == null)
            {
                return null;
            }
            else
            {
                try
                {
                    return (string)sk1.GetValue(KeyName.ToUpper());
                }
                catch (Exception e)
                {
                    //ShowErrorMessage(e, "Reading registry " + KeyName.ToUpper());
                    return e.Message;
                }
            }
        }

        public byte[] ReadByte(string KeyName)
        {
            RegistryKey rk = baseRegistryKey;
            string[] strT = rk.GetSubKeyNames();

            RegistryKey sk1 = rk.OpenSubKey(subKey);
            if (sk1 == null)
            {
                return null;
            }
            else
            {
                return (byte[])sk1.GetValue(KeyName.ToUpper());
            }
        }
    }
}

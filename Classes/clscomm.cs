using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Win32;

namespace Foodicsapi.Classess
{
    internal class ClsComm
    {
        public static string SqlConnectionString()
        {
            //Data Source=AHMED-VAIO\SQL2008R2;Initial Catalog=focus50w0;UID=sa;PWD=focus;Integrated Security=false
            var g_CompCode = "";
            g_CompCode = "060";
            try
            {
                g_CompCode = Default_Company();
            }
            catch (Exception e)
            {
                g_CompCode = "1b0";
                //throw;
            }

            //var clsRg = new clsReg();

            //return "Data Source=" + clsRg.Read("SQLServerName") +";Initial Catalog=Focus80x0" +";UID=" + clsRg.Read("SQLLoginID") +";PWD=" + clsRg.Read("SQLPW") +";Integrated Security=false";
            //return "Data Source=" + SQLServerName() + ";Initial Catalog=Focus8" + Default_Company() + ";" + "UId=" + SQLLoginID() + ";" + "Pwd=" + SQLPW() + ";";
            //return "Data Source=" + SQLServerName() + ";Initial Catalog=Focus5" + g_CompCode + ";" + "UId=" + SQLLoginID() + ";" + "Pwd=" + SQLPW() + ";";
            return "Data Source=" + SQLServerName() + ";Initial Catalog=" + g_CompCode + ";" + "UId=" + SQLLoginID() + ";" + "Pwd=" + SQLPW() + ";";// + "Multipleconnection="+ Multipleconnection()+ ";";
        }
        public static string SqlConnectionString2()
        {
            //Data Source=AHMED-VAIO\SQL2008R2;Initial Catalog=focus50w0;UID=sa;PWD=focus;Integrated Security=false
            var g_CompCode = "";
            g_CompCode = "030";
            try
            {
                g_CompCode = Default_Company2();
            }
            catch (Exception e)
            {
                g_CompCode = "1b0";
                //throw;
            }

            //var clsRg = new clsReg();

            //return "Data Source=" + clsRg.Read("SQLServerName") +";Initial Catalog=Focus80x0" +";UID=" + clsRg.Read("SQLLoginID") +";PWD=" + clsRg.Read("SQLPW") +";Integrated Security=false";
            //return "Data Source=" + SQLServerName() + ";Initial Catalog=Focus8" + Default_Company() + ";" + "UId=" + SQLLoginID() + ";" + "Pwd=" + SQLPW() + ";";
            //return "Data Source=" + SQLServerName() + ";Initial Catalog=Focus5" + g_CompCode + ";" + "UId=" + SQLLoginID() + ";" + "Pwd=" + SQLPW() + ";";
            return "Data Source=" + SQLServerName() + ";Initial Catalog=Focus5" + g_CompCode + ";" + "UId=" + SQLLoginID() + ";" + "Pwd=" + SQLPW() + ";";
        }

        public static void Write2ErrLog(string ex, string thevent)
        {
            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "SMLOG",
                string.Format(@"{0}{1:dd/MM/yyyy hh:mm:ss tt}: >> An error from: {2} || msg: {3}", Environment.NewLine, DateTime.Now, thevent, ex));
        }


        public static string GetPutstr_Fld_Val(string putstr, string fieldName, string Concate)
        {
            string fstr = "";
            try
            {
                fstr = putstr.Substring(putstr.IndexOf(fieldName + "="));
                fstr = fstr.Substring(fstr.IndexOf(Concate) + 1);
                fstr = fstr.Substring(0, fstr.IndexOf(","));
            }

            catch (Exception ex)
            {
                fstr = "";
                MessageBox.Show(ex.Message);
            }
            return fstr;
        }

        public static string RegValue(RegistryHive Hive, string Key, string ValueName, string OptionalByRefErrInfo)
        {
            RegistryKey objParent = Registry.Users;
            string sAns = String.Empty;
            try
            {
                RegistryKey objSubkey = objParent.OpenSubKey(Key);
                if (objSubkey != null)
                {
                    return (string)objSubkey.GetValue(ValueName);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex + "Reading registry " + ValueName.ToUpper());
                return null;
            }
            return null;
        }

        public static string SQLServerName()
        {
            return RegValue(Microsoft.Win32.RegistryHive.Users, ".DEFAULT\\Software\\FocusExternalModule", "SQLServerName", "");
        }

        public static string SQLLoginID()
        {
            return RegValue(Microsoft.Win32.RegistryHive.Users, ".DEFAULT\\Software\\FocusExternalModule", "SQLLoginID", "");
        }

        public static string SQLPW()
        {
            return RegValue(Microsoft.Win32.RegistryHive.Users, ".DEFAULT\\Software\\FocusExternalModule", "SQLPW", "");
        }
        public static string Default_Company()
        {
            return RegValue(Microsoft.Win32.RegistryHive.Users, ".DEFAULT\\Software\\FocusExternalModule", "IntegratedDBName", "");

        }
        public static string Multipleconnection()
        {
            return RegValue(Microsoft.Win32.RegistryHive.Users, ".DEFAULT\\Software\\FocusExternalModule", "Multipleconnection", "");

        }
        public static string Default_Company2()
        {
            return RegValue(Microsoft.Win32.RegistryHive.Users, ".DEFAULT\\Software\\Focus5", "Default Company", "");

        }
    }
}

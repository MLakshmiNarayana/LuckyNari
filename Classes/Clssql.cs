using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Foodicsapi.Classess
{
    internal class ClsSql
    {
        //public static SqlConnection Con;

        public ClsSql()
        {
            var sConStr = ClsComm.SqlConnectionString();
            SqlConnection Con = new SqlConnection(sConStr);
            var ConStr = ClsComm.SqlConnectionString2();
            SqlConnection Conn = new SqlConnection(ConStr);
        }

        public static int cmbBox(string Qry, ComboBox cmb)
        {
            //ComboBox cmb = new ComboBox();
            cmb.Enabled = false;
            DataSet Ds = GetDs(Qry);
            cmb.DataSource = Ds.Tables[0];
            cmb.DisplayMember = Ds.Tables[0].Columns[1].ColumnName;
            cmb.ValueMember = Ds.Tables[0].Columns[0].ColumnName;
            cmb.SelectedIndex = -1;
            cmb.Enabled = true;
            return 1;
        }

        public static int cmbBox_StdNarr(string Qry, ComboBox cmb)
        {
            //ComboBox cmb = new ComboBox();
            cmb.Enabled = false;
            DataSet Ds = ClsSql.GetDs(Qry);
            cmb.DataSource = Ds.Tables[0];
            cmb.DisplayMember = Ds.Tables[0].Columns[0].ColumnName;
            //cmb.ValueMember = Ds.Tables[0].Columns[0].ColumnName;
            cmb.AutoCompleteSource = AutoCompleteSource.ListItems;
            cmb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmb.SelectedIndex = -1;
            cmb.Enabled = true;
            return 1;
        }

        internal static SqlDataReader GetReader<TKey, TValue>(string getStr, SortedList<TKey, TValue> sortedList, object parametersList)
        {
            throw new NotImplementedException();
        }

        internal static SqlDataReader GetReader(string getStr, SortedList<object, object> parametersList)
        {
            throw new NotImplementedException();
        }

        public static DataGridViewComboBoxColumn GridCol(string Qry, string ColHeader)
        {
            DataGridViewComboBoxColumn cmb = new DataGridViewComboBoxColumn();
            DataSet Ds = ClsSql.GetDs(Qry);
            cmb.DataSource = Ds.Tables[0];
            cmb.Name = Ds.Tables[0].Columns[0].ColumnName;
            cmb.HeaderText = ColHeader;
            cmb.DisplayMember = Ds.Tables[0].Columns[1].ColumnName;
            cmb.ValueMember = Ds.Tables[0].Columns[0].ColumnName;
            cmb.Width = 250;
            cmb.AutoComplete = true;
            cmb.MaxDropDownItems = 3;
            cmb.FlatStyle = FlatStyle.Flat;
            return cmb;
        }


        public static DataGridViewComboBoxCell dgb_cmbCell(string Qry)
        {
            DataGridViewComboBoxCell cmbCell = new DataGridViewComboBoxCell();
            //dataGridView1["cmbUnit",e.RowIndex] as DataGridViewComboBoxCell;
            DataSet Ds = ClsSql.GetDs(Qry);
            //cmbcell.DataSource            
            cmbCell.DataSource = Ds.Tables[0];
            //cmbCell.Name = Ds.Tables[0].Columns[0].ColumnName;
            //cmbCell.HeaderText = ColHeader;
            cmbCell.DisplayMember = Ds.Tables[0].Columns[1].ColumnName;
            cmbCell.ValueMember = Ds.Tables[0].Columns[0].ColumnName;
            //cmbCell.Width = 200;
            cmbCell.AutoComplete = true;
            cmbCell.MaxDropDownItems = 3;
            cmbCell.FlatStyle = FlatStyle.Flat;

            return cmbCell;
        }

        public static DataGridViewComboBoxColumn dgb_cmbColumn(DataGridViewComboBoxColumn cmbCell, string Qry, string sHeaderText)
        {
            //DataGridViewComboBoxColumn cmbCell = new DataGridViewComboBoxColumn();
            DataSet Ds = ClsSql.GetDs(Qry);
            cmbCell.DataSource = Ds.Tables[0];
            cmbCell.HeaderText = sHeaderText;
            //cmbCell.Name = Ds.Tables[0].Columns[0].ColumnName;
            cmbCell.HeaderText = sHeaderText;
            cmbCell.Name = sHeaderText;
            cmbCell.DisplayMember = Ds.Tables[0].Columns[1].ColumnName;
            cmbCell.ValueMember = Ds.Tables[0].Columns[0].ColumnName;
            //cmbCell.Width = 200;
            cmbCell.AutoComplete = true;
            cmbCell.MaxDropDownItems = 3;
            cmbCell.FlatStyle = FlatStyle.Popup;

            return cmbCell;
        }

        public static DataGridViewComboBoxColumn dgb_cmbColumn2(DataGridViewComboBoxColumn cmbCell, string Qry, string sHeaderText)
        {
            //DataGridViewComboBoxColumn cmbCell = new DataGridViewComboBoxColumn();
            //dataGridView1["cmbUnit",e.RowIndex] as DataGridViewComboBoxCell;
            DataSet Ds = ClsSql.GetDs(Qry);
            //cmbcell.DataSource            
            cmbCell.DataSource = Ds.Tables[0];
            cmbCell.HeaderText = sHeaderText;
            //cmbCell.Name = Ds.Tables[0].Columns[0].ColumnName;
            cmbCell.Name = sHeaderText;

            //cmbCell.HeaderText = ColHeader;
            cmbCell.DisplayMember = Ds.Tables[0].Columns[1].ColumnName;
            //cmbCell.ValueMember = Ds.Tables[0].Columns[0].ColumnName;
            cmbCell.Width = 150;
            cmbCell.AutoComplete = true;
            cmbCell.MaxDropDownItems = 3;
            cmbCell.FlatStyle = FlatStyle.Popup;

            return cmbCell;
        }
        public static DataSet GetDs(string Qry)
        {
            string connectionString = ClsComm.SqlConnectionString();
            //string connectionString = ConfigurationManager.ConnectionStrings["SQLCon"].ToString;
            SqlConnection connection = new SqlConnection(connectionString);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            SqlCommand selectCommand = new SqlCommand(Qry, connection);
            SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet, "SMData");
            connection.Dispose();
            return dataSet;
        }
        public static DataSet GetDs2(string Qry)
        {
            string connectionString = ClsComm.SqlConnectionString2();
            //string connectionString = ConfigurationManager.ConnectionStrings["SQLCon"].ToString;
            SqlConnection connection = new SqlConnection(connectionString);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            SqlCommand selectCommand = new SqlCommand(Qry, connection);
            SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet, "SMData");
            connection.Dispose();
            return dataSet;
        }
        public static object GetQryVal(string Qry)
        {
            string connectionString = ClsComm.SqlConnectionString();
            //string connectionString = ConfigurationManager.ConnectionStrings["SQLCon"].ToString;
            SqlConnection connection = new SqlConnection(connectionString);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            SqlCommand selectCommand = new SqlCommand(Qry, connection);
            SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            connection.Dispose();
            return dataSet.Tables[0].Rows[0][0];
        }
        public static SqlDataReader GetReader(string commandQuery)
        {
            string connectionString = ClsComm.SqlConnectionString();
            SqlConnection connection = new SqlConnection(connectionString);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            SqlCommand command = new SqlCommand(commandQuery, connection);
            return command.ExecuteReader();
        }


        public static SqlDataReader GetReader(string spName, SortedList parametersList)
        {
            string connectionString = ClsComm.SqlConnectionString();
            SqlConnection connection = new SqlConnection(connectionString);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = spName;
            int count = parametersList.Count;
            for (int i = 0; i < count; i++)
            {
                string parameterName = parametersList.GetKey(i).ToString();
                object obj2 = parametersList[parameterName];
                command.Parameters.Add(new SqlParameter(parameterName, obj2));
            }
            return command.ExecuteReader();
        }

        public static DataTable SelectData(string selectStatment)
        {
            string connectionString = ClsComm.SqlConnectionString();
            SqlConnection connection = new SqlConnection(connectionString);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            var dt = new DataTable();

            var da = new SqlDataAdapter(selectStatment, connection);
            da.Fill(dt);

            return dt;
        }

        public static string gRun_select(string sQry)
        {
            string str1 = "";
            string sConStr = ClsComm.SqlConnectionString();
            //string connectionString = ConfigurationManager.ConnectionStrings["SQLCon"].ToString;
            SqlConnection con = new SqlConnection(sConStr);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }

            var dt = new DataTable();
            var da = new SqlDataAdapter(sQry, con);
            da.Fill(dt);
            str1 = (dt.Rows.Count != 0) ? dt.Rows[0][0].ToString() : "";
            dt.Dispose();
            da.Dispose();
            Close(con);
            return str1;
        }

        public static void ExcuteCommand(string excutestatment)
        {
            string sConStr = ClsComm.SqlConnectionString();
            //string connectionString = ConfigurationManager.ConnectionStrings["SQLCon"].ToString;
            SqlConnection con = new SqlConnection(sConStr);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }

            var sqlcmd = new SqlCommand(excutestatment, con);
            sqlcmd.ExecuteNonQuery();
            Close(con);

        }
        private static void Open()
        {
            var sConStr = ClsComm.SqlConnectionString();
            SqlConnection Con = new SqlConnection(sConStr);

            if (Con.State != ConnectionState.Open)
            {
                Con.Open();
            }
        }
        private static void Close(SqlConnection con)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
    }
}

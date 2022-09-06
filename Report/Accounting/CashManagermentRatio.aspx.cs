using Microsoft.Ajax.Utilities;
using Microsoft.Reporting.WebForms;
using MySql.Data.MySqlClient;
using Report.Models;
using Report.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Report.Accounting
{
    public partial class CashManagermentRatio : System.Web.UI.Page
    {
        private DBConnect db = new DBConnect();
        public string format = "dd/MM/yyyy";
        public string dateError = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.checkLoginSession();
                dtpSystemDate.Text = DataHelper.getSystemDate().ToString(format);
                DataHelper.populateBranchDDL(ddBranchName, DataHelper.getUserId());
            }
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            var dateSearch = "";
            try
            {
                dateSearch = DateTime.ParseExact(dtpSystemDate.Text.Trim(), format, null).ToString("yyyy-MM-dd");
            }
            catch (Exception)
            {
                dateError = "* Date wrong format";
                return;
            }
            var spd_cash = "PS_CASH_IN_BANK";
            List<Procedure> parameters_cash = new List<Procedure>();
            parameters_cash.Add(item: new Procedure() { field_name = "@pBranch", sql_db_type = MySqlDbType.VarChar, value_name = ddBranchName.SelectedItem.Value });
            parameters_cash.Add(item: new Procedure() { field_name = "@pSystemDate", sql_db_type = MySqlDbType.VarChar, value_name = dateSearch });
            DataTable dt_cash = db.getProcedureDataTable(spd_cash, parameters_cash);

            var spd_collect = "PS_CASH_COLLECT_ON_DATE";
            List<Procedure> parameters_collect = new List<Procedure>();
            parameters_collect.Add(item: new Procedure() { field_name = "@pBranch", sql_db_type = MySqlDbType.VarChar, value_name = ddBranchName.SelectedItem.Value });
            parameters_collect.Add(item: new Procedure() { field_name = "@pSystemDate", sql_db_type = MySqlDbType.VarChar, value_name = dateSearch });
            DataTable dt_collect = db.getProcedureDataTable(spd_collect, parameters_collect);

            var spd_end = "PS_CASH_ENDING";
            List<Procedure> parameters_end = new List<Procedure>();
            parameters_end.Add(item: new Procedure() { field_name = "@pBranch", sql_db_type = MySqlDbType.VarChar, value_name = ddBranchName.SelectedItem.Value });
            parameters_end.Add(item: new Procedure() { field_name = "@pSystemDate", sql_db_type = MySqlDbType.VarChar, value_name = dateSearch });
            DataTable dt_end = db.getProcedureDataTable(spd_end, parameters_end);

            
            GenerateReport(dt_cash, dt_collect,dt_end);


        }

        private void GenerateReport(DataTable dt1, DataTable dt2,DataTable dt3)
        {

            var reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("Branch", ddBranchName.SelectedItem.Text));
            reportParameters.Add(new ReportParameter("SystemDate", DateTime.ParseExact(dtpSystemDate.Text.Trim(), format, null).ToString("dd-MMM-yyyy")));

            var ds1 = new ReportDataSource("CashInBank", dt1);
            var ds2 = new ReportDataSource("CashCollect", dt2);
            var ds3 = new ReportDataSource("CashEnd", dt3);
            DataHelper.generateAccountingReport(ReportViewer1, "CashManagermentRatio", reportParameters, ds1,ds2,ds3);

        }
        }
}
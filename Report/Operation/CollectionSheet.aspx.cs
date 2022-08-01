using Microsoft.Reporting.WebForms;
using System;
using System.Data;
using Report.Utils;
using System.Web;

namespace Report.Operation
{
    public partial class CollectionSheet : System.Web.UI.Page
    {
        private DBConnect db = new DBConnect();
        DateTime currentDate = DateTime.Today;
        private static string fromDate, toDate;
        public string format = "dd/MM/yyyy";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.checkLoginSession();
                fromDate = dtpFromDate.Text;
                toDate = dtpToDate.Text;
                var sysDate = DataHelper.getSystemDate();
                if(Convert.ToBoolean(HttpContext.Current.Session["isSuperAdmin"]))
                {
                    DataHelper.populateBranchDDL(ddBranchName, DataHelper.getUserId(), true);
                }
                else
                {
                    DataHelper.populateBranchDDL(ddBranchName, DataHelper.getUserId(), false);
                }
                
                populateOfficer();
                dtpFromDate.Text = sysDate.ToString(format);
                dtpToDate.Text = sysDate.ToString(format);
            }
        }

        private void GenerateReport(DataTable collectionSheetDT)
        {
            ReportParameterCollection reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("Branch", ddBranchName.SelectedItem.Text));
            reportParameters.Add(new ReportParameter("PawnOfficer", ddOfficer.SelectedItem.Text));
            reportParameters.Add(new ReportParameter("FromDate", DateTime.ParseExact(dtpFromDate.Text, format, null).ToString("dd-MMM-yyyy")));
            reportParameters.Add(new ReportParameter("ToDate", DateTime.ParseExact(dtpToDate.Text, format, null).ToString("dd-MMM-yyyy")));

            var _collectionSheetlist = new ReportDataSource("CollectionSheetDS", collectionSheetDT);
            DataHelper.generateOperationReport(ReportViewer1, "CollectionSheet", reportParameters, _collectionSheetlist);
        }

        private void GenerateReportConsolidate(DataTable collectionSheetDT)
        {
            ReportParameterCollection reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("Branch", ddBranchName.SelectedItem.Text));
            reportParameters.Add(new ReportParameter("FromDate", DateTime.ParseExact(dtpFromDate.Text, format, null).ToString("dd-MMM-yyyy")));
            reportParameters.Add(new ReportParameter("ToDate", DateTime.ParseExact(dtpToDate.Text, format, null).ToString("dd-MMM-yyyy")));

            var _collectionSheetlist = new ReportDataSource("CollectionSheetConsolidateDS", collectionSheetDT);
            DataHelper.generateOperationReport(ReportViewer1, "CollectionSheetConsolidate", reportParameters, _collectionSheetlist);
        }

        protected void btnView_Click(object sender, EventArgs e)
        {   
            var fromDateSql = DateTime.ParseExact(dtpFromDate.Text, format, null);
            var fromDay = fromDateSql.ToString("yyyy-MM-dd");

            var toDateSql = DateTime.ParseExact(dtpToDate.Text, format, null);
            var toDay = toDateSql.ToString("yyyy-MM-dd");
            
            var branchId = ddBranchName.SelectedItem.Value;
            if (branchId != "0")
            {
                var branchWhere = " AND ST.branch_id=" + branchId;

                var sql = "SELECT ST.branch_id, B.branch_name, B.appr, ST.ticket_no,CUS.customer_name,CUS.personal_phone, " +
                    "CUR.currency,PL.principle_less princ_outstanding, ST.due_date, " +
                    "ST.interest_less,ST.principle_less,ST.penalty_less, " +
                    "P.lob_name,SI.name CO_Name " +
                    "FROM schedule_ticket ST " +
                    "INNER JOIN contract C ON ST.contract_id = C.id " +
                    "LEFT JOIN branch B ON ST.branch_id=B.id " +
                    "LEFT JOIN customer CUS ON C.customer_id = CUS.id " +
                    "LEFT JOIN currency CUR ON C.currency_id = CUR.id " +
                    "LEFT JOIN product P ON C.product_id = P.id " +
                    "LEFT JOIN staff_info SI ON C.pawn_officer_id = SI.id " +
                    "LEFT JOIN (SELECT ST.contract_id, SUM(ST.principle_less) principle_less " +
                    "    FROM schedule_ticket ST" +
                    "    WHERE ST.ticket_status != 'P' " + branchWhere +
                    " GROUP BY contract_id) PL ON C.id = PL.contract_id " +
                    "WHERE ST.ticket_status != 'P' AND ST.ticket_status != 'FPP' " +
                    "AND DATE(ST.due_date) BETWEEN DATE('" + fromDay + "') AND DATE('" + toDay + "') " +
                    branchWhere + " AND C.contract_status IN(4, 7) AND c.`b_status`= TRUE " +
                    "AND ST.ticket_status != 'I' ";

                if (ddOfficer.SelectedItem.Value != "0")
                {
                    sql += " AND C.pawn_officer_id=" + ddOfficer.SelectedItem.Value;
                }

                sql += " ORDER BY B.id";
                var collectionSheetDT = db.getDataTable(sql);
                GenerateReport(collectionSheetDT);
            }
            else
            {
                var sql = "SELECT B.id as branch_id, B.branch_name, B.appr, P.id AS lob_id, P.lob_name, IFNULL(CL.princ_outstanding, 0) princ_outstanding, " +
                "IFNULL(CL.interest_less, 0) interest_less, IFNULL(CL.principle_less, 0) principle_less, " +
                "IFNULL(CL.penalty_less, 0) penalty_less FROM branch B CROSS JOIN Product P " +
                "LEFT JOIN " +
                "(" +
                "SELECT ST.branch_id, C.`product_id`, SUM(PL.principle_less) princ_outstanding, " +
                "SUM(ST.interest_less) interest_less, SUM(ST.principle_less) principle_less, SUM(ST.penalty_less) penalty_less " +
                "FROM schedule_ticket ST " +
                "LEFT JOIN contract C ON ST.`contract_id`= C.id " +
                "LEFT JOIN(" +
                 "   SELECT ST.contract_id, SUM(ST.principle_less) principle_less " +
                 "   FROM schedule_ticket ST " +
                 "   WHERE ST.ticket_status != 'P' " +
                 "   GROUP BY contract_id " +
                ") PL ON ST.contract_id = PL.contract_id " +
                "WHERE ST.ticket_status != 'P' AND ST.ticket_status != 'FPP' " +
                "AND DATE(ST.due_date) BETWEEN DATE('"+ fromDay + "') AND DATE('"+ toDay +"')  " +
                "AND C.contract_status IN(4, 7) AND c.`b_status`= TRUE " +
                "AND ST.ticket_status != 'I' " +
                "GROUP BY ST.`branch_id`, C.`product_id` " +
                ") AS CL ON CL.branch_id = B.id AND P.id = CL.product_id ORDER BY B.id, P.id";
                
                var collectionSheetDT = db.getDataTable(sql);
                GenerateReportConsolidate(collectionSheetDT);
            }
        }

        protected void ddBranchName_SelectedIndexChanged(object sender, EventArgs e)
        {
            populateOfficer();
        }
        private void populateOfficer()
        {
            if (ddBranchName.SelectedItem.Value != "" && ddBranchName.SelectedItem.Value != "0")
            {
                ddOfficer.Enabled = true;
                DataHelper.populateOfficerDDL(ddOfficer, Convert.ToInt32(ddBranchName.SelectedItem.Value));
            }
            else
            {
                ddOfficer.Enabled = false;
                ddOfficer.Items.Clear();
            }
        }

    }
}
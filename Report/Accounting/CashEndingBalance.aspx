﻿<%@ Page Title="Cash Ending Balance" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CashEndingBalance.aspx.cs" Inherits="Report.Accounting.CashEndingBalance" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="../Content/jquery-ui.css" rel="Stylesheet" type="text/css" />
    <script src="../Scripts/jquery-ui.js" type="text/javascript"></script>
    <script src="../Scripts/datetimepicker.js" type="text/javascript"></script>
    <div class="panel panel-default no-margin">
        <div class="panel-body">
            <div class="row">
                <div class="col-sm-2 form-group">
                    <label>System Date :</label>
                    <asp:TextBox ID="dtpSystemDate" runat="server" class="form-control input-sm datepick"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="dtpSystemDate"
                        ErrorMessage="* Please select date" ForeColor="Red" Font-Names="Tahoma" Display="Dynamic">
                    </asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="reqValidate" runat="server" ControlToValidate="dtpSystemDate"
                        ErrorMessage="* Wrong date formate" ForeColor="Red" Font-Names="Tahoma" Display="Dynamic"
                        ValidationExpression="^([0-2][0-9]|(3)[0-1])(\/)(((0)[0-9])|((1)[0-2]))(\/)\d{4}$"></asp:RegularExpressionValidator>
                    <span style="color: red"><%=dateError %></span>
                </div>
                <div class="form-group ml16">
                    <div>
                        <label>&nbsp;</label>
                    </div>
                    <asp:Button ID="btnView" runat="server" Text="View Report" CssClass="btn btn-sm btn-primary" OnClick="btnView_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <center>
                <rsweb:ReportViewer ID="ReportViewer1" runat="server" nt-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana"  
                WaitMessageFont-Size="14pt" ShowPrintButton="true" ShowBackButton="true" BackColor="#999999" CssClass="printer"  
                PageCountMode="Actual" ShowZoomControl="False"></rsweb:ReportViewer>
            </center>
        </div>
    </div>
</asp:Content>
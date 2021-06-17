﻿<%@ Page Title="CO Productivity" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="COProductivity.aspx.cs" Inherits="Report.Operation.COProductivity" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="panel panel-warning no-margin">
        <div class="panel-body">
            <div class="form-inline">
                <div class="form-group">
                    <label>Branch:</label>
                    <asp:DropDownList ID="ddBranchName" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddBranchName_SelectedIndexChanged" CssClass="form-control cnt-min-width">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddBranchName"
                        ErrorMessage="* Please select branch" ForeColor="Red" Font-Names="Tahoma" Display="Dynamic">
                    </asp:RequiredFieldValidator>
                </div>
                <div class="form-group ml16">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddBranchName" />
                        </Triggers>
                        <ContentTemplate>
                            <label>Pawn Officer:</label>
                            <asp:DropDownList ID="ddOfficer" runat="server" CssClass="form-control cnt-min-width" Enabled="false">
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="ddOfficer"
                                ErrorMessage="* Please select officer" ForeColor="Red" Font-Names="Tahoma" Display="Dynamic">
                            </asp:RequiredFieldValidator>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="form-group ml16">
                    <label>Currency:</label>
                    <asp:DropDownList ID="ddCurrency" runat="server" CssClass="form-control cnt-min-width">
                    </asp:DropDownList>
                </div>
                <div class="form-group ml16">
                    <asp:Button ID="btnView" runat="server" Text="View" OnClick="btnView_Click" CssClass="btn btn-info" />
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

<%-- File: CustomerLogReport.aspx --%>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CustomerLogReport.aspx.cs" Inherits="CustomerLogReport" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Customer View Log Report</title>
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" />
    <style>
        body { padding: 20px; background-color: #f8f9fa; font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif; }
        .report-container { max-width: 1200px; margin: auto; background-color: #fff; padding: 30px; border-radius: 8px; box-shadow: 0 4px 8px rgba(0,0,0,0.1); }
        .filter-bar { display: flex; gap: 15px; align-items: flex-end; margin-bottom: 25px; flex-wrap: wrap; }
        .filter-group { display: flex; flex-direction: column; }
        .filter-group label { font-weight: 500; margin-bottom: 5px; font-size: 0.9em; color: #555; }
        .grid-header th { background-color: #f2f2f2; font-weight: 600; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="report-container">
            <h2 class="mb-4">Customer View Log Report</h2>
            
            <!-- Filter Section -->
            <div class="filter-bar">
                <div class="filter-group">
                    <label for="ddlCustomer">Customer (Company)</label>
                    <asp:DropDownList ID="ddlCustomer" runat="server" CssClass="form-control" style="min-width: 220px;"/>
                </div>
                <div class="filter-group">
                    <label for="txtFromDate">From Date</label>
                    <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" TextMode="Date" />
                </div>
                <div class="filter-group">
                    <label for="txtToDate">To Date</label>
                    <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" TextMode="Date" />
                </div>
                <div class="filter-group">
                    <asp:Button ID="btnFilter" runat="server" Text="Filter" OnClick="btnFilter_Click" CssClass="btn btn-primary" />
                </div>
                 <div class="filter-group ml-auto">
                    <asp:Button ID="btnDownloadCSV" runat="server" Text="Download CSV" OnClick="btnDownloadCSV_Click" CssClass="btn btn-success" />
                </div>
            </div>

            <!-- Data Grid to display logs -->
            <asp:GridView ID="gvLogs" runat="server" 
                AutoGenerateColumns="False" 
                CssClass="table table-bordered table-hover"
                GridLines="None"
                AllowPaging="True"
                OnPageIndexChanging="gvLogs_PageIndexChanging"
                PageSize="10"
                AllowSorting="True"
                OnSorting="gvLogs_Sorting">
                <HeaderStyle CssClass="grid-header" />
                <PagerStyle CssClass="pagination-ys" HorizontalAlign="Right" />
                <EmptyDataTemplate>
                    <div class="alert alert-info text-center mt-3">No log entries found for the selected criteria.</div>
                </EmptyDataTemplate>
                <Columns>
                    <asp:BoundField DataField="UserName" HeaderText="User" SortExpression="UserName" />
                    <asp:BoundField DataField="Company" HeaderText="Company" SortExpression="Company" />
                    <asp:BoundField DataField="IpAddress" HeaderText="IP Address" SortExpression="IpAddress" />
                    <asp:BoundField DataField="PageVisited" HeaderText="Page Visited" SortExpression="PageVisited" />
                    <asp:BoundField DataField="Action" HeaderText="Action" SortExpression="Action" />
                    <asp:BoundField DataField="Timestamp" HeaderText="Timestamp" SortExpression="Timestamp" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>



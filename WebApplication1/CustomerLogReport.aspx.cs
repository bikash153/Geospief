using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication1.Models;

public partial class CustomerLogReport : System.Web.UI.Page
{
    private readonly string _connectionString = ConfigurationManager.ConnectionStrings["PgConnection"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtFromDate.Text = DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd");

            PopulateCompanyFilter();
            BindLogData();
        }
    }

    private void PopulateCompanyFilter()
    {
        string query = "SELECT DISTINCT region FROM public.user_m WHERE region IS NOT NULL AND region <> '' ORDER BY region;";
        var companies = new List<ListItem>();

        using (var conn = new NpgsqlConnection(_connectionString))
        using (var cmd = new NpgsqlCommand(query, conn))
        {
            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string companyName = reader["region"].ToString();
                    companies.Add(new ListItem(companyName, companyName));
                }
            }
        }
        ddlCustomer.DataSource = companies;
        ddlCustomer.DataTextField = "Text";
        ddlCustomer.DataValueField = "Value";
        ddlCustomer.DataBind();
        ddlCustomer.Items.Insert(0, new ListItem("All Companies", ""));
    }

    private void BindLogData()
    {
        List<CustomerLogDto> logs = FetchLogData();

        if (ViewState["SortExpression"] != null)
        {
            string sortExpression = ViewState["SortExpression"].ToString();
            SortDirection sortDirection = (SortDirection)ViewState["SortDirection"];

            if (sortDirection == SortDirection.Ascending)
            {
                logs = logs.OrderBy(l => GetPropertyValue(l, sortExpression)).ToList();
            }
            else
            {
                logs = logs.OrderByDescending(l => GetPropertyValue(l, sortExpression)).ToList();
            }
        }

        gvLogs.DataSource = logs;
        gvLogs.DataBind();
    }

    private List<CustomerLogDto> FetchLogData()
    {
        var logs = new List<CustomerLogDto>();
        var queryBuilder = new StringBuilder(@"
            SELECT u.user_name AS UserName, u.region AS Company, cl.ip_address AS IpAddress, cl.page_visited AS PageVisited, cl.action AS Action, cl.timestamp AS Timestamp
            FROM public.customer_log cl JOIN public.user_m u ON cl.user_id = u.id WHERE 1=1 ");

        using (var conn = new NpgsqlConnection(_connectionString))
        using (var cmd = new NpgsqlCommand())
        {
            DateTime fromDate;
            if (DateTime.TryParse(txtFromDate.Text, out fromDate))
            {
                queryBuilder.Append(" AND cl.timestamp >= @FromDate");
                cmd.Parameters.AddWithValue("FromDate", fromDate.Date);
            }

            DateTime toDate;
            if (DateTime.TryParse(txtToDate.Text, out toDate))
            {
                queryBuilder.Append(" AND cl.timestamp <= @ToDate");
                cmd.Parameters.AddWithValue("ToDate", toDate.Date.AddDays(1).AddTicks(-1));
            }

            if (!string.IsNullOrEmpty(ddlCustomer.SelectedValue))
            {
                queryBuilder.Append(" AND u.region = @Company");
                cmd.Parameters.AddWithValue("Company", ddlCustomer.SelectedValue);
            }

            queryBuilder.Append(" ORDER BY cl.timestamp DESC");

            cmd.Connection = conn;
            cmd.CommandText = queryBuilder.ToString();

            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    logs.Add(new CustomerLogDto
                    {
                        UserName = reader["UserName"].ToString(),
                        Company = reader["Company"].ToString(),
                        IpAddress = reader["IpAddress"].ToString(),
                        PageVisited = reader["PageVisited"].ToString(),
                        Action = reader["Action"].ToString(),
                        Timestamp = Convert.ToDateTime(reader["Timestamp"])
                    });
                }
            }
        }
        return logs;
    }

    #region Event Handlers

    protected void btnFilter_Click(object sender, EventArgs e)
    {
        gvLogs.PageIndex = 0;
        BindLogData();
    }

    protected void btnDownloadCSV_Click(object sender, EventArgs e)
    {
        List<CustomerLogDto> dataToExport = FetchLogData();

        var sb = new StringBuilder();
        sb.AppendLine("User,Company,IP Address,Page Visited,Action,Timestamp");

        foreach (var log in dataToExport)
        {
            sb.AppendLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5:yyyy-MM-dd HH:mm:ss}\"",
                log.UserName, log.Company, log.IpAddress, log.PageVisited, log.Action, log.Timestamp));
        }

        Response.Clear();
        Response.ContentType = "text/csv";
        Response.AddHeader("Content-Disposition", "attachment;filename=CustomerLogReport.csv");
        Response.Write(sb.ToString());
        Response.End();
    }

    protected void gvLogs_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvLogs.PageIndex = e.NewPageIndex;
        BindLogData();
    }

    protected void gvLogs_Sorting(object sender, GridViewSortEventArgs e)
    {
        SortDirection direction = SortDirection.Ascending;
        if (ViewState["SortExpression"] != null && ViewState["SortExpression"].ToString() == e.SortExpression)
        {
            if ((SortDirection)ViewState["SortDirection"] == SortDirection.Ascending)
                direction = SortDirection.Descending;
        }
        ViewState["SortExpression"] = e.SortExpression;
        ViewState["SortDirection"] = direction;
        BindLogData();
    }

    #endregion

    private object GetPropertyValue(object obj, string propertyName)
    {
        PropertyInfo prop = obj.GetType().GetProperty(propertyName);
        if (prop != null)
        {
            return prop.GetValue(obj, null);
        }
        return null;
    }
}

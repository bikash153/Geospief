using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Script.Services;
using System.Web.Services;
using Npgsql;

public partial class CustomerMillMapping : System.Web.UI.Page
{
    private static readonly string connectionString = ConfigurationManager.ConnectionStrings["PgConnection"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<object> GetRegions()
    {
        var list = new List<object>();
        using (var con = new NpgsqlConnection(connectionString))
        {
            var query = "SELECT region_code, region_name FROM public.regions ORDER BY region_name";
            using (var cmd = new NpgsqlCommand(query, con))
            {
                con.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new { id = reader["region_code"].ToString(), text = reader["region_name"].ToString() });
                }
            }
        }
        return list;
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<object> GetUsersByRegion(string regionCode)
    {
        var list = new List<object>();
        using (var con = new NpgsqlConnection(connectionString))
        {
            var query = "SELECT id, name FROM public.user_m WHERE region_code = @RegionCode AND is_active = true AND role_id = 5 ORDER BY name";
            using (var cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@RegionCode", Convert.ToInt32(regionCode));
                con.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new { id = reader["id"].ToString(), text = reader["name"].ToString() });
                }
            }
        }
        return list;
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<object> GetMillsByRegion(string regionCode)
    {
        var list = new List<object>();
        using (var con = new NpgsqlConnection(connectionString))
        {
            var query = "SELECT id, mill_name FROM public.mill WHERE region_code = @RegionCode ORDER BY mill_name";
            using (var cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@RegionCode", Convert.ToInt32(regionCode));
                con.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new { id = reader["id"].ToString(), text = reader["mill_name"].ToString() });
                }
            }
        }
        return list;
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static bool SaveMapping(int userId, List<int> millIds)
    {
        using (var con = new NpgsqlConnection(connectionString))
        {
            con.Open();
            using (var tran = con.BeginTransaction())
            {
                var deleteCmd = new NpgsqlCommand("DELETE FROM public.usermillmapping WHERE userid = @UserID", con);
                deleteCmd.Parameters.AddWithValue("@UserID", userId);
                deleteCmd.ExecuteNonQuery();

                if (millIds != null && millIds.Count > 0)
                {
                    foreach (int millId in millIds)
                    {
                        var insertCmd = new NpgsqlCommand("INSERT INTO public.usermillmapping (userid, millid) VALUES (@UserID, @MillID)", con);
                        insertCmd.Parameters.AddWithValue("@UserID", userId);
                        insertCmd.Parameters.AddWithValue("@MillID", millId);
                        insertCmd.ExecuteNonQuery();
                    }
                }
                tran.Commit();
            }
        }
        return true;
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<object> GetMappedData()
    {
        var list = new List<object>();
        var query = @"
            SELECT
                u.id AS ""userId"",
                u.name AS ""userName"",
                u.region_code AS ""regionCode"",
                m.id AS ""millId"",
                m.mill_name AS ""millName""
            FROM public.usermillmapping umm
            JOIN public.user_m u ON umm.userid = u.id
            JOIN public.mill m ON umm.millid = m.id
            ORDER BY u.name, m.mill_name;
        ";
        using (var con = new NpgsqlConnection(connectionString))
        {
            using (var cmd = new NpgsqlCommand(query, con))
            {
                con.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new
                    {
                        userId = Convert.ToInt32(reader["userId"]),
                        userName = reader["userName"].ToString(),
                        regionCode = reader["regionCode"].ToString(),
                        millId = Convert.ToInt32(reader["millId"]),
                        millName = reader["millName"].ToString()
                    });
                }
            }
        }
        return list;
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static bool DeleteUserMapping(int userId)
    {
        using (var con = new NpgsqlConnection(connectionString))
        {
            var cmd = new NpgsqlCommand("DELETE FROM public.usermillmapping WHERE userid = @UserID", con);
            cmd.Parameters.AddWithValue("@UserID", userId);
            con.Open();
            cmd.ExecuteNonQuery();
        }
        return true;
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Script.Services;
using System.Web.Services;
using Npgsql;
using WebApplication1.Models;

public partial class UserManagement : System.Web.UI.Page
{
    private static readonly string connectionString = ConfigurationManager.ConnectionStrings["PgConnection"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e) { }

    private static string HashPassword(string password)
    {
        return password; // In a real app, replace with a proper hashing library like BCrypt.Net
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<object> GetRoles()
    {
        var list = new List<object>();
        using (var con = new NpgsqlConnection(connectionString))
        {
            var query = "SELECT id, role_name FROM public.role_m ORDER BY role_name";
            using (var cmd = new NpgsqlCommand(query, con))
            {
                con.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read()) { list.Add(new { id = reader["id"].ToString(), text = reader["role_name"].ToString() }); }
            }
        }
        return list;
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<object> GetLoginTypes()
    {
        var list = new List<object>();
        using (var con = new NpgsqlConnection(connectionString))
        {
            var query = "SELECT id, name FROM public.login_type_m ORDER BY name";
            using (var cmd = new NpgsqlCommand(query, con))
            {
                con.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read()) { list.Add(new { id = reader["id"].ToString(), text = reader["name"].ToString() }); }
            }
        }
        return list;
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<object> GetUsers()
    {
        var list = new List<object>();
        var query = @"
            SELECT u.id, u.name, u.user_name, r.role_name, u.region, 
                   u.email, u.mobile_no, lt.name as login_type, u.is_active
            FROM public.user_m u
            LEFT JOIN public.role_m r ON u.role_id = r.id
            LEFT JOIN public.login_type_m lt ON u.login_type_id = lt.id
            ORDER BY u.id";

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
                        id = "USR" + Convert.ToInt32(reader["id"]).ToString("D3"),
                        dbId = Convert.ToInt32(reader["id"]),
                        name = reader["name"].ToString(),
                        userName = reader["user_name"].ToString(),
                        role = reader["role_name"] != DBNull.Value ? reader["role_name"].ToString() : "N/A",
                        region = reader["region"].ToString(),
                        email = reader["email"].ToString(),
                        mobile = reader["mobile_no"].ToString(),
                        loginType = reader["login_type"] != DBNull.Value ? reader["login_type"].ToString() : "N/A",
                        isActive = Convert.ToBoolean(reader["is_active"])
                    });
                }
            }
        }
        return list;
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static UserDto GetUserById(int userId)
    {
        UserDto user = null;
        var query = "SELECT id, name, user_name, role_id, mobile_no, email, region, login_type_id, is_active FROM public.user_m WHERE id = @UserId";
        using (var con = new NpgsqlConnection(connectionString))
        {
            using (var cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                con.Open();
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    user = new UserDto
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString(),
                        UserName = reader["user_name"].ToString(),
                        RoleId = reader["role_id"] != DBNull.Value ? Convert.ToInt32(reader["role_id"]) : 0,
                        MobileNo = reader["mobile_no"].ToString(),
                        Email = reader["email"].ToString(),
                        Region = reader["region"].ToString(),
                        LoginTypeId = reader["login_type_id"] != DBNull.Value ? Convert.ToInt32(reader["login_type_id"]) : 0,
                        IsActive = Convert.ToBoolean(reader["is_active"])
                    };
                }
            }
        }
        return user;
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static bool SaveUser(UserDto user)
    {
        string query;
        using (var con = new NpgsqlConnection(connectionString))
        {
            con.Open();
            using (var tran = con.BeginTransaction())
            {
                int? regionCode = null;
                if (!string.IsNullOrEmpty(user.Region))
                {
                    var regionQuery = "SELECT region_code FROM public.regions WHERE region_name = @RegionName LIMIT 1";
                    using (var regionCmd = new NpgsqlCommand(regionQuery, con, tran))
                    {
                        regionCmd.Parameters.AddWithValue("@RegionName", user.Region.Trim());
                        object result = regionCmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value) { regionCode = Convert.ToInt32(result); }
                    }
                }

                if (user.Id == 0)
                {
                    query = @"INSERT INTO public.user_m (name, user_name, password, role_id, mobile_no, email, region, region_code, login_type_id, is_active)
                              VALUES (@Name, @UserName, @Password, @RoleId, @MobileNo, @Email, @Region, @RegionCode, @LoginTypeId, @IsActive)";
                }
                else
                {
                    string passwordQueryPart = string.IsNullOrEmpty(user.Password) ? "" : "password = @Password,";
                    query = string.Format(@"UPDATE public.user_m 
                                            SET name = @Name, user_name = @UserName, {0} role_id = @RoleId, mobile_no = @MobileNo, 
                                                email = @Email, region = @Region, region_code = @RegionCode, login_type_id = @LoginTypeId, is_active = @IsActive
                                            WHERE id = @Id", passwordQueryPart);
                }

                using (var cmd = new NpgsqlCommand(query, con, tran))
                {
                    cmd.Parameters.AddWithValue("@Name", user.Name);
                    cmd.Parameters.AddWithValue("@UserName", user.UserName);
                    cmd.Parameters.AddWithValue("@RoleId", user.RoleId);
                    cmd.Parameters.AddWithValue("@MobileNo", (object)user.MobileNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)user.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Region", (object)user.Region ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LoginTypeId", user.LoginTypeId);
                    cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                    cmd.Parameters.AddWithValue("@RegionCode", (object)regionCode ?? DBNull.Value);

                    if (user.Id != 0) { cmd.Parameters.AddWithValue("@Id", user.Id); }
                    if (!string.IsNullOrEmpty(user.Password)) { cmd.Parameters.AddWithValue("@Password", HashPassword(user.Password)); }

                    cmd.ExecuteNonQuery();
                }
                tran.Commit();
            }
        }
        return true;
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static bool DeleteUser(int userId)
    {
        var query = "DELETE FROM public.user_m WHERE id = @UserId";
        using (var con = new NpgsqlConnection(connectionString))
        {
            using (var cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
        return true;
    }
}
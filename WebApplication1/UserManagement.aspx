<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserManagement.aspx.cs" Inherits="UserManagement" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>User Management</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.6.0/jquery.min.js"></script>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    <link href="Content/usermgmt-style.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
        <div class="main-container">
            <div class="card">
                <h2>User Management</h2>
                <div class="form-grid">
                    <input type="hidden" id="hdnUserId" value="0" />
                    <div class="form-group"><label>Full Name</label><input type="text" id="txtFullName" placeholder="User full name" /></div>
                    <div class="form-group"><label>Username</label><input type="text" id="txtUsername" placeholder="Login username" /></div>
                    <div class="form-group"><label>Email</label><input type="email" id="txtEmail" placeholder="Email address" /></div>
                    <div class="form-group"><label>Mobile No</label><input type="text" id="txtMobile" placeholder="Mobile number" /></div>
                    <div class="form-group"><label>Password</label><input type="password" id="txtPassword" placeholder="Enter new password to change" /></div>
                    <div class="form-group"><label>Role</label><select id="ddlRole"></select></div>
                    <div class="form-group"><label>Region</label><input type="text" id="txtRegion" placeholder="Region" /></div>
                    <div class="form-group"><label>Login Type</label><select id="ddlLoginType"><option value="Local">Local</option><option value="SSO">SSO</option></select></div>
                    <div class="form-group"><label>Status</label><select id="ddlStatus"><option value="true">Active</option><option value="false">Inactive</option></select></div>
                </div>
                <div class="button-group">
                    <button type="button" id="btnSave" class="btn-save">Add/Save</button>
                    <button type="button" id="btnReset" class="btn-reset">Reset</button>
                </div>
            </div>
            <div class="card">
                <h2>User Data</h2>
                <div class="table-container">
                    <table id="userDataTable">
                        <thead>
                            <tr>
                                <th>Actions</th>
                                <th>User ID</th>
                                <th>Full Name</th>
                                <th>Username</th>
                                <th>Role</th>
                                <th>Region</th>
                                <th>Email</th>
                                <th>Mobile</th>
                                <th>Login Type</th>
                                <th>Status</th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </form>
    <script src="Scripts/usermgmt-script.js"></script>
</body>
</html>
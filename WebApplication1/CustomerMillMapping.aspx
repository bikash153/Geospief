<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CustomerMillMapping.aspx.cs" Inherits="CustomerMillMapping" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>User and Mills Mapping</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.6.0/jquery.min.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css" />
    <link href="Content/mapping-style.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
        <div class="main-container">
            <div class="card">
                <h2>User and Mills Mapping</h2>
                <div class="filter-grid">
                    <div class="form-group">
                        <label for="ddlRegion">Region</label>
                        <select id="ddlRegion" class="form-control"></select>
                    </div>
                    <div class="form-group">
                        <label for="ddlUser">User</label>
                        <select id="ddlUser" class="form-control" disabled></select>
                    </div>
                    <div class="form-group">
                        <label for="ddlMills">Select Mills</label>
                        <select id="ddlMills" class="form-control" multiple="multiple" disabled></select>
                    </div>
                    <div class="form-group-button">
                        <button type="button" id="btnMap" class="btn-map">Map</button>
                        <button type="button" id="btnUpdate" class="btn-update-main" style="display:none;">Update</button>
                        <button type="button" id="btnCancel" class="btn-cancel" style="display:none;">Cancel</button>
                    </div>
                </div>
            </div>

            <div class="card">
                <h2>Mapped Users & Mills</h2>
                <div class="table-container">
                    <table id="mappedDataTable">
                        <thead>
                            <tr>
                                <th>User</th>
                                <th>Mapped Mills</th>
                                <th>Action</th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </form>
    <script src="Scripts/mapping-script.js"></script>
</body>
</html>
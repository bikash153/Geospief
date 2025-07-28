$(document).ready(function () {
    // Load initial data for dropdowns and the user table
    loadRoles();
    loadLoginTypes();
    loadUsers();

    $('#btnSave').on('click', function () {
        // Build the user object to send to the server
        const user = {
            Id: parseInt($('#hdnUserId').val()),
            Name: $('#txtFullName').val(),
            UserName: $('#txtUsername').val(),
            Password: $('#txtPassword').val(),
            // CORRECTION: Ensure RoleId is parsed as an integer
            RoleId: parseInt($('#ddlRole').val()),
            MobileNo: $('#txtMobile').val(),
            Email: $('#txtEmail').val(),
            Region: $('#txtRegion').val(),
            // CORRECTION: The property MUST be 'LoginTypeId' to match the C# DTO
            LoginTypeId: parseInt($('#ddlLoginType').val()),
            IsActive: $('#ddlStatus').val() === 'true'
        };

        // --- VALIDATION FIX ---
        // Add checks to ensure dropdowns have a valid selection before saving.
        if (!user.Name || !user.UserName) {
            alert('Full Name and Username are required.');
            return;
        }
        if (isNaN(user.RoleId)) {
            alert('Please select a valid Role.');
            return;
        }
        if (isNaN(user.LoginTypeId)) {
            alert('Please select a valid Login Type.');
            return;
        }
        // --- END VALIDATION FIX ---

        callPageMethod("SaveUser", { user: user }).done(function (response) {
            if (response.d) {
                alert('User saved successfully!');
                resetForm();
                loadUsers();
            }
        });
    });

    $('#btnReset').on('click', function () {
        resetForm();
    });

    const tableBody = $('#userDataTable tbody');

    tableBody.on('click', '.btn-edit', function () {
        const userId = $(this).data('userid');
        callPageMethod("GetUserById", { userId: userId }).done(function (response) {
            populateForm(response.d);
        });
    });

    tableBody.on('click', '.btn-delete', function () {
        const userId = $(this).data('userid');
        if (confirm('Are you sure you want to delete this user?')) {
            callPageMethod("DeleteUser", { userId: userId }).done(function (response) {
                if (response.d) {
                    alert('User deleted successfully.');
                    loadUsers();
                }
            });
        }
    });

    function callPageMethod(methodName, data) {
        return $.ajax({
            type: "POST", url: "UserManagement.aspx/" + methodName, data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8", dataType: "json",
            error: function (xhr) { console.error("Error in " + methodName, xhr.responseText); }
        });
    }

    function loadRoles() {
        callPageMethod("GetRoles", {}).done(function (response) {
            populateDropdown('#ddlRole', response.d, 'Select Role');
        });
    }

    // This function must be called to populate the Login Type dropdown
    function loadLoginTypes() {
        callPageMethod("GetLoginTypes", {}).done(function (response) {
            populateDropdown('#ddlLoginType', response.d, 'Select Login Type');
        });
    }

    function loadUsers() {
        callPageMethod("GetUsers", {}).done(function (response) {
            tableBody.empty();
            response.d.forEach(user => {
                const statusPill = user.isActive
                    ? '<span class="status-pill status-active">Active</span>'
                    : '<span class="status-pill status-inactive">Inactive</span>';

                const row = `
                    <tr>
                        <td>
                            <div class="action-buttons">
                                <button type="button" class="action-btn btn-delete" title="Delete" data-userid="${user.dbId}"><i class="fas fa-trash-alt"></i></button>
                                <button type="button" class="action-btn btn-view" title="View/Permissions"><i class="fas fa-eye"></i></button>
                                <button type="button" class="action-btn btn-edit" title="Edit" data-userid="${user.dbId}"><i class="fas fa-edit"></i></button>
                            </div>
                        </td>
                        <td>${user.id}</td>
                        <td>${user.name}</td>
                        <td>${user.userName}</td>
                        <td>${user.role}</td>
                        <td>${user.region}</td>
                        <td>${user.email}</td>
                        <td>${user.mobile}</td>
                        <td>${user.loginType}</td>
                        <td>${statusPill}</td>
                    </tr>`;
                tableBody.append(row);
            });
        });
    }

    function populateForm(user) {
        if (!user) return;
        $('#hdnUserId').val(user.Id);
        $('#txtFullName').val(user.Name);
        $('#txtUsername').val(user.UserName);
        $('#txtEmail').val(user.Email);
        $('#txtMobile').val(user.MobileNo);
        $('#ddlRole').val(user.RoleId);
        $('#txtRegion').val(user.Region);
        // CORRECTION: The property from the C# DTO is 'LoginTypeId'
        $('#ddlLoginType').val(user.LoginTypeId);
        $('#ddlStatus').val(user.IsActive.toString());
        $('#txtPassword').attr('placeholder', 'Password unchanged. Enter new to update.');
        window.scrollTo(0, 0);
    }

    function resetForm() {
        $('#form1').find('input[type=text], input[type=password], input[type=email], input[type=hidden], select').val('');
        $('#hdnUserId').val('0');
        $('#ddlStatus').val('true');
        // CORRECTION: Clear the Login Type dropdown correctly
        $('#ddlLoginType').val('');
        $('#txtPassword').attr('placeholder', 'Enter password');
    }

    // New helper function to keep the code clean
    function populateDropdown(selector, data, placeholder) {
        const dropdown = $(selector);
        dropdown.empty().append(`<option value="">${placeholder}</option>`);
        data.forEach(item => dropdown.append(new Option(item.text, item.id)));
    }
});
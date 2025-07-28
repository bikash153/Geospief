$(document).ready(function () {
    let editingUserId = null;

    $('#ddlRegion, #ddlUser, #ddlMills').select2({
        width: '100%',
        placeholder: "Select an option",
        allowClear: true
    });

    loadRegions();
    loadMappedDataTable();

    $('#ddlRegion').on('change', function () {
        const regionCode = $(this).val();
        resetDropdowns(['#ddlUser', '#ddlMills']);
        if (regionCode) {
            loadUsers(regionCode);
            loadMills(regionCode);
        }
    });

    $('#btnMap').on('click', function () {
        const userId = $('#ddlUser').val();
        const millIds = $('#ddlMills').val();
        if (!userId) {
            alert('Please select a user.');
            return;
        }
        callPageMethod("SaveMapping", { userId: parseInt(userId), millIds: millIds.map(Number) })
            .done(function (response) {
                if (response.d) {
                    alert('Mapping saved successfully!');
                    loadMappedDataTable();
                    resetForm();
                }
            });
    });

    $('#btnUpdate').on('click', function () {
        const userId = editingUserId;
        const millIds = $('#ddlMills').val();
        if (!userId) {
            alert('An error occurred. No user is being edited.');
            return;
        }
        callPageMethod("SaveMapping", { userId: parseInt(userId), millIds: millIds.map(Number) })
            .done(function (response) {
                if (response.d) {
                    alert('Mapping updated successfully!');
                    loadMappedDataTable();
                    resetForm();
                }
            });
    });

    $('#btnCancel').on('click', function () {
        resetForm();
    });

    const tableBody = $('#mappedDataTable tbody');

    tableBody.on('click', '.btn-update', function () {
        const button = $(this);
        const userId = button.data('userid');
        const regionCode = button.data('regioncode');
        const mappedMills = JSON.parse(button.data('mills'));

        enterEditMode(userId);

        $('#ddlRegion').val(regionCode).trigger('change.select2');

        const usersPromise = loadUsers(regionCode);
        const millsPromise = loadMills(regionCode);

        $.when(usersPromise, millsPromise).done(function () {
            $('#ddlUser').val(userId).trigger('change.select2');
            $('#ddlMills').val(mappedMills).trigger('change.select2');
            $('#ddlUser').prop('disabled', true);
            window.scrollTo(0, 0);
        });
    });

    tableBody.on('click', '.btn-delete', function () {
        const userId = $(this).data('userid');
        if (confirm('Are you sure you want to delete all mappings for this user?')) {
            callPageMethod("DeleteUserMapping", { userId: userId }).done(function (response) {
                if (response.d) {
                    alert('Mappings deleted successfully.');
                    loadMappedDataTable();
                }
            });
        }
    });

    function callPageMethod(methodName, data) {
        return $.ajax({
            type: "POST",
            url: "CustomerMillMapping.aspx/" + methodName,
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            error: function (xhr) {
                console.error("Error calling method '" + methodName + "':", xhr.responseText);
                alert("An error occurred. Check the console (F12) for details.");
            }
        });
    }

    function loadRegions() {
        callPageMethod("GetRegions", {}).done(function (response) {
            populateDropdown('#ddlRegion', response.d, "Select Region");
        });
    }

    function loadUsers(regionCode) {
        const promise = callPageMethod("GetUsersByRegion", { regionCode: regionCode });
        promise.done(function (response) {
            populateDropdown('#ddlUser', response.d, "Select User");
            if (!editingUserId) {
                $('#ddlUser').prop('disabled', false);
            }
        });
        return promise;
    }

    function loadMills(regionCode) {
        const promise = callPageMethod("GetMillsByRegion", { regionCode: regionCode });
        promise.done(function (response) {
            populateDropdown('#ddlMills', response.d, "Select Mills");
            $('#ddlMills').prop('disabled', false);
        });
        return promise;
    }

    function loadMappedDataTable() {
        callPageMethod("GetMappedData", {}).done(function (response) {
            const data = response.d;
            const tableBody = $('#mappedDataTable tbody');
            tableBody.empty();
            if (data.length === 0) {
                tableBody.append('<tr><td colspan="3">No mappings have been created yet.</td></tr>');
                return;
            }
            const groupedByUser = data.reduce((acc, item) => {
                if (!acc[item.userId]) {
                    acc[item.userId] = { userName: item.userName, regionCode: item.regionCode, mills: [] };
                }
                acc[item.userId].mills.push({ millId: item.millId, millName: item.millName });
                return acc;
            }, {});
            for (const userId in groupedByUser) {
                const userData = groupedByUser[userId];
                const millCount = userData.mills.length;
                const millIds = JSON.stringify(userData.mills.map(m => m.millId));
                for (let i = 0; i < millCount; i++) {
                    let rowHtml = '<tr>';
                    if (i === 0) {
                        rowHtml += `<td class="user-cell" rowspan="${millCount}">${userData.userName}</td>`;
                    }
                    rowHtml += `<td>${userData.mills[i].millName}</td>`;
                    if (i === 0) {
                        rowHtml += `<td rowspan="${millCount}">
                            <div class="action-buttons">
                                <button type="button" class="action-btn btn-update" title="Edit" data-userid="${userId}" data-regioncode="${userData.regionCode}" data-mills='${millIds}'>
                                    <i class="fas fa-edit"></i>
                                </button>
                                <button type="button" class="action-btn btn-delete" title="Delete" data-userid="${userId}">
                                    <i class="fas fa-trash-alt"></i>
                                </button>
                            </div>
                        </td>`;
                    }
                    rowHtml += '</tr>';
                    tableBody.append(rowHtml);
                }
            }
        });
    }

    function populateDropdown(selector, data, placeholder) {
        const currentVal = $(selector).val();
        $(selector).empty().append(`<option value="">${placeholder}</option>`);
        data.forEach(function (item) {
            $(selector).append(new Option(item.text, item.id));
        });
        $(selector).val(currentVal);
    }

    function resetDropdowns(selectors) {
        selectors.forEach(s => $(s).empty().prop('disabled', true).trigger('change.select2'));
    }

    function resetForm() {
        editingUserId = null;
        $('#ddlRegion').val(null).trigger('change');
        $('#ddlUser').prop('disabled', true);
        $('#ddlMills').prop('disabled', true);

        $('#btnUpdate, #btnCancel').hide();
        $('#btnMap').show();
        $('.form-group-button').removeClass('edit-mode');
    }

    function enterEditMode(userId) {
        editingUserId = userId;
        $('#btnMap').hide();
        $('#btnUpdate, #btnCancel').show();
        $('.form-group-button').addClass('edit-mode');
    }
});
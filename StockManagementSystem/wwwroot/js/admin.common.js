﻿// html event tag (onchange/onclick/etc) helpers
function setLocation(url) {
    window.location.href = url;
}

function OpenWindow(query, w, h, scroll) {
    var l = (screen.width - w) / 2;
    var t = (screen.height - h) / 2;

    winprops = 'resizable=1, height=' + h + ',width=' + w + ',top=' + t + ',left=' + l + 'w';
    if (scroll) winprops += ',scrollbars=1';
    var f = window.open(query, "_blank", winprops);
}

// loading gif for long run task
function showThrobber(message) {
    $('.throbber-header').html(message);
    window.setTimeout(function() {
            $(".throbber").show();
        },
        1000);
}

// CSRF (XSRF) security
function addAntiForgeryToken(data) {
    //if the object is undefined, create a new one.
    if (!data) {
        data = {};
    }
    //add token
    var tokenInput = $('input[name=__RequestVerificationToken]');
    if (tokenInput.length) {
        data.__RequestVerificationToken = tokenInput.val();
    }
    return data;
}

function saveUserPreferences(url, name, value) {
    var postData = {
        name: name,
        value: value
    }
    addAntiForgeryToken(postData);
    $.ajax({
        cache: false,
        url: url,
        type: 'post',
        data: postData,
        dataType: 'json',
        error: function(xhr, ajaxOptions, thrownError) {
            alert('Failed to save preferences.');
        }
    });
}

// tab helper
function bindBootstrapTabSelectEvent(tabsId, inputId) {
    $('#' + tabsId + ' > ul li a[data-toggle="tab"]').on('shown.bs.tab',
        function(e) {
            var tabName = $(e.target).attr("data-tab-name");
            $("#" + inputId).val(tabName);
        });
}

// kendoui error
function display_kendoui_grid_error(e) {
    if (e.errors) {
        if ((typeof e.errors) == 'string') {
            //single error
            //display the message
            alert(e.errors);
        } else {
            //array of errors
            //source: http://docs.kendoui.com/getting-started/using-kendo-with/aspnet-mvc/helpers/grid/faq#how-do-i-display-model-state-errors?
            var message = "The following errors have occurred:";
            //create a message containing all errors.
            $.each(e.errors,
                function(key, value) {
                    if (value.errors) {
                        message += "\n";
                        message += value.errors.join("\n");
                    }
                });
            //display the message
            alert(message);
        }
        //ignore empty error
    } else if (e.errorThrown) {
        alert('Error happened');
    }
}

//nested settings
function toggleNestedSetting(parentSettingName, parentFormGroupId) {
    if ($('input[name="' + parentSettingName + '"]').is(':checked')) {
        $('#' + parentFormGroupId).addClass('opened');
    } else {
        $('#' + parentFormGroupId).removeClass('opened');
    }
}

function parentSettingClick(e) {
    toggleNestedSetting(e.data.parentSettingName, e.data.parentFormGroupId);
}

function initNestedSetting(parentSettingName, parentSettingId, nestedSettingId) {
    var parentFormGroup = $('input[name="' + parentSettingName + '"]').closest('.form-group');
    var parentFormGroupId = $(parentFormGroup).attr('id');
    if (!parentFormGroupId) {
        parentFormGroupId = parentSettingId;
    }
    $(parentFormGroup).addClass('parent-setting').attr('id', parentFormGroupId);
    if ($('#' + nestedSettingId + ' .form-group').length ==
        $('#' + nestedSettingId + ' .form-group.advanced-setting').length) {
        $('#' + parentFormGroupId).addClass('parent-setting-advanced');
    }

    //$(document).on('click', 'input[name="' + parentSettingName + '"]', toggleNestedSetting(parentSettingName, parentFormGroupId));
    $('input[name="' + parentSettingName + '"]').click(
        { parentSettingName: parentSettingName, parentFormGroupId: parentFormGroupId },
        parentSettingClick);
    toggleNestedSetting(parentSettingName, parentFormGroupId);
}

//scroll to top
(function($) {
    $.fn.backTop = function() {
        var backBtn = this;

        var position = 300;
        var speed = 900;

        $(document).scroll(function() {
            var pos = $(window).scrollTop();

            if (pos >= position) {
                backBtn.fadeIn(speed);
            } else {
                backBtn.fadeOut(speed);
            }
        });

        backBtn.click(function() {
            $("html, body").animate({ scrollTop: 0 }, 900);
        });
    }
}(jQuery));

// Ajax activity indicator bound to ajax start/stop document events
$(document).ajaxStart(function() {
    $('#ajaxBusy').show();
}).ajaxStop(function() {
    $('#ajaxBusy').hide();
});

$(document).ready(function() {
    $('.multi-tenant-override-option').each(function(k, v) {
        checkOverriddenTenantValue(v, $(v).attr('data-for-input-selector'));
    });

    // intercept all events of pressing the Enter button in the search bar
    $("div.panel-search").keypress(function(event) {
        if (event.which == 13 || event.keyCode == 13) {
            $("button.btn-search").click();
            return false;
        }
    });

    //pressing Enter in the table should not lead to any action
    $("div[id$='-grid']").keypress(function(event) {
        if (event.which == 13 || event.keyCode == 13) {
            return false;
        }
    });
});

function checkAllOverriddenTenantValue(item) {
    $('.multi-tenant-override-option').each(function(k, v) {
        $(v).attr('checked', item.checked);
        checkOverriddenTenantValue(v, $(v).attr('data-for-input-selector'));
    });
}

function checkOverriddenTenantValue(obj, selector) {
    var elementsArray = selector.split(",");
    if (!$(obj).is(':checked')) {
        $(selector).attr('disabled', true);
        //Kendo UI elements are enabled/disabled some other way
        $.each(elementsArray,
            function(key, value) {
                var kenoduiElement = $(value).data("kendoNumericTextBox") || $(value).data("kendoMultiSelect");
                if (kenoduiElement !== undefined && kenoduiElement !== null) {
                    kenoduiElement.enable(false);
                }
            });
    } else {
        $(selector).removeAttr('disabled');
        //Kendo UI elements are enabled/disabled some other way
        $.each(elementsArray,
            function(key, value) {
                var kenoduiElement = $(value).data("kendoNumericTextBox") || $(value).data("kendoMultiSelect");
                if (kenoduiElement !== undefined && kenoduiElement !== null) {
                    kenoduiElement.enable();
                }
            });
    };
}

//no-tabs solution
$(document).ready(function() {
    $(".panel.collapsible-panel >.panel-heading").click(WrapAndSaveBlockData);
});

function WrapAndSaveBlockData() {
    $(this).parents(".panel").find(">.panel-container").slideToggle();

    var icon = $(this).find("i.toggle-icon");
    if ($(this).hasClass("opened")) {
        icon.removeClass("fa-minus");
        icon.addClass("fa-plus");
        saveUserPreferences(rootAppPath + 'common/savepreference', $(this).attr("data-hideAttribute"), true);
    } else {
        icon.addClass("fa-minus");
        icon.removeClass("fa-plus");
        saveUserPreferences(rootAppPath + 'common/savepreference', $(this).attr("data-hideAttribute"), false);
    }

    $(this).toggleClass("opened");
}
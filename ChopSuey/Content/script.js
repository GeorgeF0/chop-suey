// Clipboard

// http://stackoverflow.com/questions/22581345/click-button-copy-to-clipboard-using-jquery
// Jesus Christ JavaScript...
function copyToClipboard(text) {
    var $temp = $("<textarea>");
    $("body").append($temp);
    $temp.val(text).select();
    document.execCommand("copy");
    $temp.remove();
}

function copyStateToClipboard(element) {
    copyToClipboard(element.state);
}

// Knockout and state

ko.bindingHandlers.codemirror = {
    init: function (element, valueAccessor) {
        var value = ko.unwrap(valueAccessor());
        var editor = CodeMirror(element, { value: value, mode: { name: "javascript", json: true }, readOnly: true });
        element.editor = editor;
    },
    update: function (element, valueAccessor) {
        var observedValue = ko.unwrap(valueAccessor());
        if (element.editor) {
            element.editor.setValue(observedValue);
            element.editor.refresh();
        }
    }
};

function Query() {
    this.streak = ko.observable("");
    this.init = ko.observable("");
    this.aggregate = ko.observable("");
    this.description = ko.observable("");
    this.lastQueryStatus = ko.observable("none");
    this.lastError = ko.observable("none");

    this.alertClassIsInfo = ko.computed(() => this.lastQueryStatus() === "none");
    this.alertClassIsDanger = ko.computed(() => this.lastQueryStatus() === "error");
    this.alertClassIsSuccess = ko.computed(() => this.lastQueryStatus() === "success");

    this.alertStrongText = ko.computed(() => {
        switch (this.lastQueryStatus()) {
            case "success":
                return "Success!";
            case "error":
                return "Error: ";
            default:
                return "Info: ";
        }
    });

    this.alertDetails = ko.computed(() => {
        switch (this.lastQueryStatus()) {
            case "success":
                return "";
            case "error":
                return this.lastError();
            default:
                return "Add a query below";
        }
    });
}

function AppViewModel() {
    this.query = new Query();
    this.running = ko.observableArray([]);
}

var viewModel = new AppViewModel();
ko.applyBindings(viewModel);

// Functions

function createQuery() {
    $.post("api/query", {
        streak: viewModel.query.streak(),
        init: viewModel.query.init(),
        aggregate: viewModel.query.aggregate(),
        description: viewModel.query.description()
    })
        .done(() => { viewModel.query.lastQueryStatus("success"); })
        .fail(error => { viewModel.query.lastQueryStatus("error"); viewModel.query.lastError(error.responseText); });
}

function updateRunningQueries() {
    $.get("api/query")
        .done(data => {
            viewModel.running(data);
            setTimeout(updateRunningQueries, 1000);
        })
        .fail(() => setTimeout(updateRunningQueries, 1000));
}

// Setup

updateRunningQueries();
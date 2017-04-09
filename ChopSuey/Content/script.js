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

// Knockout and state

//http://stackoverflow.com/questions/16875773/bootstraps-tooltip-not-working-with-knockout-bindings-w-fiddle 
ko.bindingHandlers.tooltip = {
    init: function (element, valueAccessor) {
        var local = ko.utils.unwrapObservable(valueAccessor());
        var options = {};

        ko.utils.extend(options, ko.bindingHandlers.tooltip.options);
        ko.utils.extend(options, local);

        $(element).tooltip(options);

        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            $(element).tooltip("destroy");
        });
    },
    options: {
        placement: "top",
        trigger: "hover",
        template: '<div class="tooltip" role="tooltip"><div class="tooltip-arrow"></div><div class="tooltip-inner" style="white-space: pre; text-align: left; max-width: none"></div></div>'
    }
}; 

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

function RunningQuery(data) {
    ko.mapping.fromJS(data, {}, this);

    this.info = ko.computed(function () {
        return `Streak:\n${this.query.streak()}\n\nInit:\n${this.query.init()}\n\nAggregate:\n${this.query.aggregate()}`;
    }, this);
}

function AppViewModel() {
    this.query = new Query();
    this.running = ko.mapping.fromJS([],
        {
            key: function (data) {
                return ko.utils.unwrapObservable(data.query.id);
            },
            create: function (options) {
                return new RunningQuery(options.data);
            }
        });
}

var viewModel = new AppViewModel();
ko.applyBindings(viewModel);

// Functions

function copyQueryToQueryInput(element) {
    viewModel.query.streak(element.query.streak());
    viewModel.query.init(element.query.init());
    viewModel.query.aggregate(element.query.aggregate());
    viewModel.query.description(element.query.description());
}

function copyStateToClipboard(element) {
    copyToClipboard(element.state());
}

function deleteQuery(element) {
    $.ajax({
        url: `api/query/${element.query.id()}`,
        type: "DELETE"
    });
}

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
            ko.mapping.fromJS(data, viewModel.running);
            setTimeout(updateRunningQueries, 1000);
        })
        .fail(() => setTimeout(updateRunningQueries, 1000));
}

// Setup

updateRunningQueries();
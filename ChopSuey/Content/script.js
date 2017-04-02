//http://jsfiddle.net/unLSJ/

if (!library) var library = {};

library.json = {
    replacer: function (match, pIndent, pKey, pVal, pEnd) {
        const key = "<span class=json-key>";
        const val = "<span class=json-value>";
        const str = "<span class=json-string>";
        var r = pIndent || "";
        if (pKey)
            r = r + key + pKey.replace(/[": ]/g, "") + "</span>: ";
        if (pVal)
            r = r + (pVal[0] === '"' ? str : val) + pVal + "</span>";
        return r + (pEnd || "");
    },
    prettyPrint: function (obj) {
        const jsonLine = /^( *)("[\w]+": )?("[^"]*"|[\w.+-]*)?([,[{])?$/mg;
        return JSON.stringify(obj, null, 3)
            .replace(/&/g, "&amp;").replace(/\\"/g, "&quot;")
            .replace(/</g, "&lt;").replace(/>/g, "&gt;")
            .replace(jsonLine, library.json.replacer);
    }
};

// Knockout and state

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

// Setup

setInterval(() => {
    $.get("api/query")
        .done(data => viewModel.running(data));
}, 1000);
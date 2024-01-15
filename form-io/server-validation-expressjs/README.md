Inspired from:
    Issue => https://github.com/formio/formio.js/issues/5245
    Referece file => https://github.com/formio/formio/blob/master/src/resources/Validator.js

An api created with Expressjs. 
It gets formio struct and data for validation with validation class which is in formio library

Run app
    node .\src\app.js
Dockerize app
    docker compose up --build

Validation Test Request
http://localhost:3000/
{
    "formDefinition": {
        "components": [
            {
                "label": "Text Field",
                "applyMaskOn": "change",
                "tableView": true,
                "validate": {
                    "required": true
                },
                "key": "textField",
                "type": "textfield",
                "input": true
            },
            {
                "type": "button",
                "label": "Submit",
                "key": "submit",
                "disableOnInvalid": true,
                "input": true,
                "tableView": false
            }
        ]
    },
    "submittedData": {
        "data": {
            "textField": "1",
            "submit": false
        }
    }
}

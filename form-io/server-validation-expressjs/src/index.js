const Validator = require('./Validator');
const formik ={"components": [
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
  ]};
  var submiData ={
    "data": {
      "textField": "",
      "submit": false
    }
  };
const validator = new Validator(formik, submiData);

validator.validate(submiData, (err, data, visibleComponents) => {
    console.log(submiData);
    console.log(err);
    console.log(data);
    


  });
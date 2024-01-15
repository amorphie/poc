const express = require('express');
const app = express();
const port = 3000;
app.use(express.json())
const Validator = require('./Validator');
app.get('/', (req, res) => {
    res.send("This is get");
});

app.post('/', (req, res, next) => {
    // const formDefinition ={"components": [
    //     {
    //       "label": "Text Field",
    //       "applyMaskOn": "change",
    //       "tableView": true,
    //       "validate": {
    //         "required": true
    //       },
    //       "key": "textField",
    //       "type": "textfield",
    //       "input": true
    //     },
    //     {
    //       "type": "button",
    //       "label": "Submit",
    //       "key": "submit",
    //       "disableOnInvalid": true,
    //       "input": true,
    //       "tableView": false
    //     }
    //   ]};
    //   var submittedData ={
    //     "data": {
    //       "textField": "12",
    //       "submit": false
    //     }
    //   };
    console.log("request formData", req.body.formDefinition);
    console.log("request submittedData", req.body.submittedData);
    var formDefinition =req.body.formDefinition;
    var submittedData =req.body.submittedData;
    const validator = new Validator(formDefinition, submittedData);
    var error ={};
    var data={};
    validator.validate(submittedData, (err, data, visibleComponents) => {
        console.log("submittedData ",submittedData);
        console.log("error ",err);
        //console.log("data ",data);
         error=err;
         res.send(error);
    
    
      });
  
})

app.listen(port, () => {
  console.log(`Example app listening on port ${port}`)
})
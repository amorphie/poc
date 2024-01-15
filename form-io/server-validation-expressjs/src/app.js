const express = require('express');
const app = express();
const port = 3000;
app.use(express.json())
const Validator = require('./Validator');
app.get('/', (req, res) => {
    res.send("This is get");
});

app.post('/', (req, res, next) => {

    //console.log("request formData", req.body.formDefinition);
    //console.log("request submittedData", req.body.submittedData);
    var formDefinition =req.body.formDefinition;
    var submittedData =req.body.submittedData;
    const validator = new Validator(formDefinition, submittedData);
    //var error ={};

    validator.validate(submittedData, (err, data, visibleComponents) => {
        //console.log("submittedData ",submittedData);
        //console.log("error ",err);
        //console.log("data ",data);
         //error=err;
         res.send(err);
    
    
      });
  
})

app.listen(port, () => {
  console.log(`Formio validation app listening on port ${port}`)
})
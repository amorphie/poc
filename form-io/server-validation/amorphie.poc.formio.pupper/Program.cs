using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();

app.MapPost("/validate-formio", async () =>
{
    //<link rel=stylesheet href=https://stackpath.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css>
    //<link rel=stylesheet href=https://stackpath.bootstrapcdn.com/bootstrap/4.1.3/css/bootstrap.min.css>
    //<link rel= stylesheet href= https://cdn.form.io/formiojs/formio.full.min.css>

    string htmlContent = @"
  <html>
  <head>
      <script src=https://cdn.form.io/formiojs/formio.full.min.js></script>
  </head>
  <body>
      <div id=formio style=padding: 40px;></div>
      <script>
      var fr;
      Formio.createForm(document.getElementById('formio'),{form.Data})
          .then(
              function (form) {
                  fr = form;
              });

          function onButtonClick() {
              return fr.checkValidity();
          }

          </script>
  </body>
  </html>
  ";

    string formid = "Formio.createForm(document.getElementById('formio'), {\n    \"display\": \"form\",\n    \"components\": [\n        {\n            \"label\": \"Text Field\",\n            \"applyMaskOn\": \"change\",\n            \"tableView\": true,\n            \"validate\": {\n                \"required\": true,\n                \"minWords\": 5,\n                \"maxWords\": 15\n            },\n            \"key\": \"textField\",\n            \"type\": \"textfield\",\n            \"input\": true\n        }\n    ]\n}).then(function(form) {  form.on('submit', function(submission) {    console.log(submission);  });});";

    htmlContent = htmlContent.Replace("{form.Data}", formid);
    var options = new LaunchOptions
    {
        Headless = true,
        Args = new string[] { "--no-sandbox", "'--disable-web-security'" }// Run in headless mode (no visible browser window)
    };

    var browser = await Puppeteer.LaunchAsync(options);
    try
    {


        using (var page = await browser.NewPageAsync())
        {
            page.Request += (sender, e) =>
            {
                Console.WriteLine($"Request: {e.Request.Method} {e.Request.Url}");
                foreach (var header in e.Request.Headers)
                {
                    Console.WriteLine($"{header.Key}: {header.Value}");
                }
            };
            page.Error += (sender, e) =>
            {
                Console.WriteLine($"Error: {e.Error}");
            };
            await page.GoToAsync("http://localhost/form-io-test.html?v=1", new NavigationOptions() { WaitUntil = new[] { WaitUntilNavigation.Networkidle2 } });

            // await page.SetContentAsync(htmlContent, new NavigationOptions { WaitUntil = new[] { WaitUntilNavigation.Networkidle2 } });


            var toCheckIsFormLoaded = await page.WaitForSelectorAsync("#formio");

            var VerifyAllPageLoaded = await page.WaitForFunctionAsync("()=> document.readyState === 'complete'");
            var sss = await page.WaitForExpressionAsync("dummy()");
            var result = await page.WaitForExpressionAsync("onButtonClick()");

            //var buttonClickMe = await page.WaitForSelectorAsync("#clickMe");
            // await buttonClickMe.ClickAsync();
        }
        Console.WriteLine("JavaScript code executed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error occurred: " + ex.Message);
        // Handle the error gracefully or take appropriate action here
    }
    finally
    {
        await browser.CloseAsync();
    }




})
.WithName("validate-formio")
.WithOpenApi();

app.Run();


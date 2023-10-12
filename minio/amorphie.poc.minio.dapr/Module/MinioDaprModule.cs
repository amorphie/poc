﻿
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Diagnostics.Metrics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Web;

public static class MinioDaprModule
{
    public static void AddRoutes(WebApplication app)
    {

        app.MapGet("/dapr/getFileList", async ValueTask<IResult>(
            [FromServices] DaprClient daprClient) =>
        {
            /*
               var query = new Dictionary<string, object>
               {
                   ["maxResults"] = 10,
                   ["prefix"] = "file",
                   ["marker"] = "hvlcCQFSOD5TD",
                   ["delimiter"] = "i0FvxAn2EOEL6"
               };

               string queryString = JsonSerializer.Serialize(query);
            */
            string queryString = "{\"prefix\":\"folder\"}";

            var data = new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(queryString));

            var response = await daprClient.InvokeBindingAsync(new BindingRequest("minio-bind", "list")
            {
                Data = data
            });

            var str = System.Text.Encoding.Default.GetString(response.Data.ToArray());


            return Results.Ok(str);
        });


        app.MapGet("/dapr/getFile/{fileName}", async ValueTask<IResult>(
            [FromServices] DaprClient daprClient,
            [FromRoute] string fileName) =>
        {
            var response = await daprClient.InvokeBindingAsync(new BindingRequest("minio-bind", "get")
            {
                Metadata = { ["key"] = fileName }
            });

            return Results.File(response.Data.ToArray(), GetContentType(fileName), fileName);
        });
        string GetContentType(string fileName)
        {
            if (fileName.Contains(".jpg"))
            {
                return "image/jpg";
            }
            else if (fileName.Contains(".jpeg"))
            {
                return "image/jpeg";
            }
            else if (fileName.Contains(".png"))
            {
                return "image/png";
            }
            else if (fileName.Contains(".gif"))
            {
                return "image/gif";
            }
            else if (fileName.Contains(".pdf"))
            {
                return "application/pdf";
            }
            else
            {
                return "application/octet-stream";
            }
        }


        app.MapPost("/dapr/putFile", async ValueTask<IResult>(
            [FromServices] DaprClient daprClient,
            string fileName) =>
        {

            byte[] data = System.Text.Encoding.UTF8.GetBytes("hello world");
            var response = await daprClient.InvokeBindingAsync(new BindingRequest("minio-bind", "create")
            {
                Data = data,
                Metadata = { ["key"] = fileName, ["presignTTL"] = "1m", ["tags"] = "tag1,tag3" } // TODO : TTL and tag not working
            });


            var str = System.Text.Encoding.Default.GetString(response.Data.ToArray());


            Console.WriteLine(HttpUtility.HtmlDecode(str));

            return Results.Ok(str);
        });

        app.MapDelete("/dapr/{fileName}", async ValueTask<IResult>(
            [FromServices] DaprClient daprClient,
            [FromRoute] string fileName) =>
        {
            var response = await daprClient.InvokeBindingAsync(new BindingRequest("minio-bind", "delete")
            {
                Metadata = { ["key"] = fileName }
            });

            return Results.Ok();
        });
    }
}



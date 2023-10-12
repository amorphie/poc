using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Net.Security;
using System.Text;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.Encryption;
using Minio.DataModel.Result;
using Minio.DataModel.Tags;
using Minio.Exceptions;




var endpoint = "localhost:9000";
var accessKey = "minioadmin";
var secretKey = "minioadmin";


IMinioClient minioClient = new MinioClient()
                            .WithEndpoint(endpoint)
                            .WithCredentials(accessKey, secretKey)
                            .WithSSL(false)
                            .Build();

string bucketName = "from-api-bucket-test";

try
{




    bool isExist = await IsBucketExist(bucketName);
    Console.WriteLine(isExist);

    if (!isExist)
        await CreateBucket(bucketName);

    ListAllMyBucketsResult listAllMyBucketsResult = await GetBucketList();
    foreach (Bucket bucket in listAllMyBucketsResult.Buckets)
        Console.WriteLine($"Name:{bucket.Name} - Create Date:{bucket.CreationDate}");







    //    await SetBucketVersioning(bucketName);

    //    await SetBucketEncryption(bucketName);

    //    await GetBucketEncryption(bucketName);

    //    await SetBucketTag(bucketName, "tag1", "tag2");

    await UploadFile();
}
catch (Exception ex)
{

}
/*
 
await RemoveBucketEncryption(bucketName);
await DeleteBucket(bucketName);

*/
Console.ReadLine();

async Task<bool> IsBucketExist(string bucketName)
{
    var args = new BucketExistsArgs()
        .WithBucket(bucketName);

    return await minioClient.BucketExistsAsync(args);
}
async Task CreateBucket(string bucketName)
{
    var args = new MakeBucketArgs()
        .WithBucket(bucketName);

    await minioClient.MakeBucketAsync(args);
}
async Task<ListAllMyBucketsResult> GetBucketList()
{
    return await minioClient.ListBucketsAsync();
}
async Task DeleteBucket(string bucketName)
{
    var args = new RemoveBucketArgs()
        .WithBucket(bucketName);

    await minioClient.RemoveBucketAsync(args);
}
async Task SetBucketVersioning(string bucketName)
{
    var args = new SetVersioningArgs()
        .WithBucket(bucketName)
         .WithVersioningEnabled();

    await minioClient.SetVersioningAsync(args);
}
async Task SetBucketEncryption(string bucketName)
{
    ServerSideEncryptionConfiguration configuration = new ServerSideEncryptionConfiguration();

    var args = new SetBucketEncryptionArgs()
        .WithBucket(bucketName)
         .WithEncryptionConfig(configuration);

    await minioClient.SetBucketEncryptionAsync(args);
}
async Task GetBucketEncryption(string bucketName)
{
    var args = new GetBucketEncryptionArgs()
                  .WithBucket(bucketName);
    ServerSideEncryptionConfiguration config = await minioClient.GetBucketEncryptionAsync(args);
}
async Task RemoveBucketEncryption(string bucketName)
{
    var args = new RemoveBucketEncryptionArgs()
                   .WithBucket(bucketName);
    await minioClient.RemoveBucketEncryptionAsync(args);
}
async Task SetBucketTag(string bucketName, params string[] tags)
{
    Tagging tagging = new Tagging();
    tagging.TaggingSet = new TagSet();

    foreach (var tag in tags)
    {
        tagging.TaggingSet.Tag.Add(new Tag() { Key = tag, Value = tag });
    }

    SetBucketTagsArgs args = new SetBucketTagsArgs()
    .WithBucket(bucketName)
                                    .WithTagging(tagging);
    await minioClient.SetBucketTagsAsync(args);
}

async Task UploadFile()
{
    byte[] data = System.Text.Encoding.UTF8.GetBytes("hello world");
    MemoryStream stream = new MemoryStream(data);

    var objectName = "golden-oldies.txt";
    var putObjectArgs = new PutObjectArgs()
                     .WithBucket(bucketName)
                     .WithObject(objectName)

                     // .WithFileName(filePath)
                     .WithStreamData(stream)
                     .WithObjectSize(stream.Length)
                     .WithContentType("application/octet-stream");




    await minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
    Console.WriteLine("Successfully uploaded " + objectName);
}
// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

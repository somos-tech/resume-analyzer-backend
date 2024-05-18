using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace SomosTech.ResumeExtractor;

public class ResumeUploaderService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public ResumeUploaderService(string connectionString, string containerName)
    {
        _blobServiceClient = new BlobServiceClient(connectionString);
        _containerName = containerName;
    }

    public async Task<string> UploadBase64ResumeAsync(string base64String, string fileName, string extension)
    {
        fileName = $"{fileName}_" + GenerateMD5Hash(fileName + DateTime.UtcNow).Substring(5) + $".{extension}";

        if (string.IsNullOrWhiteSpace(base64String))
        {
            throw new ArgumentException("Base64 string cannot be null or empty.", nameof(base64String));
        }

        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        // Ensure the container exists
        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient(fileName);

        // Convert base64 string to byte array
        byte[] fileBytes = Convert.FromBase64String(base64String);

        // Upload the byte array as a blob
        using (var stream = new MemoryStream(fileBytes))
        {
            await blobClient.UploadAsync(stream, overwrite: true);
        }

        // Return the URI of the uploaded blob
        return blobClient.Uri.ToString();
    }

    public static string GenerateMD5Hash(string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to a hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}

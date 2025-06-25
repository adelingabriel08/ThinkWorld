using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using ThinkWorld.Services.Options;
using ThinkWorld.Services.DataContext;
using System.Threading.Tasks;
using ThinkWorld.Domain.Aggregates;

namespace ThinkWorld.PII.Processor.Functions
{
    public class PiiChangeFeedProcessor
    {
        private readonly PiiDatabaseOptions _options;
        private readonly CosmosDbContext _dbContext;
        public PiiChangeFeedProcessor(IOptions<PiiDatabaseOptions> options, CosmosDbContext dbContext)
        {
            _options = options.Value;
            _dbContext = dbContext;
            // Set environment variables for CosmosDBTrigger
            Environment.SetEnvironmentVariable("PII_COSMOS_DB_NAME", _options.DatabaseName);
            Environment.SetEnvironmentVariable("PII_COSMOS_CONTAINER_NAME", "your-comment-container-name"); // or _options.ContainerName if you add it
            var connection = _options.UseManagedIdentity ? _options.Endpoint : $"AccountEndpoint={_options.Endpoint};AccountKey={_options.EndpointKey};";
            Environment.SetEnvironmentVariable("PII_COSMOS_CONNECTION", connection);
        }

        [FunctionName("PiiChangeFeedProcessor")]
        public async Task Run(
            [CosmosDBTrigger(
                databaseName: "%PII_COSMOS_DB_NAME%", // fallback to env var
                containerName: "%PII_COSMOS_CONTAINER_NAME%", // fallback to env var
                Connection = "%PII_COSMOS_CONNECTION%", // This should be set to either the endpoint or the connection string in app settings
                LeaseContainerName = "leases",
                CreateLeaseContainerIfNotExists = true)]
            IReadOnlyList<PostComment> input,
            ILogger log)
        {
            log.LogInformation($"UseManagedIdentity: {_options.UseManagedIdentity}");
            log.LogInformation($"Endpoint: {_options.Endpoint}");
            log.LogInformation($"DatabaseName: {_options.DatabaseName}");
            if (!_options.UseManagedIdentity && string.IsNullOrEmpty(_options.EndpointKey))
            {
                log.LogWarning("EndpointKey is required when not using managed identity.");
            }
            if (input != null && input.Count > 0)
            {
                foreach (var comment in input)
                {
                    if (comment != null && comment.Id != Guid.Empty && comment.PostId != Guid.Empty && !string.IsNullOrEmpty(comment.CreatedBy))
                    {
                        // Fetch the post for this comment
                        var post = await _dbContext.Posts.FindAsync(comment.PostId);
                        if (post != null && !string.IsNullOrEmpty(post.CreatedBy))
                        {
                            var postCreatorEmail = post.CreatedBy;
                            var subject = $"New comment on your post ({post.Id})";
                            var body = $"A new comment (ID: {comment.Id}) was added by {comment.CreatedBy} at {comment.CreatedAt}.";
                            Console.WriteLine($"[FAKE EMAIL] To: {postCreatorEmail}\nSubject: {subject}\nBody: {body}\n");
                            log.LogInformation($"Notified post creator {postCreatorEmail} about new comment {comment.Id}.");
                        }
                        else
                        {
                            log.LogWarning($"Post {comment.PostId} not found or has no creator.");
                        }
                    }
                }
                log.LogInformation($"PII Change feed: {input.Count} document(s) processed");
            }
        }
    }
}

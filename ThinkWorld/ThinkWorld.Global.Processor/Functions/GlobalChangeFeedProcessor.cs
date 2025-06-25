using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ThinkWorld.Domain.Aggregates;
using ThinkWorld.Services.Options;
using ThinkWorld.Services.DataContext;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ThinkWorld.Global.Processor.Functions
{
    public class GlobalChangeFeedProcessor
    {
        private readonly GlobalDatabaseOptions _options;
        private readonly CosmosDbContext _dbContext;
        public GlobalChangeFeedProcessor(IOptions<GlobalDatabaseOptions> options, CosmosDbContext dbContext)
        {
            _options = options.Value;
            _dbContext = dbContext;
            // Set environment variables for CosmosDBTrigger
            Environment.SetEnvironmentVariable("GLOBAL_COSMOS_DB_NAME", _options.DatabaseName);
            // You may need to set the container name to the one you want to listen to
            Environment.SetEnvironmentVariable("GLOBAL_COSMOS_CONTAINER_NAME", "your-container-name"); // or _options.ContainerName if you add it
            // For managed identity, set to endpoint; for key-based, set to connection string
            var connection = _options.UseManagedIdentity ? _options.Endpoint : $"AccountEndpoint={_options.Endpoint};AccountKey={_options.EndpointKey};";
            Environment.SetEnvironmentVariable("GLOBAL_COSMOS_CONNECTION", connection);
        }

        [FunctionName("GlobalChangeFeedProcessor")]
        public async Task Run(
            [CosmosDBTrigger(
                databaseName: "%GLOBAL_COSMOS_DB_NAME%", // fallback to env var
                containerName: "%GLOBAL_COSMOS_CONTAINER_NAME%", // fallback to env var
                Connection = "%GLOBAL_COSMOS_CONNECTION%", // This should be set to either the endpoint or the connection string in app settings
                LeaseContainerName = "leases",
                CreateLeaseContainerIfNotExists = true)]
            IReadOnlyList<CommunityPost> input,
            ILogger log)
        {
            // The binding will use managed identity if the connection string setting is just the endpoint URI and UseManagedIdentity is true.
            // If UseManagedIdentity is false, the connection string should include the key.
            log.LogInformation($"UseManagedIdentity: {_options.UseManagedIdentity}");
            log.LogInformation($"Endpoint: {_options.Endpoint}");
            log.LogInformation($"DatabaseName: {_options.DatabaseName}");
            if (!_options.UseManagedIdentity && string.IsNullOrEmpty(_options.EndpointKey))
            {
                log.LogWarning("EndpointKey is required when not using managed identity.");
            }
            if (input != null && input.Count > 0)
            {
                foreach (var post in input)
                {
                    // For demo: consider every document as a new post
                    if (post != null && post.Id != Guid.Empty && post.CommunityId != Guid.Empty && !string.IsNullOrEmpty(post.CreatedBy))
                    {
                        // Fetch the community from CosmosDbContext
                        var community = await _dbContext.Communities.FirstOrDefaultAsync(c => c.Id == post.CommunityId);
                        if (community != null && !string.IsNullOrEmpty(community.CreatedBy))
                        {
                            var communityCreatorEmail = community.CreatedBy;
                            var subject = $"New post in your community ({post.CommunityId})";
                            var body = $"A new post (ID: {post.Id}) was added by {post.CreatedBy} at {post.CreatedAt}.";
                            // Fake sending email by writing to the console
                            Console.WriteLine($"[FAKE EMAIL] To: {communityCreatorEmail}\nSubject: {subject}\nBody: {body}\n");
                            log.LogInformation($"Notified community creator {communityCreatorEmail} about new post {post.Id}.");
                        }
                        else
                        {
                            log.LogWarning($"Community {post.CommunityId} not found or has no creator.");
                        }
                    }
                }
                log.LogInformation($"Global Change feed: {input.Count} document(s) processed");
            }
        }
    }
}

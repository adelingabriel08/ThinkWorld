using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var piiApi = builder.AddProject<ThinkWorld_PII_API>("ThinkWorld-PII-API");
var globalApi = builder.AddProject<ThinkWorld_Global_API>("ThinkWorld-Global-API");
var routerApi = builder.AddProject<ThinkWorld_PII_Router_API>("ThinkWorld-Router-API");


builder.Build().Run();
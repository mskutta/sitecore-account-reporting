using Newtonsoft.Json;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Rules;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace Skutta.AccountReporting.Commands
{
    public class RedeployFilterCommand : Command
    {
        public override CommandState QueryState(CommandContext context)
        {
            var item = context.Items.FirstOrDefault();

            // Check if template is "/sitecore/templates/System/Experience Analytics/Filter"
            if (item != null && item.TemplateID == new ID("{07003270-D56D-42C2-90E1-99125E0ED1AF}") && !TemplateManager.IsTemplate(item))
                return CommandState.Enabled;

            return CommandState.Hidden;
        }

        public override void Execute(CommandContext context)
        {
            var item = context.Items.FirstOrDefault();
            if (item == null)
                return;

            var parameters = new System.Collections.Specialized.NameValueCollection();
            parameters["itemUri"] = item.Uri.ToString();
            Context.ClientPage.Start(this, "Run", parameters);
        }

        protected void Run(ClientPipelineArgs args)
        {
            if (Context.ClientPage.Modified)
            {
                var title = !string.IsNullOrWhiteSpace(args.Parameters["title"]) ? args.Parameters["title"] : "Redeploy";
                SheerResponse.Alert(string.Format("You must save your edits to this item before attempting to {0}.", title));
                args.AbortPipeline();
                return;
            }

            var itemUri = new ItemUri(args.Parameters["itemUri"]);
            var filterItem = Database.GetItem(itemUri);

            if (filterItem == null)
            {
                Context.ClientPage.ClientResponse.Alert(string.Format(@"Filter {0} not found.", itemUri));
                return;
            }

            var ruleField = filterItem.Fields["Rule"];
            if (ruleField == null)
            {
                Context.ClientPage.ClientResponse.Alert(string.Format(@"Rule field not found for {0}.", itemUri));
                return;
            }
            var filter = JsonConvert.SerializeObject(RuleFactory.GetRules<RuleContext>(ruleField), new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });

            var updatedSegments = new List<string>();

            // Find segments using filter.  Crawl, do not use links database as it may be out of sync.
            var dimensions = filterItem.Database.GetItem("{FBF255C0-72A2-4E76-A83D-633B852D82E7}");
            foreach(var folder in dimensions.Children.OfType<Item>())
            {
                foreach(var dimension in folder.Children.OfType<Item>())
                {
                    foreach(var segment in dimension.Children.OfType<Item>())
                    {
                        var filterField = segment.Fields["Filter"];
                        if (filterField == null)
                            continue;
                        if (string.IsNullOrEmpty(filterField.Value))
                            continue;
                        ID filterId;
                        if (!ID.TryParse(filterField.Value, out filterId))
                            continue;
                        if (filterId != filterItem.ID)
                            continue;

                        var workflow = filterItem.Database.WorkflowProvider.GetWorkflow(segment);
                        var workflowState = workflow.GetState(segment);
                        if (!workflowState.FinalState)
                            continue;

                        var command = new SqlCommand { CommandText = "UPDATE dbo.Segments SET [Filter] = @Filter WHERE [SegmentId] = @SegmentId" };
                        command.Parameters.AddWithValue("@Filter", filter);
                        command.Parameters.AddWithValue("@SegmentId", segment.ID.Guid);
                        Factory.GetRetryer().ExecuteNoResult(() =>
                        {
                            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["reporting"].ConnectionString))
                            {
                                connection.Open();
                                command.Connection = connection;
                                command.ExecuteNonQuery();
                            }
                        });

                        // Run the deployment pipeline to deploy the segment.
                        // Sitecore.ExperienceAnalytics.Client.Deployment.DeploySegmentDefinitionProcessor DateTime.UtcNow
                        //var deployDefinitionArgs = new DeployDefinitionArgs(item);
                        //CorePipeline.Run("deployDefinition", deployDefinitionArgs);

                        updatedSegments.Add(segment.Name);
                    }
                }
            }

            if (updatedSegments.Any())
                Context.ClientPage.ClientResponse.Alert(string.Format(@"Redeployed: {0}.", string.Join(",", updatedSegments)));
            else
                Context.ClientPage.ClientResponse.Alert(@"Filter NOT Redeployed. Make sure filter is attached to a segment and the segment is deployed.");
        }

        private static string GetFilter(Item item)
        {
            ID id;
            Field filterField = item.Fields["Filter"];
            if (filterField == null)
                return null;
            if (string.IsNullOrEmpty(filterField.Value))
                return null;
            if (!ID.TryParse(filterField.Value, out id))
                return null;
            var filter = item.Database.GetItem(id);
            if (filter == null)
                return null;
            var ruleField = filter.Fields["Rule"];
            if (ruleField == null)
                return null;
            return JsonConvert.SerializeObject(RuleFactory.GetRules<RuleContext>(ruleField), new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
        }
    }
}

/*****************************************
 * 
 * The Main Code is taken from Sitecore.Kernel.dll and adjusted to support the required customization
 * 
 *****************************************/

 using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Sites;
using Sitecore.Web.UI.Sheer;
using Sitecore.Workflows;
using System.Collections.Specialized;
using System.Linq;

namespace SitecoreFootsteps.Modules.CustomEditorFullPublishOption.CustomCommand
{
    public class CustomFullPublish : Command
    {
        private static string CustomPublishSetting { get; set; }

        public CustomFullPublish()
        {
            CustomPublishSetting = Settings.GetSetting("SitecoreFootsteps.EditorFullPublishOption");
        }

        /// <summary>Executes the command in the specified context.</summary>
        /// <param name="context">The context.</param>
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull((object)context, "context");
            NameValueCollection parameters = new NameValueCollection();

            if (!string.IsNullOrEmpty(CustomPublishSetting) && CustomPublishSetting.ToLower().Equals("disabled"))
            {
                SheerResponse.Alert("Site Full Publishing is Disabled.");
                return;
            }

            if (context.Items != null && context.Items.Length == 1)
            {
                Item obj = context.Items[0];
                parameters["id"] = obj.ID.ToString();
                parameters["language"] = obj.Language.ToString();
                parameters["version"] = obj.Version.ToString();
                parameters["workflow"] = "0";
            }
            Context.ClientPage.Start((object)this, "Run", parameters);
        }

        /// <summary>Queries the state of the command.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The state of the command.</returns>
        public override CommandState QueryState(CommandContext context)
        {
            Assert.ArgumentNotNull((object)context, "context");

            if (string.IsNullOrEmpty(CustomPublishSetting) || !CustomPublishSetting.ToLower().Equals("hidden"))
            {
                return !Settings.Publishing.Enabled ? CommandState.Hidden : CommandState.Enabled;
            }

            return CommandState.Hidden;
        }

        /// <summary>Runs the specified args.</summary>
        /// <param name="args">The args.</param>
        protected static void Run(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull((object)args, "args");
            string parameter1 = args.Parameters["id"];
            if (!string.IsNullOrEmpty(parameter1))
            {
                string parameter2 = args.Parameters["language"];
                string parameter3 = args.Parameters["version"];
                Item obj = Context.ContentDatabase.Items[parameter1, Language.Parse(parameter2), Sitecore.Data.Version.Parse(parameter3)];
                if (obj == null)
                {
                    SheerResponse.Alert("Item not found.");
                    return;
                }
                if (!CustomFullPublish.CheckWorkflow(args, obj))
                    return;
            }
            Sitecore.Shell.Framework.Items.Publish();
        }

        private static bool CheckWorkflow(ClientPipelineArgs args, Item item)
        {
            Assert.ArgumentNotNull((object)args, "args");
            Assert.ArgumentNotNull((object)item, "item");
            if (args.Parameters["workflow"] == "1")
                return true;
            args.Parameters["workflow"] = "1";
            if (args.IsPostBack)
            {
                if (args.Result == "yes")
                {
                    args.IsPostBack = false;
                    return true;
                }
                args.AbortPipeline();
                return false;
            }
            SiteContext site = Factory.GetSite("publisher");
            if (site != null && !site.EnableWorkflow)
                return true;
            IWorkflowProvider workflowProvider = Context.ContentDatabase.WorkflowProvider;
            if (workflowProvider == null || workflowProvider.GetWorkflows().Length <= 0)
                return true;
            IWorkflow workflow = workflowProvider.GetWorkflow(item);
            if (workflow == null)
                return true;
            WorkflowState state = workflow.GetState(item);
            if (state == null || state.FinalState)
                return true;
            args.Parameters["workflow"] = "0";
            if (state.PreviewPublishingTargets.Any<string>())
                return true;
            SheerResponse.Confirm(Translate.Text("The current item \"{0}\" is in the workflow state \"{1}\"\nand will not be published.\n\nAre you sure you want to publish?", (object)item.DisplayName, (object)state.DisplayName));
            args.WaitForPostBack();
            return false;
        }
    }
}

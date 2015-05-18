using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Workflows.Simple;

namespace SitecoreFootsteps.Modules.LangItemSkipAction.Workflow.Actions
{
    /// <summary>
    /// Class to skip a given language items to given workflow state
    /// </summary>
    public class LangItemsAction
    {
        #region Statis Variables

        private static string FieldnameWorkflowLanguages { get { return "Languages To Execute"; } }

        private static string FieldnameWorkflowNextState { get { return "next state"; } } 

        #endregion

        #region Publich Methods

        public void Process(WorkflowPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            ProcessorItem processorItem = args.ProcessorItem;
            if (processorItem == null || args.DataItem == null)
            {
                return;
            }

            Item innerItem = processorItem.InnerItem;

            MultilistField langs = innerItem.Fields[FieldnameWorkflowLanguages];
            Database db = Factory.GetDatabase("master");

            if (langs == null || langs.TargetIDs == null || !langs.TargetIDs.Any() || db == null)
            {
                return;
            }

            ID iD = ID.Null;

            foreach (var lang in langs.TargetIDs)
            {
                Item item = db.GetItem(lang);

                if (item == null)
                {
                    continue;
                } 
                
                if(item.Name.Equals(args.DataItem.Language.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    iD = MainUtil.GetID(innerItem[FieldnameWorkflowNextState], null);
                    break;
                }
            }
            
            if (iD.IsNull)
            {
                return;
            }

            args.NextStateId = iD;
        }

        #endregion
    }
}

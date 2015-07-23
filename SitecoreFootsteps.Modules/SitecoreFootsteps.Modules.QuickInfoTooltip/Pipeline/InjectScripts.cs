using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Sitecore.Pipelines;
using Sitecore.Diagnostics;
using Sitecore.StringExtensions;

namespace SitecoreFootsteps.Modules.QuickInfoTooltip.Pipeline
{
    /// <summary>
    /// Code was taken from following blog post
    /// https://jammykam.wordpress.com/2014/04/24/adding-custom-javascript-and-stylesheets-in-the-content-editor/
    /// </summary>
    public class InjectScripts
    {
        private const string JavascriptTag = "<script src=\"{0}\"></script>";
        private const string StylesheetLinkTag = "<link href=\"{0}\" rel=\"stylesheet\" />";

        public void Process(PipelineArgs args)
        {
            var enabled = Sitecore.Configuration.Settings.GetSetting("SitecoreFootsteps.Modules.QuickInfoTooltip.Enable");

            if (string.IsNullOrEmpty(enabled) || !enabled.Equals("true"))
            {
                return;
            }

            AddControls(JavascriptTag, "SitecoreFootsteps.Modules.QuickInfoTooltip.CustomContentEditorJavascript");
            AddControls(StylesheetLinkTag, "SitecoreFootsteps.Modules.QuickInfoTooltip.CustomContentEditorStylesheets");
        }

        private void AddControls(string resourceTag, string configKey)
        {
            Assert.IsNotNullOrEmpty(configKey, "Content Editor resource config key cannot be null");

            var resources = Sitecore.Configuration.Settings.GetSetting(configKey);

            if (string.IsNullOrEmpty(resources))
                return;

            foreach (var resource in resources.Split('|'))
            {
                Sitecore.Context.Page.Page.Header.Controls.Add((Control)new LiteralControl(resourceTag.FormatWith(resource)));
            }
        }
    }
}
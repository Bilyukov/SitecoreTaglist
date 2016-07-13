using System.Web.UI;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.StringExtensions;

namespace TagField.Pipelines
{
    public class InjectScripts 
    {
        private const string JAVASCRIPT_TAG = "<script src=\"{0}\"></script>";
        private const string STYLESHEET_LINK_TAG = "<link href=\"{0}\" rel=\"stylesheet\" />";

        public void Process(PipelineArgs args)
        {
            AddControls(JAVASCRIPT_TAG, "CustomContentEditorJavascript");
            AddControls(STYLESHEET_LINK_TAG, "CustomContentEditorStylesheets");
        }

        private void AddControls(string resourceTag, string configKey)
        {
            Assert.IsNotNullOrEmpty(configKey, "Content Editor resource config key cannot be null");

            string resources = Sitecore.Configuration.Settings.GetSetting(configKey);

            if (string.IsNullOrEmpty(resources))
                return;

            foreach (var resource in resources.Split('|'))
            {
                Sitecore.Context.Page.Page.Header.Controls.Add(new LiteralControl(resourceTag.FormatWith(resource)));
            }
        }
    }
}
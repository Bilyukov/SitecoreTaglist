using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Text;
using Sitecore.Web.UI.HtmlControls.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Sitecore.Shell.Applications.ContentEditor;


namespace TagField
{
    public sealed class TagField : Sitecore.Web.UI.HtmlControls.Control, IContentField
    {
        private const int MAX_ITEMS_COUNT = 1000;

        private string _itemid;
        private string _source;

        public string ItemId
        {
            get
            {
                return _itemid;
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                _itemid = value;
            }
        }

        public string ItemLanguage
        {
            get
            {
                return StringUtil.GetString(ViewState["ItemLanguage"]);
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                ViewState["ItemLanguage"] = value;
            }
        }

        public string Source
        {
            get
            {
                return _source;
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                _source = value;
            }
        }

        public string GetValue()
        {
            return Value;
        }

        public void SetValue(string value)
        {
            Assert.ArgumentNotNull(value, "value");
            Value = value;
        }

        protected override void DoRender(HtmlTextWriter output)
        {
            var selectedItems = GetSelectedItems();
            var sourceItems = GetSourceItems().Take(MAX_ITEMS_COUNT);

            RenderMultiList(output, selectedItems, sourceItems);

            string script = "<script type=\"text/javascript\">jQuery('.chosen-select').chosen().change(scContent.onTagFieldUpdated);</script>";
            output.Write(script);

            base.DoRender(output);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            string value = Sitecore.Context.ClientPage.ClientRequest.Form[ID + "_value"];

            if (value != null)
            {
                string viewStateValue = GetViewStateString("Value", string.Empty);

                if (viewStateValue != value)
                {
                    SetModified();
                }

                SetViewStateString("Value", value);
            }
        }

        private IEnumerable<Item> GetSelectedItems()
        {
            ListString itemsIds = new ListString(Value);
            return itemsIds.Select(itemId => Sitecore.Context.ContentDatabase.GetItem(itemId)).Where(item => item != null);
        }

        private IEnumerable<Item> GetSourceItems()
        {
            Item current = Sitecore.Context.ContentDatabase.GetItem(ItemId, Language.Parse(ItemLanguage));

            using (new LanguageSwitcher(ItemLanguage))
            {
                var sources = LookupSources.GetItems(current, Source);
                return sources;
            }
        }

        private void SetModified()
        {
            Sitecore.Context.ClientPage.Modified = true;
        }

        private void RenderMultiList(HtmlTextWriter output, IEnumerable<Item> selectedItems, IEnumerable<Item> sourceItems)
        {
            output.Write("<input id=\"" + ID + "_Value\" class=\"scContentControl\" type=\"hidden\" value=\"" + StringUtil.EscapeQuote(Value) + "\" />");

            output.Write("<select id=\"" + ID + "_Select\" multiple class=\"chosen-select\" " + GetControlAttributes() + " >");

            var selectedHash = selectedItems.ToDictionary(k => k.ID);

            foreach (var item in selectedHash)
            {
                output.Write("<option selected value=\"{0}\">{1}</option>", item.Key, item.Value.Name);
            }

            var notSelectedItems = sourceItems.Where(s => !selectedHash.ContainsKey(s.ID));

            foreach (var item in notSelectedItems)
            {
                output.Write("<option value=\"{0}\">{1}</option>", item.ID, item.Name);
            }

            output.Write("</select>");

            output.Write("<script type=\"text/javascript\">jQuery('#" + UniqueID + "_Select').chosen({sitecoreId: '" + UniqueID + "'}); ");
            output.Write("jQuery('#" + UniqueID + "_Select').on('change', scContent.onTagFieldUpdated); ");
            output.Write("jQuery('#" + UniqueID + "_Select').on('chosen:showing_dropdown', scContent.onDropDown); ");
            output.Write("jQuery('#" + UniqueID + "_Select').on('chosen:hiding_dropdown', scContent.onHideDropDown); </script>");
        }

    }
}

// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using Cottle.Documents;
using Cottle.Stores;
using Softeq.NetKit.Services.EmailNotifications.Models;

namespace Softeq.NetKit.Services.EmailNotifications.Abstract
{
    public abstract class BaseEmailNotification<T> : IEmailNotification
    {
        protected BaseEmailNotification(params RecipientDto[] recipients)
        {
            Recipients = recipients;
        }

        public string Subject { get; set; }
        public IEnumerable<RecipientDto> Recipients { get; set; }
        public string Text { get; set; }
	    public string BaseHtmlTemplate { get; set; }
	    public string HtmlTemplate { get; set; }
		public T TemplateModel { get; set; }

	    public virtual string GetHtml()
	    {
		    if (HtmlTemplate == null)
		    {
			    return null;
		    }

		    var document = new SimpleDocument(HtmlTemplate);
		    var store = new BuiltinStore();

		    foreach (var prop in TemplateModel.GetType().GetProperties())
		    {
			    store[prop.Name] = prop.GetValue(TemplateModel).ToString();
		    }

		    var body = document.Render(store);
			var baseDocument = new SimpleDocument(BaseHtmlTemplate);
		    store = new BuiltinStore
		    {
			    ["Body"] = body
		    };
		    var result = baseDocument.Render(store);
			return result;
	    }

        public virtual string FormatSubject() => Subject;
    }
}

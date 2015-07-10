using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NITASA.Services.Security;

namespace NITASA.Views
{
    public abstract class AutofacEnabledViewPage : WebViewPage
    {
        public IAclService aclService { get; set; }
    }

    public abstract class AutofacEnabledViewPage<T> : WebViewPage<T>
    {
        public IAclService aclService { get; set; }
    }
}
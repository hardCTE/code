﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASP
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    using NewLife;
    using NewLife.Cube;
    using NewLife.Reflection;
    
    #line 1 "..\..\Views\EntityList\_Pager1.cshtml"
    using NewLife.Web;
    
    #line default
    #line hidden
    using XCode;
    using XCode.Membership;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/EntityList/_Pager1.cshtml")]
    public partial class _Views_EntityList__Pager1_cshtml : System.Web.Mvc.WebViewPage<dynamic>
    {

#line 7 "..\..\Views\EntityList\_Pager1.cshtml"
public System.Web.WebPages.HelperResult  GetNumericPage(Pager page, int len = 8)
{
#line default
#line hidden
return new System.Web.WebPages.HelperResult(__razor_helper_writer => {

#line 8 "..\..\Views\EntityList\_Pager1.cshtml"
 

    int left = (len - 1) / 2, right = len - left - 1;
    int start = page.PageIndex - left;//开始页数
    int end = page.PageIndex + right;//结束页数
    start = start < 1 ? 1 : start;
    end = end > page.PageCount ? page.PageCount : end;
    if (page.PageIndex <= 1)
    {


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "        <li");

WriteLiteralTo(__razor_helper_writer, " class=\"paginItem current\"");

WriteLiteralTo(__razor_helper_writer, "><a");

WriteLiteralTo(__razor_helper_writer, " href=\"#\"");

WriteLiteralTo(__razor_helper_writer, ">|<</a></li>\r\n");

WriteLiteralTo(__razor_helper_writer, "        <li");

WriteLiteralTo(__razor_helper_writer, " class=\"paginItem current\"");

WriteLiteralTo(__razor_helper_writer, "><a");

WriteLiteralTo(__razor_helper_writer, " href=\"#\"");

WriteLiteralTo(__razor_helper_writer, "><</a></li>\r\n");


#line 19 "..\..\Views\EntityList\_Pager1.cshtml"
    }
    else
    {


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "        <li");

WriteLiteralTo(__razor_helper_writer, " class=\"paginItem\"");

WriteLiteralTo(__razor_helper_writer, "><a");

WriteAttributeTo(__razor_helper_writer, "href", Tuple.Create(" href=\"", 669), Tuple.Create("\"", 701)

#line 22 "..\..\Views\EntityList\_Pager1.cshtml"
, Tuple.Create(Tuple.Create("", 676), Tuple.Create<System.Object, System.Int32>(page.GetPageUrl("首页", 1)

#line default
#line hidden
, 676), false)
);

WriteLiteralTo(__razor_helper_writer, ">|<</a></li>\r\n");

WriteLiteralTo(__razor_helper_writer, "        <li");

WriteLiteralTo(__razor_helper_writer, " class=\"paginItem\"");

WriteLiteralTo(__razor_helper_writer, "><a");

WriteAttributeTo(__razor_helper_writer, "href", Tuple.Create(" href=\"", 748), Tuple.Create("\"", 796)

#line 23 "..\..\Views\EntityList\_Pager1.cshtml"
, Tuple.Create(Tuple.Create("", 755), Tuple.Create<System.Object, System.Int32>(page.GetPageUrl("上一页", page.PageIndex-1)

#line default
#line hidden
, 755), false)
);

WriteLiteralTo(__razor_helper_writer, "><</a></li>\r\n");


#line 24 "..\..\Views\EntityList\_Pager1.cshtml"
    }

    for (int i = start; i <= end; i++)
    {
        if (i == page.PageIndex)
        {


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "            <li");

WriteLiteralTo(__razor_helper_writer, " class=\"paginItem current\"");

WriteLiteralTo(__razor_helper_writer, "><a");

WriteLiteralTo(__razor_helper_writer, " href=\"javascript:;\"");

WriteLiteralTo(__razor_helper_writer, ">");


#line 30 "..\..\Views\EntityList\_Pager1.cshtml"
                                   WriteTo(__razor_helper_writer, i);


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "</a></li>\r\n");


#line 31 "..\..\Views\EntityList\_Pager1.cshtml"
        }
        else
        {


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "            <li");

WriteLiteralTo(__razor_helper_writer, " class=\"paginItem\"");

WriteLiteralTo(__razor_helper_writer, "><a");

WriteAttributeTo(__razor_helper_writer, "href", Tuple.Create(" href=\"", 1061), Tuple.Create("\"", 1101)

#line 34 "..\..\Views\EntityList\_Pager1.cshtml"
, Tuple.Create(Tuple.Create("", 1068), Tuple.Create<System.Object, System.Int32>(page.GetPageUrl(i.ToString(), i)

#line default
#line hidden
, 1068), false)
);

WriteLiteralTo(__razor_helper_writer, ">");


#line 34 "..\..\Views\EntityList\_Pager1.cshtml"
                                                WriteTo(__razor_helper_writer, i);


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "</a></li>\r\n");


#line 35 "..\..\Views\EntityList\_Pager1.cshtml"
        }
    }

    if (page.PageIndex >= page.PageCount)
    {


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "        <li");

WriteLiteralTo(__razor_helper_writer, " class=\"paginItem current\"");

WriteLiteralTo(__razor_helper_writer, "><a");

WriteLiteralTo(__razor_helper_writer, " href=\"#\"");

WriteLiteralTo(__razor_helper_writer, ">></a></li>\r\n");

WriteLiteralTo(__razor_helper_writer, "        <li");

WriteLiteralTo(__razor_helper_writer, " class=\"paginItem current\"");

WriteLiteralTo(__razor_helper_writer, "><a");

WriteLiteralTo(__razor_helper_writer, " href=\"#\"");

WriteLiteralTo(__razor_helper_writer, ">>|</a></li>\r\n");


#line 42 "..\..\Views\EntityList\_Pager1.cshtml"
    }
    else
    {


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "        <li");

WriteLiteralTo(__razor_helper_writer, " class=\"paginItem\"");

WriteLiteralTo(__razor_helper_writer, "><a");

WriteAttributeTo(__razor_helper_writer, "href", Tuple.Create(" href=\"", 1367), Tuple.Create("\"", 1417)

#line 45 "..\..\Views\EntityList\_Pager1.cshtml"
, Tuple.Create(Tuple.Create("", 1374), Tuple.Create<System.Object, System.Int32>(page.GetPageUrl("下一页", page.PageIndex + 1)

#line default
#line hidden
, 1374), false)
);

WriteLiteralTo(__razor_helper_writer, ">></a></li>\r\n");

WriteLiteralTo(__razor_helper_writer, "        <li");

WriteLiteralTo(__razor_helper_writer, " class=\"paginItem\"");

WriteLiteralTo(__razor_helper_writer, "><a");

WriteAttributeTo(__razor_helper_writer, "href", Tuple.Create(" href=\"", 1463), Tuple.Create("\"", 1508)

#line 46 "..\..\Views\EntityList\_Pager1.cshtml"
, Tuple.Create(Tuple.Create("", 1470), Tuple.Create<System.Object, System.Int32>(page.GetPageUrl("未页", page.PageCount)

#line default
#line hidden
, 1470), false)
);

WriteLiteralTo(__razor_helper_writer, ">>|</a></li>\r\n");


#line 47 "..\..\Views\EntityList\_Pager1.cshtml"
    }


#line default
#line hidden
});

#line 48 "..\..\Views\EntityList\_Pager1.cshtml"
}
#line default
#line hidden

        public _Views_EntityList__Pager1_cshtml()
        {
        }
        public override void Execute()
        {
            
            #line 2 "..\..\Views\EntityList\_Pager1.cshtml"
  
    var page = ViewBag.Page as Pager;
    page.PageUrlTemplate = Url.Action("Index") + "{链接}";
    // 没有总记录数的时候不显示分页，可以认为不启用分页

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("\r\n");

            
            #line 50 "..\..\Views\EntityList\_Pager1.cshtml"
 if (page.TotalCount > 0)
{

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"pagin\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"message\"");

WriteLiteral(">共<i");

WriteLiteral(" class=\"blue\"");

WriteLiteral(">");

            
            #line 53 "..\..\Views\EntityList\_Pager1.cshtml"
                                         Write(page.TotalCount);

            
            #line default
            #line hidden
WriteLiteral("</i>条记录，当前显示第&nbsp;<i");

WriteLiteral(" class=\"blue\"");

WriteLiteral(">");

            
            #line 53 "..\..\Views\EntityList\_Pager1.cshtml"
                                                                                            Write(page.PageIndex);

            
            #line default
            #line hidden
WriteLiteral("&nbsp;</i>页/共&nbsp;<i");

WriteLiteral(" class=\"blue\"");

WriteLiteral(">");

            
            #line 53 "..\..\Views\EntityList\_Pager1.cshtml"
                                                                                                                                              Write(page.PageCount);

            
            #line default
            #line hidden
WriteLiteral("&nbsp;</i>页</div>\r\n        <ul");

WriteLiteral(" class=\"paginList\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 55 "..\..\Views\EntityList\_Pager1.cshtml"
       Write(GetNumericPage(page));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </ul>\r\n    </div>\r\n");

            
            #line 58 "..\..\Views\EntityList\_Pager1.cshtml"
}
            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591

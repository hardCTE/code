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
    using NewLife.Web;
    using XCode;
    using XCode.Membership;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/EntityList/_Data_Action.cshtml")]
    public partial class _Views_EntityList__Data_Action_cshtml : System.Web.Mvc.WebViewPage<dynamic>
    {
        public _Views_EntityList__Data_Action_cshtml()
        {
        }
        public override void Execute()
        {
            
            #line 1 "..\..\Views\EntityList\_Data_Action.cshtml"
  
    var fact = ViewBag.Factory as IEntityOperate;
    var fi = fact.Fields.FirstOrDefault(e => e.Name.EqualIgnoreCase("Deleted", "IsDelete", "IsDeleted"));
    var entity = Model as IEntity;
    var key = entity[fact.Unique];

    var ajax = true;
    if (ViewBag.AjaxDelete is Boolean)
    {
        ajax = (Boolean)ViewBag.AjaxDelete;
    }

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 13 "..\..\Views\EntityList\_Data_Action.cshtml"
 if (ManageProvider.User.Has(PermissionFlags.Update))
{

            
            #line default
            #line hidden
WriteLiteral("    <i");

WriteLiteral(" class=\"glyphicon glyphicon-edit\"");

WriteLiteral(" style=\"color: blue;\"");

WriteLiteral("></i>\r\n");

            
            #line 16 "..\..\Views\EntityList\_Data_Action.cshtml"
    
            
            #line default
            #line hidden
            
            #line 16 "..\..\Views\EntityList\_Data_Action.cshtml"
Write(Html.ActionLink("编辑", "Edit", new { id = @key }, new { @class = "editcell" }));

            
            #line default
            #line hidden
            
            #line 16 "..\..\Views\EntityList\_Data_Action.cshtml"
                                                                                  
}
else if (ManageProvider.User.Has(PermissionFlags.Detail))
{

            
            #line default
            #line hidden
WriteLiteral("    <i");

WriteLiteral(" class=\"glyphicon glyphicon-edit\"");

WriteLiteral(" style=\"color: blue;\"");

WriteLiteral("></i>\r\n");

            
            #line 21 "..\..\Views\EntityList\_Data_Action.cshtml"
    
            
            #line default
            #line hidden
            
            #line 21 "..\..\Views\EntityList\_Data_Action.cshtml"
Write(Html.ActionLink("查看", "Detail", new { id = @key }, new { @class = "editcell" }));

            
            #line default
            #line hidden
            
            #line 21 "..\..\Views\EntityList\_Data_Action.cshtml"
                                                                                    
}

            
            #line default
            #line hidden
            
            #line 23 "..\..\Views\EntityList\_Data_Action.cshtml"
 if (ManageProvider.User.Has(PermissionFlags.Delete))
{
    if (fi != null && fi.Type == typeof(Boolean) && (Boolean)entity[fi.Name])
    {

            
            #line default
            #line hidden
WriteLiteral("        <i");

WriteLiteral(" class=\"glyphicon glyphicon-transfer\"");

WriteLiteral(" style=\"color: green;\"");

WriteLiteral("></i>\r\n");

            
            #line 28 "..\..\Views\EntityList\_Data_Action.cshtml"
        if (ajax)
        {
            
            
            #line default
            #line hidden
            
            #line 30 "..\..\Views\EntityList\_Data_Action.cshtml"
       Write(Html.ActionLink("恢复", "DeleteAjax", new { id = @key }, new { data_action = "delete", data_ajax = "1", onclick = "return confirm('确认恢复？');" }));

            
            #line default
            #line hidden
            
            #line 30 "..\..\Views\EntityList\_Data_Action.cshtml"
                                                                                                                                                          
        }
        else
        {
            
            
            #line default
            #line hidden
            
            #line 34 "..\..\Views\EntityList\_Data_Action.cshtml"
       Write(Html.ActionLink("恢复", "Delete", new { id = @key }, new { onclick = "return confirm('确认恢复？');" }));

            
            #line default
            #line hidden
            
            #line 34 "..\..\Views\EntityList\_Data_Action.cshtml"
                                                                                                             
        }
    }
    else
    {

            
            #line default
            #line hidden
WriteLiteral("        <i");

WriteLiteral(" class=\"glyphicon glyphicon-remove\"");

WriteLiteral(" style=\"color: red;\"");

WriteLiteral("></i>\r\n");

            
            #line 40 "..\..\Views\EntityList\_Data_Action.cshtml"
        if (ajax)
        {
            
            
            #line default
            #line hidden
            
            #line 42 "..\..\Views\EntityList\_Data_Action.cshtml"
       Write(Html.ActionLink("删除", "DeleteAjax", new { id = @key }, new { data_action = "delete", data_ajax = "1", onclick = "return confirm('确认删除？');" }));

            
            #line default
            #line hidden
            
            #line 42 "..\..\Views\EntityList\_Data_Action.cshtml"
                                                                                                                                                          
        }
        else
        {
            
            
            #line default
            #line hidden
            
            #line 46 "..\..\Views\EntityList\_Data_Action.cshtml"
       Write(Html.ActionLink("删除", "Delete", new { id = @key }, new { onclick = "return confirm('确认删除？');" }));

            
            #line default
            #line hidden
            
            #line 46 "..\..\Views\EntityList\_Data_Action.cshtml"
                                                                                                             
        }
    }
}
            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591

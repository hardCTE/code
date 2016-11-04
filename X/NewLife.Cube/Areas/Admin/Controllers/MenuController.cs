﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using XCode.Membership;

namespace NewLife.Cube.Admin.Controllers
{
    /// <summary>菜单控制器</summary>
    [DisplayName("菜单")]
    [Description("系统操作菜单以及功能目录树。支持排序，不可见菜单仅用于功能权限限制。每个菜单的权限子项由系统自动生成，请不要人为修改")]
    public class MenuController : EntityTreeController<Menu>
    {
        static MenuController()
        {
            //// 过滤要显示的字段
            //var names = "ID,TreeNodeName,DisplayName,Url,Sort,Visible,Necessary".Split(",");
            //var fs = Menu.Meta.AllFields;
            //var list = names.Select(e => fs.FirstOrDefault(f => f.Name.EqualIgnoreCase(e))).Where(e => e != null);
            ////list.RemoveAll(e => !names.Contains(e.Name));
            //ListFields.Clear();
            //ListFields.AddRange(list);

            ListFields.RemoveField("Remark");
            FormFields.RemoveField("Remark");
        }

        ///// <summary>动作执行前</summary>
        ///// <param name="filterContext"></param>
        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    ViewBag.HeaderContent = "系统操作菜单以及功能目录树。支持排序，不可见菜单仅用于功能权限限制。每个菜单的权限子项由系统自动生成，请不要人为修改";

        //    base.OnActionExecuting(filterContext);
        //}
    }
}
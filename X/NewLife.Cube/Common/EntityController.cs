﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using NewLife.Common;
using NewLife.Serialization;
using NewLife.Web;
using XCode;
using XCode.Configuration;
using XCode.Membership;

namespace NewLife.Cube
{
    /// <summary>实体控制器基类</summary>
    /// <typeparam name="TEntity"></typeparam>
    [EntityAuthorize]
    public class EntityController<TEntity> : ControllerBaseX where TEntity : Entity<TEntity>, new()
    {
        #region 属性
        /// <summary>实体工厂</summary>
        public static IEntityOperate Factory { get { return Entity<TEntity>.Meta.Factory; } }
        #endregion

        #region 构造
        /// <summary>构造函数</summary>
        public EntityController()
        {
            // 强行实例化一次，初始化实体对象
            var entity = new TEntity();

            ViewBag.Title = Entity<TEntity>.Meta.Table.Description + "管理";
        }
        #endregion

        #region 默认Action
        /// <summary>数据列表首页</summary>
        /// <returns></returns>
        [EntityAuthorize(PermissionFlags.Detail)]
        [DisplayName("{type}管理")]
        public virtual ActionResult Index(Pager p = null)
        {
            if (p == null) p = new Pager();

            ViewBag.Page = p;

            // 用于显示的列
            var fields = GetFields(false);
            ViewBag.Fields = fields;

            return IndexView(p);
        }

        /// <summary>列表页视图。子控制器可重载，以传递更多信息给视图，比如修改要显示的列</summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected virtual ActionResult IndexView(Pager p)
        {
            var list = Entity<TEntity>.Search(p["Q"], p);

            return View("List", list);
        }

        /// <summary>表单，查看</summary>
        /// <param name="id">主键。可能为空（表示添加），所以用字符串而不是整数</param>
        /// <returns></returns>
        [EntityAuthorize(PermissionFlags.Detail)]
        [DisplayName("查看{type}")]
        public virtual ActionResult Detail(String id)
        {
            var entity = Entity<TEntity>.FindByKeyForEdit(id);
            if (entity.IsNullKey) throw new XException("要查看的数据[{0}]不存在！", id);

            return FormView(entity);
        }

        /// <summary>删除</summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [EntityAuthorize(PermissionFlags.Delete)]
        [DisplayName("删除{type}")]
        public virtual ActionResult Delete(Int32 id)
        {
            var url = Request.UrlReferrer + "";

            try
            {
                var entity = Entity<TEntity>.FindByKey(id);
                OnDelete(entity);

                Js.Alert("删除成功！").Redirect(url);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                Js.Alert("删除失败！" + ex.Message).Redirect(url);
                return new EmptyResult();
            }

            //// 跳转到来源地址
            //if (url != "")
            //    return Redirect(url);
            //else
            //    return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [EntityAuthorize(PermissionFlags.Delete)]
        [DisplayName("删除{type}")]
        public virtual JsonResult DeleteAjax(Int32 id)
        {
            var url = Request.UrlReferrer + "";

            try
            {
                var entity = Entity<TEntity>.FindByKey(id);
                OnDelete(entity);

                return Json(new { msg = "删除成功！", code = 0, url = url }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { msg = "删除失败！" + ex.Message, url, code = -1 }, JsonRequestBehavior.AllowGet);

            }
        }

        /// <summary>表单，添加/修改</summary>
        /// <returns></returns>
        [EntityAuthorize(PermissionFlags.Insert)]
        [DisplayName("添加{type}")]
        public virtual ActionResult Add()
        {
            var entity = Factory.Create() as TEntity;

            // 记下添加前的来源页，待会添加成功以后跳转
            Session["Cube_Add_Referrer"] = Request.UrlReferrer.ToString();

            return FormView(entity);
        }

        /// <summary>保存</summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [EntityAuthorize(PermissionFlags.Insert)]
        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult Add(TEntity entity)
        {
            // 检测避免乱用Add/id
            if (Factory.Unique.IsIdentity && entity[Factory.Unique.Name].ToInt() != 0) throw new Exception("我们约定添加数据时路由id部分默认没有数据，以免模型绑定器错误识别！");

            if (!Valid(entity))
            {
                ViewBag.StatusMessage = "验证失败！";
                return FormView(entity);
            }

            var rs = false;
            try
            {
                OnInsert(entity);
                rs = true;
            }
            catch (ArgumentException aex)
            {
                ModelState.AddModelError(aex.ParamName, aex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            if (!rs)
            {
                ViewBag.StatusMessage = "添加失败！";
                // 添加失败，ID清零，否则会显示保存按钮
                entity[Entity<TEntity>.Meta.Unique.Name] = 0;
                return FormView(entity);
            }

            ViewBag.StatusMessage = "添加成功！";

            var url = Session["Cube_Add_Referrer"] + "";
            if (!url.IsNullOrEmpty())
                return Redirect(url);
            else
                // 新增完成跳到列表页，更新完成保持本页
                return RedirectToAction("Index");
        }

        /// <summary>表单，添加/修改</summary>
        /// <param name="id">主键。可能为空（表示添加），所以用字符串而不是整数</param>
        /// <returns></returns>
        [EntityAuthorize(PermissionFlags.Update)]
        [DisplayName("更新{type}")]
        public virtual ActionResult Edit(String id)
        {
            var entity = Entity<TEntity>.FindByKeyForEdit(id);
            if (entity.IsNullKey) throw new XException("要编辑的数据[{0}]不存在！", id);

            return FormView(entity);
        }

        /// <summary>保存</summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [EntityAuthorize(PermissionFlags.Update)]
        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult Edit(TEntity entity)
        {
            if (!Valid(entity))
            {
                ViewBag.StatusMessage = "验证失败！";
                return FormView(entity);
            }

            var rs = 0;
            try
            {
                rs = OnUpdate(entity);
                if (rs <= 0) rs = 1;
            }
            catch (ArgumentException aex)
            {
                ModelState.AddModelError(aex.ParamName, aex.Message);
            }
            catch (Exception ex)
            {
                //ModelState.AddModelError("", ex.Message);
                ModelState.AddModelError("", ex);
            }

            ViewBag.RowsAffected = rs;
            if (rs <= 0)
            {
                ViewBag.StatusMessage = "保存失败！";
                return FormView(entity);
            }
            else
            {
                ViewBag.StatusMessage = "保存成功！";
                // 更新完成保持本页
                return FormView(entity);
            }
        }

        /// <summary>表单页视图。子控制器可以重载，以传递更多信息给视图，比如修改要显示的列</summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual ActionResult FormView(TEntity entity)
        {
            // 用于显示的列
            if (ViewBag.Fields == null) ViewBag.Fields = GetFields(true);

            return View("Form", entity);
        }
        #endregion

        #region 高级Action
        /// <summary>导出Xml</summary>
        /// <returns></returns>
        [EntityAuthorize(PermissionFlags.Detail)]
        [DisplayName("导出")]
        public virtual ActionResult ExportXml()
        {
            //var list = Entity<TEntity>.FindAll();
            //var xml = list.ToXml();
            var obj = OnExportXml();
            var xml = "";
            if (obj is IEntity)
                xml = (obj as IEntity).ToXml();
            else if (obj is EntityList<TEntity>)
                xml = (obj as EntityList<TEntity>).ToXml();

            SetAttachment(null, ".xml");

            return Content(xml, "text/xml", Encoding.UTF8);
        }

        /// <summary>要导出Xml的对象</summary>
        /// <returns></returns>
        protected virtual Object OnExportXml()
        {
            var count = Entity<TEntity>.Meta.Count;
            if (count > 10000) count = 10000;

            return Entity<TEntity>.FindAll(null, null, null, 0, count);
        }

        /// <summary>设置附件响应方式</summary>
        /// <param name="name"></param>
        /// <param name="ext"></param>
        protected void SetAttachment(String name, String ext)
        {
            if (name.IsNullOrEmpty()) name = GetType().GetDisplayName();
            if (name.IsNullOrEmpty()) name = Factory.EntityType.GetDisplayName();
            if (name.IsNullOrEmpty()) name = GetType().Name.TrimEnd("Controller");
            if (!ext.IsNullOrEmpty()) ext = ext.EnsureStart(".");
            name += ext;
            name = HttpUtility.UrlEncode(name, Encoding.UTF8);
            Response.AddHeader("Content-Disposition", "Attachment;filename=" + name);
        }

        /// <summary>导入Xml</summary>
        /// <returns></returns>
        [EntityAuthorize(PermissionFlags.Insert)]
        [DisplayName("导入")]
        [HttpPost]
        public virtual ActionResult ImportXml()
        {
            throw new NotImplementedException();
        }

        /// <summary>导出Json</summary>
        /// <returns></returns>
        [EntityAuthorize(PermissionFlags.Detail)]
        [DisplayName("导出")]
        public virtual ActionResult ExportJson()
        {
            //var list = Entity<TEntity>.FindAll();
            //var json = list.ToJson(true);
            //var json = new Json().Serialize(list);
            var json = OnExportJson().ToJson(true);

            SetAttachment(null, ".json");

            //return Json(list, JsonRequestBehavior.AllowGet);

            return Content(json, "application/json", Encoding.UTF8);
        }

        /// <summary>要导出Json的对象</summary>
        /// <returns></returns>
        protected virtual Object OnExportJson()
        {
            var count = Entity<TEntity>.Meta.Count;
            if (count > 10000) count = 10000;

            return Entity<TEntity>.FindAll(null, null, null, 0, count);
        }

        /// <summary>导入Json</summary>
        /// <returns></returns>
        [EntityAuthorize(PermissionFlags.Insert)]
        [DisplayName("导入")]
        [HttpPost]
        public virtual ActionResult ImportJson()
        {
            throw new NotImplementedException();
        }

        /// <summary>导出Excel</summary>
        /// <returns></returns>
        [EntityAuthorize(PermissionFlags.Detail)]
        [DisplayName("导出")]
        public virtual ActionResult ExportExcel()
        {
            //throw new NotImplementedException();

            // 准备需要输出的列
            var list = new List<FieldItem>();
            foreach (var fi in Factory.AllFields)
            {
                if (Type.GetTypeCode(fi.Type) == TypeCode.Object) continue;

                list.Add(fi);
            }

            var ms = new MemoryStream();

            OnExportExcel(ms, list);

            ms.Position = 0;

            var name = GetType().GetDisplayName() ?? Factory.EntityType.GetDisplayName() ?? Factory.EntityType.Name;
            //name = HttpUtility.UrlEncode(name, Encoding.UTF8);

            return File(ms, "application/ms-excel", name + ".csv");
        }

        /// <summary>导出Excel，可重载修改要输出的结果集</summary>
        /// <param name="ms"></param>
        /// <param name="fs"></param>
        protected virtual void OnExportExcel(Stream ms, List<FieldItem> fs)
        {
            var list = Entity<TEntity>.FindAll();

            OnExportExcel(ms, fs, list);
        }

        /// <summary>导出Excel，可重载修改要输出的列</summary>
        /// <param name="ms"></param>
        /// <param name="fs"></param>
        /// <param name="list"></param>
        protected virtual void OnExportExcel(Stream ms, List<FieldItem> fs, List<TEntity> list)
        {
            // 列头
            {
                var sb = new StringBuilder();
                foreach (var fi in fs)
                {
                    if (sb.Length > 0) sb.Append(",");
                    sb.Append(fi.Name);
                }
                sb.AppendLine();
                ms.Write(sb.ToString().GetBytes());
            }
            // 内容
            foreach (var item in list)
            {
                var sb = new StringBuilder();
                var f = true;
                foreach (var fi in fs)
                {
                    //if (sb.Length > 0) sb.Append(",");
                    // 注意第一个字段的值可能为空，那样导致逗号不匹配
                    if (f)
                        f = false;
                    else
                        sb.Append(",");

                    var v = "{0}".F(item[fi.Name]);

                    // 列内容如存在半角引号（即"）则应替换成半角双引号（""）转义，并用半角引号（即""）将该字段值包含起来
                    if (v.Contains("\"")) v = "\"" + v.Replace("\"", "\"\"") + "\"";
                    // 列内容如存在半角逗号（即,）则用半角双引号（即""）将该字段值包含起来
                    if (v.Contains(",")) v = "\"" + v + "\"";

                    sb.Append(v);
                }
                sb.AppendLine();
                ms.Write(sb.ToString().GetBytes());
            }
        }

        /// <summary>清空全表数据</summary>
        /// <returns></returns>
        [EntityAuthorize(PermissionFlags.Delete)]
        [DisplayName("清空")]
        public virtual ActionResult Clear()
        {
            //var list = Entity<TEntity>.FindAll();

            //list.Delete();

            var count = Entity<TEntity>.Meta.Session.Truncate();
            //return Content("共删除{0}行数据".F(count));
            //return Index();
            //var url = Request.UrlReferrer + "";
            //Js.Alert("共删除{0}行数据".F(count)).Redirect(url);
            //return new EmptyResult();
            Js.Alert("共删除{0}行数据".F(count));
            return Index();
        }
        #endregion

        #region 模版Action
        /// <summary>生成列表</summary>
        /// <returns></returns>
        [EntityAuthorize(PermissionFlags.Delete)]
        [DisplayName("生成列表")]
        public ActionResult MakeList()
        {
            if (!SysConfig.Current.Develop) throw new InvalidOperationException("仅支持开发模式下使用！");

            // 视图路径，Areas/区域/Views/控制器/_List_Data.cshtml
            var vpath = "Areas/{0}/Views/{1}/_List_Data.cshtml".F(RouteData.DataTokens["area"], this.GetType().Name.TrimEnd("Controller"));

            var rs = ViewHelper.MakeListDataView(vpath, ListFields);

            Js.Alert("生成列表模版 {0} 成功！".F(vpath));

            return Index();
        }

        /// <summary>生成表单</summary>
        /// <returns></returns>
        [EntityAuthorize(PermissionFlags.Delete)]
        [DisplayName("生成表单")]
        public ActionResult MakeForm()
        {
            if (!SysConfig.Current.Develop) throw new InvalidOperationException("仅支持开发模式下使用！");

            // 视图路径，Areas/区域/Views/控制器/_List_Data.cshtml
            var vpath = "Areas/{0}/Views/{1}/_List_Data.cshtml".F(RouteData.DataTokens["area"], this.GetType().Name.TrimEnd("Controller"));

            var rs = ViewHelper.MakeListDataView(vpath, FormFields);

            Js.Alert("生成列表模版 {0} 成功！".F(vpath));

            return Index();
        }
        #endregion

        #region 实体操作重载
        /// <summary>添加实体对象</summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual Int32 OnInsert(TEntity entity) { return entity.Insert(); }

        /// <summary>更新实体对象</summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual Int32 OnUpdate(TEntity entity) { return entity.Update(); }

        /// <summary>删除实体对象</summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual Int32 OnDelete(TEntity entity) { return entity.Delete(); }

        /// <summary>验证实体对象</summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual Boolean Valid(TEntity entity) { return true; }
        #endregion

        #region 列表字段和表单字段
        private static FieldCollection _ListFields;
        /// <summary>列表字段过滤</summary>
        protected static FieldCollection ListFields { get { return _ListFields ?? (_ListFields = new FieldCollection(Factory).SetRelation(false)); } set { _ListFields = value; } }

        private static FieldCollection _FormFields;
        /// <summary>表单字段过滤</summary>
        protected static FieldCollection FormFields { get { return _FormFields ?? (_FormFields = new FieldCollection(Factory).SetRelation(true)); } set { _FormFields = value; } }

        /// <summary>获取要显示的字段列表</summary>
        /// <param name="isForm">是否是表单</param>
        /// <returns></returns>
        protected virtual List<FieldItem> GetFields(Boolean isForm)
        {
            return (isForm ? FormFields : ListFields) ?? Entity<TEntity>.Meta.Fields.ToList();
        }
        #endregion

        #region 权限菜单
        /// <summary>自动从实体类拿到显示名</summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        protected override IDictionary<MethodInfo, Int32> ScanActionMenu(IMenu menu)
        {
            // 设置显示名
            if (menu.DisplayName.IsNullOrEmpty())
            {
                menu.DisplayName = Entity<TEntity>.Meta.Table.DataTable.DisplayName;
                menu.Visible = true;
                //menu.Save();
            }

            var dic = base.ScanActionMenu(menu);

            // 只写实体类过滤掉添删改权限
            if (Factory.Table.DataTable.InsertOnly)
            {
                var arr = new PermissionFlags[] { PermissionFlags.Insert, PermissionFlags.Update, PermissionFlags.Delete }.Select(e => (Int32)e).ToArray();
                dic = dic.Where(e => !arr.Contains(e.Value)).ToDictionary(e => e.Key, e => e.Value);
            }

            return dic;
        }
        #endregion

        #region 默认页头
        /// <summary>动作执行前</summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // 默认加上实体工厂
            ViewBag.Factory = Entity<TEntity>.Meta.Factory;

            if (ViewBag.HeaderTitle == null) ViewBag.HeaderTitle = Entity<TEntity>.Meta.Table.Description + "管理";

            var txt = (String)ViewBag.HeaderContent;
            if (txt.IsNullOrEmpty() && ManageProvider.Menu.Current != null) txt = ManageProvider.Menu.Current.Remark;
            if (txt.IsNullOrEmpty()) txt = GetType().GetDescription();
            if (txt.IsNullOrEmpty() && SysConfig.Current.Develop)
                txt = "这里是页头内容，来自于菜单备注，或者给控制器增加Description特性";
            //txt = "这里是页头内容，你可以通过重载OnActionExecuting然后设置ViewBag.HeaderTitle/HeaderContent来修改，或者给控制器增加Description特性";
            ViewBag.HeaderContent = txt;

            base.OnActionExecuting(filterContext);
        }
        #endregion
    }
}
﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using System.Windows.Forms;
using NewLife.Log;
using NewLife.Threading;
using XCode.DataAccessLayer;

namespace XCoder
{
    public partial class FrmSchema : Form
    {
        #region 属性
        private IDatabase _Db;
        /// <summary>数据库</summary>
        public IDatabase Db
        {
            get { return _Db; }
            set { _Db = value; }
        }
        #endregion

        #region 初始化界面
        public FrmSchema()
        {
            InitializeComponent();

            Icon = Source.GetIcon();
        }

        public static FrmSchema Create(IDatabase db)
        {
            if (db == null) throw new ArgumentNullException("db");

            FrmSchema frm = new FrmSchema();
            frm.Db = db;

            return frm;
        }

        private void FrmSchema_Load(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                var tables = Db.CreateMetaData().GetTables();
                this.Invoke(SetList, cbTables, tables);
            }).LogException();
            Task.Factory.StartNew(() =>
            {
                var list = Db.CreateMetaData().MetaDataCollections;
                this.Invoke(SetList, cbSchemas, list);
            }).LogException();
        }
        #endregion

        #region 加载
        void SetList(ComboBox cb, IEnumerable data)
        {
            if (cb == null || data == null) return;

            try
            {
                if (!(data is IList))
                {
                    var list = new List<Object>();
                    foreach (Object item in data)
                    {
                        list.Add(item);
                    }
                    data = list;
                }
                cb.DataSource = data;
                //cb.DisplayMember = "value";
                cb.Update();
            }
            catch (Exception ex)
            {
                XTrace.WriteException(ex);
            }
        }
        #endregion

        private void cbTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb == null) return;

            Object obj = cb.SelectedItem;
            if (obj == null) return;

            try
            {
                if (obj is IDataTable)
                {
                    //obj = (obj as IDataTable).Columns;
                    DbCommand cmd = Db.CreateSession().CreateCommand();
                    cmd.CommandText = "select * from " + (obj as IDataTable).TableName;
                    DataTable dt = null;
                    try
                    {
                        using (DbDataReader reader = cmd.ExecuteReader(CommandBehavior.KeyInfo | CommandBehavior.SchemaOnly))
                        {
                            dt = reader.GetSchemaTable();
                        }
                    }
                    finally
                    {
                        Db.CreateSession().AutoClose();
                    }
                    obj = dt;
                }
                else if (obj is String)
                    obj = Db.CreateSession().GetSchema((String)obj, null);
                gv.DataSource = obj;
                gv.Update();
            }
            catch (Exception ex)
            {
                XTrace.WriteException(ex);
            }
        }
    }
}

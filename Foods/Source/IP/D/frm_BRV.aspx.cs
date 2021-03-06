﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Configuration;
using System.Text.RegularExpressions;
using System.IO;
using Foods;

using NHibernate;
using NHibernate.Criterion;
using NHibernate.Cfg;

namespace Foods
{
    public partial class frm_BRV : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["D"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                ShwBRVId();
                FillGrid();
                TBbrvDat.Text = DateTime.Now.ToShortDateString();
                TBChqDat.Text = DateTime.Now.ToShortDateString();
                chk_Act.Checked = true;
                chk_prtd.Checked = true;
                DDL_AccCde.Focus();
                BindDll();
                pnlitno.Visible = false;
            }        
        }

        public void ShwBRVId()
        {
            try
            {
                string str = "select mjv_id, mjv_sono from tbl_mjv order by mjv_id desc";
                SqlCommand cmd = new SqlCommand(str, con);
                con.Open();

                DataTable dt = new DataTable();
                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                adp.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        if (string.IsNullOrEmpty(lbl_brvSNo.Text))
                        {
                            int v = Convert.ToInt32(reader["mjv_id"].ToString());
                            int b = v + 1;
                            lbl_brvSNo.Text = "BR00 " + b.ToString();

                        }
                    }
                }
                else
                {
                    lbl_brvSNo.Text = "BR00 1";
 
                }
                con.Close();

            }
            catch (Exception ex)
            {
                // throw ex;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "isActive", "Alert();", true);
                //lbl_Heading.Text = "Error!";
                lblalert.Text = ex.Message;
            }
        }

        public void FillGrid()
        {
            try
            {
                DataTable dt_ = new DataTable();
                dt_ = tbl_mjvManager.GetBRVList();

                GVScrhBrv.DataSource = dt_;
                GVScrhBrv.DataBind();

                ViewState["MBrv"] = dt_;
            }
            catch (Exception ex)
            {
                //throw;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "isActive", "Alert();", true);
                lblalert.Text = ex.Message;
            }

        }

        public void BindDll()
        {
            try
            {
                //For Sub Head Categories
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "select SubHeadCategoriesID,SubHeadCategoriesName from SubHeadCategories";
                    cmd.Connection = con;
                    con.Open();

                    DataTable dtSbHdcat = new DataTable();
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dtSbHdcat);

                    DDL_AccCde.DataSource = dtSbHdcat;
                    DDL_AccCde.DataTextField = "SubHeadCategoriesName";
                    DDL_AccCde.DataValueField = "SubHeadCategoriesID";
                    DDL_AccCde.DataBind();
                    DDL_AccCde.Items.Add(new ListItem("--Select--", "0"));

                    con.Close();
                }           
            }
       
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "isActive", "Alert();", true);
                //lbl_Heading.Text = "Error!";
                lblalert.Text = ex.Message;
            }
        }

        private int save()
        {
            try
            {
                int j = 1;

                tbl_mjv mjv = new tbl_mjv();
                
                mjv.mjv_id = HFmjv.Value;
                mjv.mjv_sono = string.IsNullOrEmpty(lbl_brvSNo.Text) ? null : lbl_brvSNo.Text;
                mjv.mjv_dat = Convert.ToDateTime(string.IsNullOrEmpty(TBbrvDat.Text) ? null : TBbrvDat.Text);
                mjv.ProductTypeID = "0";
                mjv.SubHeadCategoriesID = string.IsNullOrEmpty(DDL_AccCde.SelectedValue) ? null : DDL_AccCde.SelectedValue;
                mjv.mjv_Narr = string.IsNullOrEmpty(TBNarr.Text) ? null : TBNarr.Text;
                mjv.mjv_debtamt = "0";
                mjv.mjv_crdtamt = "0";
                mjv.mjv_ttl = string.IsNullOrEmpty(TBTotal.Text) ? null : TBTotal.Text;
                mjv.Bank_ID = "";
                mjv.mjv_chqno = string.IsNullOrEmpty(TBChqNo.Text) ? null : TBChqNo.Text;
                mjv.mjv_chqdat = string.IsNullOrEmpty(TBChqDat.Text) ? null : TBChqDat.Text; 
                mjv.mjv_taxper = string.IsNullOrEmpty(TBITAX.Text) ? null : TBITAX.Text;
                mjv.mjv_taxamt = string.IsNullOrEmpty(TBItaxamt.Text) ? null : TBItaxamt.Text;
                mjv.employeeID = "1";
                mjv.CreatedBy = Session["user"].ToString();
                mjv.CreatedAt = DateTime.Today;
                mjv.ISActive = chk_Act.Checked.ToString();
                mjv.mjv_grdttl = string.IsNullOrEmpty(TBttlAmt.Text) ? null : TBttlAmt.Text;
                mjv.mjv_Vchtyp = "BRV";

                tbl_mjvManager mjvmanag = new tbl_mjvManager(mjv);
                mjvmanag.Save();

                return j;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public void Clear()
        {
            BindDll();
            //TBRmk.Text = "";
            //SetInitRowPuritm();
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int i = 0;

                i = save();

                if (i > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "isActive", "Alert();", true);                    
                    lblalert.Text = "Data Saved";
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "isActive", "Alert();", true);
                    lblalert.Text = "Opps Some thing is Wrong Please Call the Administrator!";
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "isActive", "Alert();", true);
                //lbl_Heading.Text = "Error!";
                lblalert.Text = ex.Message; 
            }
        }

        protected void chk_incm_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_incm.Checked == true)
            {
                pnlitno.Visible = true;
            }
            else if (chk_incm.Checked == false)
            {
                pnlitno.Visible = false;
            }
        }

        protected void GVScrhBrv_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                string MBrvSNO = Server.HtmlDecode(GVScrhBrv.Rows[e.RowIndex].Cells[0].Text.ToString());

                SqlCommand cmd = new SqlCommand();

                cmd = new SqlCommand("sp_del_Vchr", con);
                cmd.Parameters.Add("@mjv_sono", SqlDbType.VarChar).Value = MBrvSNO;
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                lbl_err.Text = "Voucher # " + MBrvSNO + " has been Deleted!";
                FillGrid();
            }
            catch (Exception ex)
            {
                lbl_err.Text = ex.Message;
            }

        }

        protected void TBSrhBRV_TextChanged(object sender, EventArgs e)
        {

            try
            {
                FillGrid();
                DataTable _dt = (DataTable)ViewState["MBrv"];
                DataView dv = new DataView(_dt, "mjv_sono LIKE '%" + TBSrhBRV.Text.Trim().ToUpper() + "%'", "[mjv_sono] ASC", DataViewRowState.CurrentRows);
                DataTable dt_ = new DataTable();
                dt_ = dv.ToTable();
                GVScrhBrv.DataSource = dt_;
                GVScrhBrv.DataBind();
                ViewState["MBrv"] = dt_;
            }
            catch (Exception ex)
            {
                //   throw;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "isActive", "Alert();", true);
                lblalert.Text = ex.Message;
            }
        }

        protected void GVScrhBrv_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            GVScrhBrv.PageIndex = e.NewSelectedIndex;
            FillGrid();
        }
    }
}
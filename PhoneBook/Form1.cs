using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlServerCe;

namespace PhoneBook
{
    public partial class Form1 : Form
    {
        //Connect to BD
        SqlCeConnection con = new SqlCeConnection(Properties.Settings.Default.PhoneBookDBConnectionString);
        SqlCeCommand cmd;
        //Moves data from DB to DataSet
        SqlCeDataAdapter da;
        //Display table like in database
        DataTable dt;
        //For local display data
        DataSet ds = new DataSet();

        public Form1()
        {
            InitializeComponent();
            textBox5.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Transfer info from DB to grid view
            con.Open();
            updateDataGrid();
            con.Close();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("Enter name and phone");
                return;
            }
            if (duplicateCheck() == true)
            {
                return;
            }
            try
            {
                //When we click "Save" we put data in DB and update data in dataGrid
                con.Open();
                cmd = new SqlCeCommand("INSERT INTO Users(Name,Phone)VALUES('"+textBox1.Text+"','"+textBox2.Text+"')",con);
                cmd.ExecuteNonQuery();
                updateDataGrid();
                MessageBox.Show("Saved");
                con.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            clear();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                //When we click "Update" we update data in DB and update data in dataGrid
                con.Open();
                cmd = new SqlCeCommand("UPDATE Users SET Name='" + textBox1.Text + "', Phone='" + textBox2.Text + "' where ID ='" + textBox5.Text + "'", con);
                cmd.ExecuteNonQuery();
                updateDataGrid();
                MessageBox.Show("Updated");
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            clear();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                cmd = new SqlCeCommand("DELETE FROM Users where ID ='" + textBox5.Text + "'", con);
                cmd.ExecuteNonQuery();
                updateDataGrid();
                MessageBox.Show("Deleted");
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            clear();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //Copy data from dataGrid in textbox
            int i;
            i = e.RowIndex;
            DataGridViewRow row = dataGridView1.Rows[i];
            textBox1.Text = row.Cells[1].Value.ToString();
            textBox2.Text = row.Cells[2].Value.ToString();
            textBox5.Text = row.Cells[0].Value.ToString();
        }

        private void clear()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox5.Text = "";
        }

        private void updateDataGrid()
        {
            da = new SqlCeDataAdapter("SELECT * FROM Users", con);
            dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            //BindingSource display the same data in different places
            //for example, when we need to display the same information in two forms (in one we will see this information and in another edit it)
            BindingSource bs = new BindingSource();
            bs.DataSource = dataGridView1.DataSource;
            bs.Filter = "Name like '%" + textBox3.Text + "%'";
            dataGridView1.DataSource = bs;
        }

        private Boolean duplicateCheck()
        {
            cmd = new SqlCeCommand("SELECT * FROM Users WHERE Phone = '"+textBox2.Text+"'",con);
            SqlCeDataAdapter da = new SqlCeDataAdapter(cmd);
            da.Fill(ds);
            int i = ds.Tables[0].Rows.Count;
            if (i > 0)
            {
                MessageBox.Show("Mobile: " + textBox2.Text +" already exists");
                ds.Clear();
                return true;
            }
            return false;
        }
    }
}

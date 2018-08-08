using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CYB_Build.Model;

namespace CYB_Build.View
{
    public partial class TaskDefsEditor : UserControl
    {
        public List<TaskItem> Items { get; private set; }
        public TaskDefsEditor(IEnumerable<TaskItem> items)
        {
            InitializeComponent();

            Items = items.ToList();

            Items.ForEach(it =>
            {
                lvTaskDefs.Items.Add(new ListViewItem()
                {
                    Name = it.Config,
                    Text = it.Config,
                    Tag = it
                });


            });
        }
    }
}

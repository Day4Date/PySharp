namespace PyCross
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            flowLayoutPanel1 = new FlowLayoutPanel();
            button1 = new Button();
            button2 = new Button();
            listView1 = new ListView();
            richTextBox1 = new RichTextBox();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            button3 = new Button();
            flowLayoutPanel1.SuspendLayout();
            tabControl1.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(button1);
            flowLayoutPanel1.Controls.Add(button2);
            flowLayoutPanel1.Controls.Add(button3);
            flowLayoutPanel1.Dock = DockStyle.Top;
            flowLayoutPanel1.Location = new Point(0, 0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(800, 103);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // button1
            // 
            button1.Location = new Point(3, 3);
            button1.Name = "button1";
            button1.Size = new Size(116, 29);
            button1.TabIndex = 0;
            button1.Text = "Plugin laden";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(125, 3);
            button2.Name = "button2";
            button2.Size = new Size(165, 29);
            button2.TabIndex = 1;
            button2.Text = "Plugin Ausführen";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // listView1
            // 
            listView1.Dock = DockStyle.Left;
            listView1.GridLines = true;
            listView1.Location = new Point(0, 103);
            listView1.Margin = new Padding(3, 3, 10, 3);
            listView1.Name = "listView1";
            listView1.Size = new Size(151, 347);
            listView1.TabIndex = 1;
            listView1.UseCompatibleStateImageBehavior = false;
            // 
            // richTextBox1
            // 
            richTextBox1.Dock = DockStyle.Left;
            richTextBox1.Location = new Point(151, 103);
            richTextBox1.Margin = new Padding(10, 3, 3, 3);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(181, 347);
            richTextBox1.TabIndex = 2;
            richTextBox1.Text = "";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(332, 103);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(468, 347);
            tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            tabPage1.Location = new Point(4, 29);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(460, 314);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Plugins";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(296, 3);
            button3.Name = "button3";
            button3.Size = new Size(165, 29);
            button3.TabIndex = 2;
            button3.Text = "Event";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tabControl1);
            Controls.Add(richTextBox1);
            Controls.Add(listView1);
            Controls.Add(flowLayoutPanel1);
            Name = "Form1";
            Text = "Form1";
            flowLayoutPanel1.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private Button button1;
        private Button button2;
        private ListView listView1;
        private RichTextBox richTextBox1;
        private TabPage tabPage1;
        public TabControl tabControl1;
        private Button button3;
    }
}

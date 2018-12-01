using System;
using System.Windows.Forms;
using System.Drawing;
using WixSharp;
using Microsoft.Deployment.WindowsInstaller;
using System.IO;

public class CustomDialog : WixCLRDialog
{
    private GroupBox groupBox1;
    private Button cancelBtn;
    private Button nextBtn;
    private TextBox textBox1;
    private Label label1;
    private Label label2;
    private TextBox textBox2;
    private Label label3;
    private TextBox textBox3;
    private PictureBox pictureBox1;
    private Label label4;
    private Label label5;
    private Button backBtn;
    private RadioButton demoRadioButton;
    private GroupBox groupBox3;
    private RadioButton freeRadioButton;
    private RadioButton proRadioButton;
    private RadioButton trialRadioButton;
    private GroupBox groupBox2;

    public CustomDialog()
    {
        InitializeComponent();
    }

    protected override IntPtr GetMsiForegroundWindow()
    {
        var handle = base.GetMsiForegroundWindow();

        if (handle == IntPtr.Zero)
        {
            //use any suitable algorithm to find your setup main window
            handle = Win32.FindWindow(null, "your setup main window title");
        }

        return handle;
    }

    public CustomDialog(Session session)
        : base(session)
    {
        InitializeComponent();

        LoadResources();
    }

    void LoadResources()
    {
        try
        {
            Stream s = this.GetMSIBinaryStream("WixUI_Bmp_Banner");
            pictureBox1.Image = Bitmap.FromStream(s);
        }
        catch { }
    }

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
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
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.groupBox1 = new System.Windows.Forms.GroupBox();
        this.cancelBtn = new System.Windows.Forms.Button();
        this.nextBtn = new System.Windows.Forms.Button();
        this.backBtn = new System.Windows.Forms.Button();
        this.textBox1 = new System.Windows.Forms.TextBox();
        this.label1 = new System.Windows.Forms.Label();
        this.label2 = new System.Windows.Forms.Label();
        this.textBox2 = new System.Windows.Forms.TextBox();
        this.label3 = new System.Windows.Forms.Label();
        this.textBox3 = new System.Windows.Forms.TextBox();
        this.pictureBox1 = new System.Windows.Forms.PictureBox();
        this.label4 = new System.Windows.Forms.Label();
        this.label5 = new System.Windows.Forms.Label();
        this.groupBox2 = new System.Windows.Forms.GroupBox();
        this.demoRadioButton = new System.Windows.Forms.RadioButton();
        this.groupBox3 = new System.Windows.Forms.GroupBox();
        this.freeRadioButton = new System.Windows.Forms.RadioButton();
        this.proRadioButton = new System.Windows.Forms.RadioButton();
        this.trialRadioButton = new System.Windows.Forms.RadioButton();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
        this.groupBox3.SuspendLayout();
        this.SuspendLayout();
        //
        // groupBox1
        //
        this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
        | System.Windows.Forms.AnchorStyles.Right)));
        this.groupBox1.Location = new System.Drawing.Point(8, 305);
        this.groupBox1.Name = "groupBox1";
        this.groupBox1.Size = new System.Drawing.Size(483, 2);
        this.groupBox1.TabIndex = 7;
        this.groupBox1.TabStop = false;
        //
        // cancelBtn
        //
        this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        this.cancelBtn.Location = new System.Drawing.Point(412, 322);
        this.cancelBtn.Name = "cancelBtn";
        this.cancelBtn.Size = new System.Drawing.Size(75, 23);
        this.cancelBtn.TabIndex = 5;
        this.cancelBtn.Text = "Cancel";
        this.cancelBtn.UseVisualStyleBackColor = true;
        this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
        //
        // nextBtn
        //
        this.nextBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        this.nextBtn.Enabled = false;
        this.nextBtn.Location = new System.Drawing.Point(316, 322);
        this.nextBtn.Name = "nextBtn";
        this.nextBtn.Size = new System.Drawing.Size(75, 23);
        this.nextBtn.TabIndex = 4;
        this.nextBtn.Text = "Next";
        this.nextBtn.UseVisualStyleBackColor = true;
        this.nextBtn.Click += new System.EventHandler(this.nextBtn_Click);
        //
        // backBtn
        //
        this.backBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        this.backBtn.Location = new System.Drawing.Point(235, 322);
        this.backBtn.Name = "backBtn";
        this.backBtn.Size = new System.Drawing.Size(75, 23);
        this.backBtn.TabIndex = 3;
        this.backBtn.Text = "Back";
        this.backBtn.UseVisualStyleBackColor = true;
        this.backBtn.Click += new System.EventHandler(this.backBtn_Click);
        //
        // textBox1
        //
        this.textBox1.Location = new System.Drawing.Point(47, 252);
        this.textBox1.Name = "textBox1";
        this.textBox1.Size = new System.Drawing.Size(41, 20);
        this.textBox1.TabIndex = 0;
        this.textBox1.TextChanged += new System.EventHandler(this.textBox_TextChanged);
        this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
        this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
        //
        // label1
        //
        this.label1.AutoSize = true;
        this.label1.Location = new System.Drawing.Point(44, 227);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(123, 13);
        this.label1.TabIndex = 9;
        this.label1.Text = "Product activation code:";
        //
        // label2
        //
        this.label2.AutoSize = true;
        this.label2.Location = new System.Drawing.Point(93, 255);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(10, 13);
        this.label2.TabIndex = 10;
        this.label2.Text = "-";
        //
        // textBox2
        //
        this.textBox2.Location = new System.Drawing.Point(108, 252);
        this.textBox2.Name = "textBox2";
        this.textBox2.Size = new System.Drawing.Size(41, 20);
        this.textBox2.TabIndex = 1;
        this.textBox2.TextChanged += new System.EventHandler(this.textBox_TextChanged);
        this.textBox2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
        this.textBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
        //
        // label3
        //
        this.label3.AutoSize = true;
        this.label3.Location = new System.Drawing.Point(154, 255);
        this.label3.Name = "label3";
        this.label3.Size = new System.Drawing.Size(10, 13);
        this.label3.TabIndex = 10;
        this.label3.Text = "-";
        //
        // textBox3
        //
        this.textBox3.Location = new System.Drawing.Point(169, 252);
        this.textBox3.Name = "textBox3";
        this.textBox3.Size = new System.Drawing.Size(41, 20);
        this.textBox3.TabIndex = 2;
        this.textBox3.TextChanged += new System.EventHandler(this.textBox_TextChanged);
        this.textBox3.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
        this.textBox3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
        //
        // pictureBox1
        //
        this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Top;
        this.pictureBox1.Location = new System.Drawing.Point(0, 0);
        this.pictureBox1.Name = "pictureBox1";
        this.pictureBox1.Size = new System.Drawing.Size(498, 60);
        this.pictureBox1.TabIndex = 11;
        this.pictureBox1.TabStop = false;
        //
        // label4
        //
        this.label4.AutoSize = true;
        this.label4.BackColor = System.Drawing.Color.White;
        this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)), true);
        this.label4.Location = new System.Drawing.Point(25, 9);
        this.label4.Name = "label4";
        this.label4.Size = new System.Drawing.Size(111, 13);
        this.label4.TabIndex = 9;
        this.label4.Text = "Product activation";
        //
        // label5
        //
        this.label5.AutoSize = true;
        this.label5.BackColor = System.Drawing.Color.White;
        this.label5.Location = new System.Drawing.Point(46, 29);
        this.label5.Name = "label5";
        this.label5.Size = new System.Drawing.Size(160, 13);
        this.label5.TabIndex = 9;
        this.label5.Text = "Please enter the activation code";
        //
        // groupBox2
        //
        this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
        | System.Windows.Forms.AnchorStyles.Right)));
        this.groupBox2.Location = new System.Drawing.Point(0, 59);
        this.groupBox2.Name = "groupBox2";
        this.groupBox2.Size = new System.Drawing.Size(499, 2);
        this.groupBox2.TabIndex = 8;
        this.groupBox2.TabStop = false;
        //
        // demoRadioButton
        //
        this.demoRadioButton.AutoSize = true;
        this.demoRadioButton.Location = new System.Drawing.Point(20, 19);
        this.demoRadioButton.Name = "demoRadioButton";
        this.demoRadioButton.Size = new System.Drawing.Size(53, 17);
        this.demoRadioButton.TabIndex = 12;
        this.demoRadioButton.Text = "Demo";
        this.demoRadioButton.UseVisualStyleBackColor = true;
        this.demoRadioButton.CheckedChanged += new System.EventHandler(this.demoRadioButton_CheckedChanged);
        //
        // groupBox3
        //
        this.groupBox3.Controls.Add(this.freeRadioButton);
        this.groupBox3.Controls.Add(this.proRadioButton);
        this.groupBox3.Controls.Add(this.trialRadioButton);
        this.groupBox3.Controls.Add(this.demoRadioButton);
        this.groupBox3.Location = new System.Drawing.Point(28, 78);
        this.groupBox3.Name = "groupBox3";
        this.groupBox3.Size = new System.Drawing.Size(200, 126);
        this.groupBox3.TabIndex = 13;
        this.groupBox3.TabStop = false;
        this.groupBox3.Text = "LicenceType";
        //
        // freeRadioButton
        //
        this.freeRadioButton.AutoSize = true;
        this.freeRadioButton.Location = new System.Drawing.Point(20, 88);
        this.freeRadioButton.Name = "freeRadioButton";
        this.freeRadioButton.Size = new System.Drawing.Size(135, 17);
        this.freeRadioButton.TabIndex = 12;
        this.freeRadioButton.Text = "Free Community Edition";
        this.freeRadioButton.UseVisualStyleBackColor = true;
        this.freeRadioButton.CheckedChanged += new System.EventHandler(this.demoRadioButton_CheckedChanged);
        //
        // proRadioButton
        //
        this.proRadioButton.AutoSize = true;
        this.proRadioButton.Checked = true;
        this.proRadioButton.Location = new System.Drawing.Point(20, 65);
        this.proRadioButton.Name = "proRadioButton";
        this.proRadioButton.Size = new System.Drawing.Size(82, 17);
        this.proRadioButton.TabIndex = 12;
        this.proRadioButton.TabStop = true;
        this.proRadioButton.Text = "Professional";
        this.proRadioButton.UseVisualStyleBackColor = true;
        this.proRadioButton.CheckedChanged += new System.EventHandler(this.demoRadioButton_CheckedChanged);
        //
        // trialRadioButton
        //
        this.trialRadioButton.AutoSize = true;
        this.trialRadioButton.Location = new System.Drawing.Point(20, 41);
        this.trialRadioButton.Name = "trialRadioButton";
        this.trialRadioButton.Size = new System.Drawing.Size(45, 17);
        this.trialRadioButton.TabIndex = 12;
        this.trialRadioButton.Text = "Trial";
        this.trialRadioButton.UseVisualStyleBackColor = true;
        this.trialRadioButton.CheckedChanged += new System.EventHandler(this.demoRadioButton_CheckedChanged);
        //
        // CustomDialog
        //
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(498, 357);
        this.Controls.Add(this.groupBox3);
        this.Controls.Add(this.groupBox2);
        this.Controls.Add(this.label3);
        this.Controls.Add(this.label2);
        this.Controls.Add(this.label4);
        this.Controls.Add(this.label5);
        this.Controls.Add(this.label1);
        this.Controls.Add(this.textBox3);
        this.Controls.Add(this.textBox2);
        this.Controls.Add(this.textBox1);
        this.Controls.Add(this.groupBox1);
        this.Controls.Add(this.cancelBtn);
        this.Controls.Add(this.nextBtn);
        this.Controls.Add(this.backBtn);
        this.Controls.Add(this.pictureBox1);
        this.MaximizeBox = false;
        this.Name = "CustomDialog";
        this.Text = "CLR Wix Dialog";
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
        this.groupBox3.ResumeLayout(false);
        this.groupBox3.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion Windows Form Designer generated code

    private void cancelBtn_Click(object sender, EventArgs e)
    {
        MSICancel();
    }

    private void nextBtn_Click(object sender, EventArgs e)
    {
        MSINext();
    }

    private void backBtn_Click(object sender, EventArgs e)
    {
        MSIBack();
    }

    const int maxCharCount = 4;

    private void textBox_KeyPress(object sender, KeyPressEventArgs e)
    {
        var textBox = (TextBox)sender;

        if (textBox.Text.Length >= maxCharCount && textBox.SelectionLength == 0 && e.KeyChar != '\b') //backspace
        {
            e.Handled = true;

            if (sender == textBox1 && textBox2.Text.Length < maxCharCount)
            {
                if (textBox2.Text.Length == 0)
                    textBox2.Text += e.KeyChar;

                JumpToTextBox(textBox2);
            }
            else if (sender == textBox2 && textBox3.Text.Length < maxCharCount)
            {
                if (textBox3.Text.Length == 0)
                    textBox3.Text += e.KeyChar;

                JumpToTextBox(textBox3);
            }
        }
        else
        {
            if (e.KeyChar == '\b') //backspace
            {
                if (textBox.Text == "")
                {
                    if (sender == textBox3)
                    {
                        JumpToTextBox(textBox2);
                    }
                    else if (sender == textBox2)
                    {
                        JumpToTextBox(textBox1);
                    }
                }
            }
        }
    }

    void JumpToTextBox(TextBox textbox)
    {
        textbox.Focus();

        textbox.SelectionLength = 0;
        textbox.SelectionStart = textbox.Text.Length;
    }

    private void textBox_TextChanged(object sender, EventArgs e)
    {
        var textbox = (TextBox)sender;

        int length = textbox.SelectionLength;
        int start = textbox.SelectionStart;

        textbox.Text = textbox.Text.ToUpper();

        textbox.SelectionLength = length;
        textbox.SelectionStart = start;

        nextBtn.Enabled =
            (textBox1.Text.Length == maxCharCount &&
             textBox2.Text.Length == maxCharCount &&
             textBox3.Text.Length == maxCharCount);
    }

    private void textBox1_KeyDown(object sender, KeyEventArgs e)
    {
        var textbox = (TextBox)sender;
        if (e.KeyCode == Keys.Left)
        {
            if (textbox.SelectionStart == 0)
            {
                if (textbox == textBox3)
                    JumpToTextBox(textBox2);
                else if (textbox == textBox2)
                    JumpToTextBox(textBox1);
            }
        }

        if (e.KeyCode == Keys.Right)
        {
            if (textbox.SelectionStart == textbox.Text.Length)
            {
                if (textbox == textBox1)
                {
                    JumpToTextBox(textBox2);
                    textBox2.SelectionStart = 0;
                }
                else if (textbox == textBox2)
                {
                    JumpToTextBox(textBox3);
                    textBox3.SelectionStart = 0;
                }
            }
        }
    }

    public string ActivationKey
    {
        get
        {
            return textBox1.Text + textBox2.Text + textBox3.Text;
        }
        set
        {
            textBox1.Text = value.Substring(0, Math.Min(value.Length, 4));
            if (value.Length > 4)
                textBox2.Text = value.Substring(4, Math.Min(value.Length - 4, 4));
            else
                textBox2.Text = "";

            if (value.Length > 8)
                textBox3.Text = value.Substring(8, Math.Min(value.Length - 8, 4));
            else
                textBox3.Text = "";
        }
    }

    private void demoRadioButton_CheckedChanged(object sender, EventArgs e)
    {
        if (demoRadioButton.Checked)
            ActivationKey = "DEMO12344775";
        else if (trialRadioButton.Checked)
            ActivationKey = "TRIAL2344775";
        else if (proRadioButton.Checked)
            ActivationKey = "";
        else if (freeRadioButton.Checked)
            ActivationKey = "FREE22344775";
        else
            ActivationKey = "";
    }
}
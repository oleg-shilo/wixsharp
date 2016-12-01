using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using System.Collections.Generic;
using System.Windows.Forms;
using ConsoleApplication1;

public partial class MultiStepCustomDialog : WixCLRDialog
{
    List<Form> stepsViews = new List<Form>();
    int CurrentStep
    {
        get
        {
            int step = 0;
            int.TryParse(session["CutsomStep"], out step);
            return step;
        }
        set
        {
            session["CutsomStep"] = value.ToString();
        }
    }

    public MultiStepCustomDialog()
    {
        InitializeComponent();
    }

    public MultiStepCustomDialog(Session session)
        : base(session)
    {
        InitializeComponent();
         
        stepsViews.Add(InitView(new Step1Panel(session)));
        stepsViews.Add(InitView(new Step2Panel(session)));

        SetStep(CurrentStep);
    }

    Form InitView(Form view)
    {
        view.FormBorderStyle = FormBorderStyle.None;
        view.TopLevel = false;
        view.Dock = DockStyle.Fill;
        view.Parent = this.panel1;
        view.Visible = true;
        return view;
    }

    void SetStep(int step)
    {
        CurrentStep = step;
        foreach (var item in stepsViews)
            item.Visible = false;
        stepsViews[step].Visible = true;
    }

    void cancelBtn_Click(object sender, EventArgs e)
    {
        MSICancel();
    }

    void nextBtn_Click(object sender, EventArgs e)
    {
        if ((CurrentStep + 1) < stepsViews.Count)
            SetStep(CurrentStep + 1);
        else
            MSINext();
    }

    void backBtn_Click(object sender, EventArgs e)
    {
        if (CurrentStep > 0)
            SetStep(CurrentStep - 1);
        else
            MSIBack();
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Xml;
using System.Windows.Markup;

using Wpf = System.Windows;
using WpfControls = System.Windows.Controls;
using WpfShapes = System.Windows.Shapes;
using WpfMedia = System.Windows.Media;
using WpfIntegration = System.Windows.Forms.Integration;

using Crom.Controls.Docking;

namespace ACSO
{
    public partial class AntColonyGUIApp : Form, IProblemReport, ICitiesReport
    {
        // Fields 
        private Colony Colony;
        //
        Form citiesForm = new Form();
        PropertiesForm propertiesForm = new PropertiesForm();
        LogForm logForm = new LogForm();
        //
        WpfIntegration.ElementHost host;
        CitiesControl citiesControl;
        WpfControls.Canvas cityCanvas;
        private ToolStripContainer toolStripContainer1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newBtn;
        private ToolStripMenuItem openBtn;
        private ToolStripMenuItem saveBtn;
        private ToolStrip mainStrip;
        private ToolStripButton selectorBtn;
        private ToolStripButton creatorBtn;
        private Crom.Controls.Docking.DockContainer docker;
        private ToolStrip toolStrip1;
        private ToolStripButton newToolStripButton;
        private ToolStripButton openToolStripButton;
        private ToolStripButton saveToolStripButton;
        private ToolStripButton printToolStripButton;
        private ToolStripSeparator toolStripSeparator;
        private ToolStripButton cutToolStripButton;
        private ToolStripButton copyToolStripButton;
        private ToolStripButton pasteToolStripButton;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton helpToolStripButton;
        WpfControls.Canvas tourCanvas;
        //

        public AntColonyGUIApp()
        {
            InitializeComponent();
        }
        public void Start()
        {
            Problem problem = new Problem();
            Graph graph = new Graph();
            problem.graph = graph;
            problem.numberIterations = Convert.ToInt32(propertiesForm.iterationsBox.Text.Trim());
            problem.probabilityThreshold = Convert.ToDouble(propertiesForm.thresholdBox.Text.Trim());
            //
            if (propertiesForm.knownOptimumBox.Text.Trim().Length > 0)
            {
                problem.knownOptimum = Convert.ToInt32(propertiesForm.knownOptimumBox.Text.Trim());
            }
            //
            try
            {
                graph.rawData = new Wpf.Point[citiesControl.CityCount + 1]; // natural indexing 
                int city = 1;
                foreach (WpfShapes.Shape shape in cityCanvas.Children)
                {
                    Wpf.Vector vector = WpfMedia.VisualTreeHelper.GetOffset(shape);
                    Wpf.Point currentPoint = new Wpf.Point(vector.X, vector.Y);
                    //
                    double x = currentPoint.X + (shape.Width / 2);
                    double y = currentPoint.Y + (shape.Height / 2);
                    graph.rawData[city] = new Wpf.Point((int)x, (int)y);
                    city++;
                }
            }
            catch (Exception)
            {
                LogTextEvent("Error reading input data.\n");
                return;
            }
            graph.numberCities = citiesControl.CityCount;
            //
            problem.report = this;
            //
            Colony = new Colony(problem);
            Colony.Start();
        }

        public void DrawTour(Wpf.Point[] rawData, Tour tour)
        {
            if (host.InvokeRequired)
            {
                host.BeginInvoke(new MethodInvoker(delegate
                {
                    DrawTour(rawData, tour);
                }));
                return;
            }
            tourCanvas.Children.Clear();
            for (int index = 1; index <= citiesControl.CityCount; index++)
            {
                Arrow arrow = new Arrow();
                arrow.X1 = rawData[tour.data[index]].X;
                arrow.Y1 = rawData[tour.data[index]].Y;
                arrow.X2 = rawData[tour.data[index + 1]].X;
                arrow.Y2 = rawData[tour.data[index + 1]].Y;
                arrow.StrokeThickness = 1;
                arrow.Stroke = WpfMedia.Brushes.Black;
                tourCanvas.Children.Add(arrow);
            }
        }
        public void Alert(string alertText)
        {
            MessageBox.Show(alertText);
        }
        public void LogTextEvent(string EventText)
        {
            LogTextEvent(logForm.richTextLog, EventText);
        }
        public void LogTextEvent(RichTextBox TextEventLog, string EventText)
        {
            LogTextEvent(TextEventLog, EventText, Color.Black);
        }
        public void LogTextEvent(RichTextBox TextEventLog, string EventText, Color TextColor)
        {
            if (TextEventLog.InvokeRequired)
            {
                TextEventLog.BeginInvoke(new MethodInvoker(delegate
                {
                    LogTextEvent(TextEventLog, EventText, TextColor);
                }));
                return;
            }

            //string nDateTime = DateTime.Now.ToString("hh:mm:ss tt") + " - ";

            // color text.
            TextEventLog.SelectionStart = TextEventLog.Text.Length;
            TextEventLog.SelectionColor = TextColor;

            // newline if first line, append if else.
            if (TextEventLog.Lines.Length == 0)
            {
                //TextEventLog.AppendText(nDateTime + EventText);
                TextEventLog.AppendText(EventText);
                TextEventLog.ScrollToCaret();
                TextEventLog.AppendText(System.Environment.NewLine);
            }
            else
            {
                //TextEventLog.AppendText(nDateTime + EventText + System.Environment.NewLine);
                TextEventLog.AppendText(EventText + System.Environment.NewLine);
                TextEventLog.ScrollToCaret();
            }
        }
        public void ProblemFinished()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate
                {
                    ProblemFinished();
                }));
                return;
            }
            propertiesForm.startBtn.Enabled = true;
            propertiesForm.iterationsBox.Enabled = true;
        }
        private void startBtn_Click(object sender, EventArgs e)
        {
            Start();
            propertiesForm.startBtn.Enabled = false;
            propertiesForm.iterationsBox.Enabled = false;
        }
        private void AntColonyGUIApp_FormClosing(object sender, FormClosingEventArgs e)
        {
            Colony.stop = true;
            Thread.Sleep(1000);
        }
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AntColonyGUIApp));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.docker = new Crom.Controls.Docking.DockContainer();
            this.mainStrip = new System.Windows.Forms.ToolStrip();
            this.selectorBtn = new System.Windows.Forms.ToolStripButton();
            this.creatorBtn = new System.Windows.Forms.ToolStripButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.openBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.saveBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.newToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.printToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.copyToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.pasteToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.helpToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.LeftToolStripPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.mainStrip.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.docker);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(890, 592);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // toolStripContainer1.LeftToolStripPanel
            // 
            this.toolStripContainer1.LeftToolStripPanel.Controls.Add(this.mainStrip);
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(914, 641);
            this.toolStripContainer1.TabIndex = 0;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // docker
            // 
            this.docker.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(118)))), ((int)(((byte)(118)))), ((int)(((byte)(118)))));
            this.docker.CanMoveByMouseFilledForms = true;
            this.docker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.docker.Location = new System.Drawing.Point(0, 0);
            this.docker.Name = "docker";
            this.docker.Size = new System.Drawing.Size(890, 592);
            this.docker.TabIndex = 0;
            this.docker.TitleBarGradientColor1 = System.Drawing.SystemColors.Control;
            this.docker.TitleBarGradientColor2 = System.Drawing.Color.White;
            this.docker.TitleBarGradientSelectedColor1 = System.Drawing.Color.DarkGray;
            this.docker.TitleBarGradientSelectedColor2 = System.Drawing.Color.White;
            this.docker.TitleBarTextColor = System.Drawing.Color.Black;
            // 
            // mainStrip
            // 
            this.mainStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.mainStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectorBtn,
            this.creatorBtn});
            this.mainStrip.Location = new System.Drawing.Point(0, 3);
            this.mainStrip.Name = "mainStrip";
            this.mainStrip.Size = new System.Drawing.Size(24, 57);
            this.mainStrip.TabIndex = 0;
            // 
            // selectorBtn
            // 
            this.selectorBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.selectorBtn.Image = global::ACSO.Properties.Resources.cursor;
            this.selectorBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.selectorBtn.Name = "selectorBtn";
            this.selectorBtn.Size = new System.Drawing.Size(22, 20);
            this.selectorBtn.Text = "toolStripButton1";
            this.selectorBtn.Click += new System.EventHandler(this.selectorBtn_Click);
            // 
            // creatorBtn
            // 
            this.creatorBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.creatorBtn.Image = global::ACSO.Properties.Resources.add;
            this.creatorBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.creatorBtn.Name = "creatorBtn";
            this.creatorBtn.Size = new System.Drawing.Size(22, 20);
            this.creatorBtn.Text = "toolStripButton2";
            this.creatorBtn.Click += new System.EventHandler(this.creatorBtn_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(914, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newBtn,
            this.openBtn,
            this.saveBtn});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newBtn
            // 
            this.newBtn.Image = global::ACSO.Properties.Resources.page_white_star;
            this.newBtn.Name = "newBtn";
            this.newBtn.Size = new System.Drawing.Size(152, 22);
            this.newBtn.Text = "New";
            this.newBtn.Click += new System.EventHandler(this.newBtn_Click);
            // 
            // openBtn
            // 
            this.openBtn.Image = global::ACSO.Properties.Resources.folder_page_white;
            this.openBtn.Name = "openBtn";
            this.openBtn.Size = new System.Drawing.Size(152, 22);
            this.openBtn.Text = "Open";
            this.openBtn.Click += new System.EventHandler(this.openBtn_Click);
            // 
            // saveBtn
            // 
            this.saveBtn.Image = global::ACSO.Properties.Resources.disk;
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(152, 22);
            this.saveBtn.Text = "Save";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripButton,
            this.openToolStripButton,
            this.saveToolStripButton,
            this.printToolStripButton,
            this.toolStripSeparator,
            this.cutToolStripButton,
            this.copyToolStripButton,
            this.pasteToolStripButton,
            this.toolStripSeparator1,
            this.helpToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(3, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(208, 25);
            this.toolStrip1.TabIndex = 1;
            // 
            // newToolStripButton
            // 
            this.newToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.newToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("newToolStripButton.Image")));
            this.newToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newToolStripButton.Name = "newToolStripButton";
            this.newToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.newToolStripButton.Text = "&New";
            this.newToolStripButton.Click += new System.EventHandler(this.newBtn_Click);
            // 
            // openToolStripButton
            // 
            this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripButton.Image")));
            this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripButton.Name = "openToolStripButton";
            this.openToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.openToolStripButton.Text = "&Open";
            this.openToolStripButton.Click += new System.EventHandler(this.openBtn_Click);
            // 
            // saveToolStripButton
            // 
            this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripButton.Image")));
            this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripButton.Name = "saveToolStripButton";
            this.saveToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.saveToolStripButton.Text = "&Save";
            // 
            // printToolStripButton
            // 
            this.printToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.printToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("printToolStripButton.Image")));
            this.printToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.printToolStripButton.Name = "printToolStripButton";
            this.printToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.printToolStripButton.Text = "&Print";
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // cutToolStripButton
            // 
            this.cutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cutToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripButton.Image")));
            this.cutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cutToolStripButton.Name = "cutToolStripButton";
            this.cutToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.cutToolStripButton.Text = "C&ut";
            // 
            // copyToolStripButton
            // 
            this.copyToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.copyToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripButton.Image")));
            this.copyToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyToolStripButton.Name = "copyToolStripButton";
            this.copyToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.copyToolStripButton.Text = "&Copy";
            // 
            // pasteToolStripButton
            // 
            this.pasteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pasteToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("pasteToolStripButton.Image")));
            this.pasteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pasteToolStripButton.Name = "pasteToolStripButton";
            this.pasteToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.pasteToolStripButton.Text = "&Paste";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // helpToolStripButton
            // 
            this.helpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.helpToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("helpToolStripButton.Image")));
            this.helpToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.helpToolStripButton.Name = "helpToolStripButton";
            this.helpToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.helpToolStripButton.Text = "He&lp";
            // 
            // AntColonyGUIApp
            // 
            this.ClientSize = new System.Drawing.Size(914, 641);
            this.Controls.Add(this.toolStripContainer1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "AntColonyGUIApp";
            this.Text = "Ant Colony System Optimization";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.LeftToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.LeftToolStripPanel.PerformLayout();
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.mainStrip.ResumeLayout(false);
            this.mainStrip.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }
        public void CitiesModified()
        {
            tourCanvas = citiesControl.tourCanvas;
            cityCanvas = citiesControl.cityCanvas;
            propertiesForm.numberCitiesBox.Text = citiesControl.CityCount.ToString();
        }
        private void LoadCitiesFile(String filename)
        {
            citiesControl.Load(filename);
        }
        private void OnFormLoad(object sender, EventArgs e)
        {
            propertiesForm.startBtn.Click += new EventHandler(startBtn_Click);
            //
            host = new WpfIntegration.ElementHost();
            //
            citiesControl = new CitiesControl();
            citiesControl.IReport = this;
            //
            LoadCitiesFile("../../Data/" + "Template.xaml");
            //
            //
            WpfControls.ScrollViewer scrollViewer = new WpfControls.ScrollViewer();
            scrollViewer.HorizontalScrollBarVisibility = WpfControls.ScrollBarVisibility.Auto;
            scrollViewer.Content = citiesControl;
            host.Child = scrollViewer;
            host.Dock = DockStyle.Fill;
            //
            Form citiesForm = new Form();
            citiesForm.Text = "Cities";
            citiesForm.Controls.Add(host);
            //
            DockableFormInfo citiesFormInfo = docker.Add(citiesForm, zAllowedDock.All, new Guid("a6402b80-2ebd-4fd3-8930-024a6201d001"));
            DockableFormInfo propertiesFormInfo = docker.Add(propertiesForm, zAllowedDock.Right, new Guid("096b52a7-5f4b-44ee-ab77-9830ec717002"));
            DockableFormInfo logFormInfo = docker.Add(logForm, zAllowedDock.Right, new Guid("3d8466c1-e406-4e47-b744-6915afe6e003"));
            //
            docker.DockForm(propertiesFormInfo, DockStyle.Right, zDockMode.Outer);
            docker.DockForm(logFormInfo, propertiesFormInfo, DockStyle.Bottom, zDockMode.Inner);
            docker.DockForm(citiesFormInfo, DockStyle.Fill, zDockMode.None);
        }
        private void newBtn_Click(object sender, EventArgs e)
        {
            LoadCitiesFile(@"..\..\Data\Template.xaml");
        }
        private void openBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog fDialog = new OpenFileDialog();
            fDialog.Title = "Open Cities File";
            fDialog.Filter = "Cities Files|*.xaml";
            //fDialog.InitialDirectory = @"C:\";
            fDialog.InitialDirectory = @"..\..\Data";
            fDialog.ShowDialog();
            string filename = fDialog.FileName.ToString();
            LoadCitiesFile(filename);
        }

        private void selectorBtn_Click(object sender, EventArgs e)
        {
            citiesControl.UseTool("select");
        }

        private void creatorBtn_Click(object sender, EventArgs e)
        {
            citiesControl.UseTool("create");
        }
    }
}

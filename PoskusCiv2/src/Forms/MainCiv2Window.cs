﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PoskusCiv2.Forms
{
    public partial class MainCiv2Window : Form
    {
        MenuStrip MainMenuStrip;
        //public MapForm mapForm;
        public StatusForm statusForm;
        public WorldMapForm worldMapForm;
        public CityForm cityForm;

        public MainCiv2Window()
        {
            InitializeComponent();
            IsMdiContainer = true;
            WindowState = FormWindowState.Maximized;

            //Load the icon
            Icon ico = Properties.Resources.civ2;
            this.Icon = ico;

            //Menustrip
            MainMenuStrip = new MenuStrip
            {
                BackColor = Color.White
            };
            Controls.Add(MainMenuStrip);

            //Advisors menu
            ToolStripMenuItem AdvisorsMenu = new ToolStripMenuItem("Advisors");
            ToolStripMenuItem ChatWithKingsItem = new ToolStripMenuItem("Chat with Kings", null, ChatWithKings_Click, (Keys)Shortcut.CtrlC);
            ToolStripMenuItem ConsultHighCouncilItem = new ToolStripMenuItem("Consult High Council", null, ConsultHighCouncil_Click);
            ToolStripMenuItem CityStatusItem = new ToolStripMenuItem("City Status", null, CityStatus_Click, (Keys)Shortcut.F1);
            ToolStripMenuItem DefenseMinisterItem = new ToolStripMenuItem("Defense Minister", null, DefenseMinister_Click, (Keys)Shortcut.F2);
            ToolStripMenuItem ForeignMinisterItem = new ToolStripMenuItem("Foreign Minister", null, ForeignMinister_Click, (Keys)Shortcut.F3);
            ToolStripMenuItem AttitudeAdvisorItem = new ToolStripMenuItem("Attitude Advisor", null, AttitudeAdvisor_Click, (Keys)Shortcut.F4);
            ToolStripMenuItem TradeAdvisorItem = new ToolStripMenuItem("Trade Advisor", null, TradeAdvisor_Click, (Keys)Shortcut.F5);
            ToolStripMenuItem ScienecAdvisorItem = new ToolStripMenuItem("Science Advisor", null, ScienceAdvisor_Click, (Keys)Shortcut.F6);
            ToolStripMenuItem CasualtyTimelineItem = new ToolStripMenuItem("Casualty Timeline", null, CasualtyTimeline_Click, (Keys)Shortcut.CtrlD);
            MainMenuStrip.Items.Add(AdvisorsMenu);
            AdvisorsMenu.DropDownItems.Add(ConsultHighCouncilItem);
            AdvisorsMenu.DropDownItems.Add(ChatWithKingsItem);
            AdvisorsMenu.DropDownItems.Add(new ToolStripSeparator());
            AdvisorsMenu.DropDownItems.Add(CityStatusItem);
            AdvisorsMenu.DropDownItems.Add(DefenseMinisterItem);
            AdvisorsMenu.DropDownItems.Add(ForeignMinisterItem);
            AdvisorsMenu.DropDownItems.Add(new ToolStripSeparator());
            AdvisorsMenu.DropDownItems.Add(AttitudeAdvisorItem);
            AdvisorsMenu.DropDownItems.Add(TradeAdvisorItem);
            AdvisorsMenu.DropDownItems.Add(ScienecAdvisorItem);
            AdvisorsMenu.DropDownItems.Add(new ToolStripSeparator());
            AdvisorsMenu.DropDownItems.Add(CasualtyTimelineItem);

            //World menu
            ToolStripMenuItem WorldMenu = new ToolStripMenuItem("World");
            ToolStripMenuItem WondersOfWorldItem = new ToolStripMenuItem("Wonders of the World", null, WondersOfWorld_Click, (Keys)Shortcut.F7);
            ToolStripMenuItem Top5CitiesItem = new ToolStripMenuItem("Top 5 Cities", null, Top5Cities_Click, (Keys)Shortcut.F8);
            ToolStripMenuItem CivScoreItem = new ToolStripMenuItem("Civilization Score", null, CivScore_Click, (Keys)Shortcut.F9);
            ToolStripMenuItem DemographicsItem = new ToolStripMenuItem("Demographics", null, Demographics_Click, (Keys)Shortcut.F11);
            ToolStripMenuItem SpaceshipsItem = new ToolStripMenuItem("Spaceships", null, Spaceships_Click, (Keys)Shortcut.F12);
            MainMenuStrip.Items.Add(WorldMenu);
            WorldMenu.DropDownItems.Add(WondersOfWorldItem);
            WorldMenu.DropDownItems.Add(Top5CitiesItem);
            WorldMenu.DropDownItems.Add(CivScoreItem);
            WorldMenu.DropDownItems.Add(new ToolStripSeparator());
            WorldMenu.DropDownItems.Add(DemographicsItem);
            WorldMenu.DropDownItems.Add(SpaceshipsItem);
        }

        private void MainCiv2Window_Load(object sender, EventArgs e)
        {
            //Forms
            MapForm mapForm = new MapForm(this);
            mapForm.MdiParent = this;
            //mapForm.Size = new Size((int)((ClientSize.Width) * 0.8625), ClientSize.Height - 80);
            mapForm.Location = new Point(0, 0);
            mapForm.Show();
            Console.WriteLine("Mapformsize={0}", mapForm.Size);
            

            statusForm = new StatusForm(this);
            worldMapForm = new WorldMapForm(this);
            cityForm = new CityForm(this);


            //Load forms
            //mapForm.MdiParent = this;
            //mapForm.Size = new Size((int)(screenW * 0.8625), screenH - titleHeight);
            //mapForm.Size = new Size(ClientSize.Width, ClientSize.Height);
            //mapForm.Show();

            //statusForm.MdiParent = this;
            //statusForm.Show();

            //worldMapForm.MdiParent = this;
            //worldMapForm.Show();

            //cityForm.MdiParent = this;
            //cityForm.StartPosition = FormStartPosition.Manual;
            //cityForm.Location = new Point(1260, 0);

            //mapForm.Focus();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        //Create unit from cheat menu!
        private void createUnitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Load unit create form
            CreateUnitForm newMDIChild = new CreateUnitForm();
            newMDIChild.MdiParent = this;
            //newMDIChild1.Size = new System.Drawing.Size(1260, 770);
            //newMDIChild1.BackColor = Color.Black;
            newMDIChild.Show();

        }

        // ADVISORS MENU
        private void ChatWithKings_Click(object sender, EventArgs e) { }
        private void ConsultHighCouncil_Click(object sender, EventArgs e) { }

        private void CityStatus_Click(object sender, EventArgs e)
        {
            CityStatusForm CityStatusForm = new CityStatusForm();
            CityStatusForm.Load += new EventHandler(AdvisorsForm_Load);   //so you set the correct size of form
            CityStatusForm.ShowDialog();
        }

        private void DefenseMinister_Click(object sender, EventArgs e)
        {
            DefenseMinisterForm DefenseMinisterForm = new DefenseMinisterForm();
            DefenseMinisterForm.Load += new EventHandler(AdvisorsForm_Load);   //so you set the correct size of form
            DefenseMinisterForm.ShowDialog();
        }

        private void ForeignMinister_Click(object sender, EventArgs e) { }

        private void AttitudeAdvisor_Click(object sender, EventArgs e)
        {
            AttitudeAdvisorForm AttitudeAdvisorForm = new AttitudeAdvisorForm();
            AttitudeAdvisorForm.Load += new EventHandler(AdvisorsForm_Load);   //so you set the correct size of form
            AttitudeAdvisorForm.ShowDialog();
        }

        private void TradeAdvisor_Click(object sender, EventArgs e)
        {
            TradeAdvisorForm TradeAdvisorForm = new TradeAdvisorForm();
            TradeAdvisorForm.Load += new EventHandler(AdvisorsForm_Load);   //so you set the correct size of form
            TradeAdvisorForm.ShowDialog();
        }

        private void ScienceAdvisor_Click(object sender, EventArgs e)
        {
            ScienceAdvisorForm ScienceAdvisorForm = new ScienceAdvisorForm();
            ScienceAdvisorForm.Load += new EventHandler(AdvisorsForm_Load);   //so you set the correct size of form
            ScienceAdvisorForm.ShowDialog();
        }

        private void CasualtyTimeline_Click(object sender, EventArgs e) { }

        // WORLD MENU
        private void WondersOfWorld_Click(object sender, EventArgs e)
        {
            WondersOfWorldForm WondersOfWorldForm = new WondersOfWorldForm();
            WondersOfWorldForm.Load += new EventHandler(AdvisorsForm_Load);   //so you set the correct size of form
            WondersOfWorldForm.ShowDialog();
        }

        private void Top5Cities_Click(object sender, EventArgs e) { }
        private void CivScore_Click(object sender, EventArgs e) { }

        private void Demographics_Click(object sender, EventArgs e)
        {
            DemographicsForm DemographicsForm = new DemographicsForm();
            DemographicsForm.Load += new EventHandler(AdvisorsForm_Load);   //so you set the correct size of form
            DemographicsForm.ShowDialog();
        }

        private void Spaceships_Click(object sender, EventArgs e) { }

        private void AdvisorsForm_Load(object sender, EventArgs e)
        {
            Form frm = sender as Form;
            frm.Location = new Point(330, 250);
            frm.Width = 622;
            frm.Height = 421;
        }
    }
}

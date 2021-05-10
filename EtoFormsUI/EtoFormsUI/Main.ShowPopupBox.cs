﻿using System;
using System.IO;
using Eto.Forms;
using Eto.Drawing;
using Civ2engine.Events;
using Civ2engine;
using System.Collections.Generic;

namespace EtoFormsUI
{
    public partial class Main : Form
    {
        private string savDirectory, savName;

        private void PopupboxEvent(object sender, PopupboxEventArgs e)
        {
            switch (e.BoxName)
            {
                case "MAINMENU":
                    {
                        var popupbox = new Civ2dialog_v2(this, popupBoxList.Find(p => p.Name == e.BoxName));
                        popupbox.Location = new Point((int)(Screen.PrimaryScreen.Bounds.Width * 0.745), (int)(Screen.PrimaryScreen.Bounds.Height * 0.570));
                        popupbox.ShowModal(Parent);
                        // Load game
                        if (popupbox.SelectedIndex == 4)
                        {
                            using var ofd = new OpenFileDialog
                            {
                                Directory = new Uri(Settings.Civ2Path),
                                Title = "Select Game To Load",
                                Filters = { new FileFilter("Save Files (*.sav)", ".SAV") }
                            };

                            if (ofd.ShowDialog(this.ParentWindow) == DialogResult.Ok)
                            {
                                // Get SAV name & directory name from result
                                savDirectory = ofd.Directory.LocalPath;
                                savName = Path.GetFileName(ofd.FileName);
                                LoadGameInitialization(savDirectory, savName);
                                Sounds.Stop();
                                Sounds.PlaySound("MENUOK.WAV");

                                OnPopupboxEvent?.Invoke(null, new PopupboxEventArgs("LOADOK", new List<string> { Game.GetActiveCiv.LeaderTitle, Game.GetActiveCiv.LeaderName, Game.GetActiveCiv.TribeName, Game.GetGameYearString, Game.DifficultyLevel.ToString() }));
                            }
                        }
                        break;
                    }
                case "GAMEOPTIONS":
                    {
                        var checkboxOptions = new List<bool> { Game.Options.SoundEffects, Game.Options.Music, Game.Options.AlwaysWaitAtEndOfTurn, Game.Options.AutosaveEachTurn,
                            Game.Options.ShowEnemyMoves, Game.Options.NoPauseAfterEnemyMoves, Game.Options.FastPieceSlide, Game.Options.InstantAdvice, Game.Options.TutorialHelp,
                            Game.Options.MoveUnitsWithoutMouse, Game.Options.EnterClosestCityScreen };
                        var popupbox = new Civ2dialog_v2(this, popupBoxList.Find(p => p.Name == e.BoxName), e.ReplaceStrings, checkboxOptions);
                        popupbox.ShowModal(Parent);
                        Game.Options.SoundEffects = popupbox.CheckboxReturnStates[0];
                        Game.Options.Music = popupbox.CheckboxReturnStates[1];
                        Game.Options.AlwaysWaitAtEndOfTurn = popupbox.CheckboxReturnStates[2];
                        Game.Options.AutosaveEachTurn = popupbox.CheckboxReturnStates[3];
                        Game.Options.ShowEnemyMoves = popupbox.CheckboxReturnStates[4];
                        Game.Options.NoPauseAfterEnemyMoves = popupbox.CheckboxReturnStates[5];
                        Game.Options.FastPieceSlide = popupbox.CheckboxReturnStates[6];
                        Game.Options.InstantAdvice = popupbox.CheckboxReturnStates[7];
                        Game.Options.TutorialHelp = popupbox.CheckboxReturnStates[8];
                        Game.Options.MoveUnitsWithoutMouse = popupbox.CheckboxReturnStates[9];
                        Game.Options.EnterClosestCityScreen = popupbox.CheckboxReturnStates[10];
                        break;
                    }
                case "GRAPHICOPTIONS":
                    {
                        var checkboxOptions = new List<bool> { Game.Options.ThroneRoomGraphics, Game.Options.DiplomacyScreenGraphics, Game.Options.AnimatedHeralds, 
                            Game.Options.CivilopediaForAdvances, Game.Options.HighCouncil, Game.Options.WonderMovies };
                        var popupbox = new Civ2dialog_v2(this, popupBoxList.Find(p => p.Name == e.BoxName), e.ReplaceStrings, checkboxOptions);
                        popupbox.ShowModal(Parent);
                        Game.Options.ThroneRoomGraphics = popupbox.CheckboxReturnStates[0];
                        Game.Options.DiplomacyScreenGraphics = popupbox.CheckboxReturnStates[1];
                        Game.Options.AnimatedHeralds = popupbox.CheckboxReturnStates[2];
                        Game.Options.CivilopediaForAdvances = popupbox.CheckboxReturnStates[3];
                        Game.Options.HighCouncil = popupbox.CheckboxReturnStates[4];
                        Game.Options.WonderMovies = popupbox.CheckboxReturnStates[5];
                        break;
                    }
                case "MESSAGEOPTIONS":
                    {
                        var checkboxOptions = new List<bool> { Game.Options.WarnWhenCityGrowthHalted, Game.Options.ShowCityImprovementsBuilt, Game.Options.ShowNonCombatUnitsBuilt,
                            Game.Options.ShowInvalidBuildInstructions, Game.Options.AnnounceCitiesInDisorder, Game.Options.AnnounceOrderRestored,
                            Game.Options.AnnounceWeLoveKingDay, Game.Options.WarnWhenFoodDangerouslyLow, Game.Options.WarnWhenPollutionOccurs,
                            Game.Options.WarnChangProductWillCostShields, Game.Options.ZoomToCityNotDefaultAction };
                        var popupbox = new Civ2dialog_v2(this, popupBoxList.Find(p => p.Name == e.BoxName), e.ReplaceStrings, checkboxOptions);
                        popupbox.ShowModal(Parent);
                        Game.Options.WarnWhenCityGrowthHalted = popupbox.CheckboxReturnStates[0];
                        Game.Options.ShowCityImprovementsBuilt = popupbox.CheckboxReturnStates[1];
                        Game.Options.ShowNonCombatUnitsBuilt = popupbox.CheckboxReturnStates[2];
                        Game.Options.ShowInvalidBuildInstructions = popupbox.CheckboxReturnStates[3];
                        Game.Options.AnnounceCitiesInDisorder = popupbox.CheckboxReturnStates[4];
                        Game.Options.AnnounceOrderRestored = popupbox.CheckboxReturnStates[5];
                        Game.Options.AnnounceWeLoveKingDay = popupbox.CheckboxReturnStates[6];
                        Game.Options.WarnWhenFoodDangerouslyLow = popupbox.CheckboxReturnStates[7];
                        Game.Options.WarnWhenPollutionOccurs = popupbox.CheckboxReturnStates[8];
                        Game.Options.WarnChangProductWillCostShields = popupbox.CheckboxReturnStates[9];
                        Game.Options.ZoomToCityNotDefaultAction = popupbox.CheckboxReturnStates[10];
                        break;
                    }
                case "LOADOK":
                    {
                        var popupbox = new Civ2dialog_v2(this, popupBoxList.Find(p => p.Name == e.BoxName), e.ReplaceStrings);
                        popupbox.ShowModal(Parent);
                        StartGame();
                        Sounds.PlaySound("MENUOK.WAV");
                        break;
                    }
                default: break;
            }
        }
    }
}
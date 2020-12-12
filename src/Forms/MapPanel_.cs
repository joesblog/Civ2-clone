﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Timers;
using ExtensionMethods;
using civ2.Events;
using civ2.Bitmaps;
using civ2.Units;
using civ2.Enums;

namespace civ2.Forms
{
    public partial class MapPanel : Civ2panel
    {
        Game Game => Game.Instance;
        Map Map => Map.Instance;

        private static List<Bitmap> AnimationBitmap;
        private int MapGridVar { get; set; }        // Style of map grid presentation
        private System.Windows.Forms.Timer Timer;   // Timer for blinking (unit or viewing piece), moving unit, etc.
        private AnimationType AnimType;
        private int TimerCounter { get; set; }
        //Label HelpLabel;

        public static event EventHandler<MapEventArgs> OnMapEvent;

        public MapPanel(int _width, int _height) : base(_width, _height, "", false)
        {
            this.Paint += new PaintEventHandler(MapPanel_Paint);

            Game.OnWaitAtTurnEnd += InitiateWaitAtTurnEnd;
            Game.OnUnitEvent += UnitEventHappened;
            Game.OnPlayerEvent += PlayerEventHappened;
            MinimapPanel.OnMapEvent += MapEventHappened;
            StatusPanel.OnMapEvent += MapEventHappened;
            MainWindow.OnMapEvent += MapEventHappened;
            MainWindow.OnCheckIfCityCanBeViewed += CheckIfCityCanBeViewed;

            NoSelectButton ZoomINButton = new NoSelectButton
            {
                Location = new Point(11, 9),
                Size = new Size(23, 23),
                FlatStyle = FlatStyle.Flat,
                //BackgroundImage = ModifyImage.ResizeImage(Images.ZoomIN, 23, 23)
                BackgroundImage = Images.ZoomIN
            };
            ZoomINButton.FlatAppearance.BorderSize = 0;
            Controls.Add(ZoomINButton);
            ZoomINButton.Click += ZoomINclicked;

            NoSelectButton ZoomOUTButton = new NoSelectButton
            {
                Location = new Point(36, 9),
                Size = new Size(23, 23),
                FlatStyle = FlatStyle.Flat,
                //BackgroundImage = ModifyImage.ResizeImage(Images.ZoomOUT, 23, 23)
                BackgroundImage = Images.ZoomOUT
            };
            ZoomOUTButton.FlatAppearance.BorderSize = 0;
            Controls.Add(ZoomOUTButton);
            ZoomOUTButton.Click += ZoomOUTclicked;

            //Uncomment this for help in drawing-logic
            //HelpLabel = new Label
            //{
            //    Location = new Point(1000, 50),
            //    AutoSize = true,
            //    BackColor = Color.White,
            //    Text = "OK"
            //};
            //DrawPanel.Controls.Add(HelpLabel);

            // Add DrawPanel from base control
            Controls.Add(DrawPanel);
            DrawPanel.BackgroundImage = null;
            DrawPanel.BackColor = Color.Black;
            DrawPanel.Paint += DrawPanel_Paint;
            DrawPanel.MouseClick += DrawPanel_MouseClick;

            // Initialize variables
            MapGridVar = 0;
            ViewPiecesMode = Game.ActiveUnit == null;  // If no unit is active at start (all units ended turn or no exist) go to View pieces mode
            if (ViewPiecesMode)
            {
                ActiveXY = Game.ActiveCursorXY; // If NOT in ViewPiecesMode, then ActiveXY will be set equal to currently active unit coords.
                AnimType = AnimationType.ViewPieces;
            }
            else
            {
                AnimType = AnimationType.UnitWaiting;
            }

            MapViewChange(Game.ClickedXY);  // Center the map view

            // Timer for waiting unit/ viewing piece
            Timer = new System.Windows.Forms.Timer();
            Timer.Tick += new EventHandler(Timer_Tick);
            StartAnimation(AnimType);
        }

        private void MapPanel_Paint(object sender, PaintEventArgs e)
        {
            // Title
            StringFormat sf = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            e.Graphics.DrawString($"{Game.PlayerCiv.Adjective} Map", new Font("Times New Roman", 17, FontStyle.Bold), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 20 + 1), sf);
            e.Graphics.DrawString($"{Game.PlayerCiv.Adjective} Map", new Font("Times New Roman", 17, FontStyle.Bold), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 20), sf);
            e.Dispose();
            sf.Dispose();
        }

        // Draw map here
        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            // Calculate these once just for less computations
            //int[] startingSqXY = StartingSqXY;
            int[] startingSqXYpx = StartingSqXYpx;
            //int[] activeXY = ActiveXY;
            int[] activeXYpx = ActiveXYpx;

            Bitmap map = Game.MapRevealed ? Map.Graphic(8, Game.Zoom) : Map.Graphic(Game.WhichCivsMapShown, Game.Zoom);

            if (startingSqXYpx[0] < 0)
            {
                Rectangle rect1 = new Rectangle(MapXdimPx + startingSqXYpx[0], startingSqXYpx[1], -startingSqXYpx[0], DrawPanel.Height);
                Rectangle rect2 = new Rectangle(0, startingSqXYpx[1], DrawPanel.Width + startingSqXYpx[0], DrawPanel.Height);
                e.Graphics.DrawImage(map, 0, 0, rect1, GraphicsUnit.Pixel);
                e.Graphics.DrawImage(map, -startingSqXYpx[0], 0, rect2, GraphicsUnit.Pixel);
            }
            else
            {
                Rectangle rect = new Rectangle(startingSqXYpx[0], startingSqXYpx[1], DrawPanel.Width, DrawPanel.Height);
                e.Graphics.DrawImage(map, 0, 0, rect, GraphicsUnit.Pixel);
            }


            // Unit/viewing piece static
            switch (AnimType)
            {
                case AnimationType.UnitWaiting:
                    {
                        IUnit unit = Game.ActiveUnit;
                        e.Graphics.DrawImage(AnimationBitmap[TimerCounter % 2], unit.Xpx - startingSqXYpx[0], unit.Ypx - startingSqXYpx[1]);
                        break;
                    }
                case AnimationType.UnitMoving:
                    {
                        IUnit unit = Game.ActiveUnit;
                        e.Graphics.DrawImage(AnimationBitmap[Game.ActiveUnit.MovementCounter], unit.LastXYpx[0] - startingSqXYpx[0] - 8 * (Game.Zoom + 8), unit.LastXYpx[1] - startingSqXYpx[1] - 4 * (Game.Zoom + 8));
                        break;
                    }
                case AnimationType.ViewPieces:
                    {
                        if (TimerCounter % 2 == 0) e.Graphics.DrawImage(Images.ViewPiece, activeXYpx[0] - startingSqXYpx[0], activeXYpx[1] - startingSqXYpx[1]);
                        break;
                    }
            }

            map.Dispose();
            e.Dispose();
        }

        private void DrawPanel_MouseClick(object sender, MouseEventArgs e)
        {
            int[] coords = Ext.PxToCoords(e.Location.X, e.Location.Y, Game.Zoom);
            Game.ClickedXY = new int[] { StartingSqXY[0] + coords[0], StartingSqXY[1] + coords[1] };  // Coordinates of clicked square

            if (e.Button == MouseButtons.Left)
            {
                if (Game.GetCities.Any(city => city.X == Game.ClickedXY[0] && city.Y == Game.ClickedXY[1]))    // City clicked
                {
                    if (ViewPiecesMode) ActiveXY = Game.ClickedXY;
                    //CityForm cityForm = new CityForm(this, Game.Cities.Find(city => city.X == ClickedXY[0] && city.Y == ClickedXY[1]));
                    //cityForm.Show();
                }
                else if (Game.GetUnits.Any(unit => unit.X == Game.ClickedXY[0] && unit.Y == Game.ClickedXY[1]))    // Unit clicked
                {
                    int clickedUnitIndex = Game.GetUnits.FindIndex(a => a.X == Game.ClickedXY[0] && a.Y == Game.ClickedXY[1]);
                    if (!Game.GetUnits[clickedUnitIndex].TurnEnded)
                    {
                        Game.ActiveUnit = Game.GetUnits[clickedUnitIndex];
                        ViewPiecesMode = false;
                        OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.SwitchViewMovePieces));
                        StartAnimation(AnimationType.UnitWaiting);
                    }
                    else
                    {
                        //TODO: determine what happens if unit has ended turn...
                    }
                    MapViewChange(Game.ClickedXY);
                }
                else    // Something else clicked
                {
                    if (ViewPiecesMode) ActiveXY = Game.ClickedXY;
                    MapViewChange(Game.ClickedXY);
                }
            }
            else    // Right click
            {
                ViewPiecesMode = true;
                OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.SwitchViewMovePieces));
                ActiveXY = Game.ClickedXY;
                MapViewChange(Game.ClickedXY);
                StartAnimation(AnimationType.ViewPieces);
            }
        }

        private void MapViewChange(int[] newCenterCoords)
        {
            CenterSqXY = newCenterCoords;

            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.MapViewChanged, StartingSqXY, DrawingSqXY));

            DrawPanel.Refresh();
        }

        #region Calculation of coordinates
        // Coordinates in DrawPanel upper left corner where map drawing begins
        private int[] _startingSqXY;
        public int[] StartingSqXY
        {
            get
            {
                int[] centerDistanceXY = Ext.PxToCoords(DrawPanel.Width / 2, DrawPanel.Height / 2, Game.Zoom); // Offset of central tile from panel NW corner
                int[] _centerSqXY = CenterSqXY;
                _startingSqXY = new int[] { _centerSqXY[0] - centerDistanceXY[0], _centerSqXY[1] - centerDistanceXY[1] };

                //if (!Game.Options.FlatEarth)    // Round world --> make sure starting X is never < 0
                //{
                //    if (_startingSqXY[0] < 0)
                //    {
                //        _startingSqXY[0] = _startingSqXY[0] % 
                //    }
                //}

                return _startingSqXY;;
            }
        }



        //// Determines offset to StartingSqXY for drawing of squares on panel edge { left, up, right, down }
        //private int[] _edgePxDrawOffsetXY;
        //private int[] EdgePxDrawOffsetXY
        //{
        //    get
        //    {
        //        int[] startingSqXY = StartingSqXY;
        //        int[] drawingSqXY = DrawingSqXY;
        //        int[] _edgeDrawOffsetXY = new int[] { -2, -2, 2, 2 };     // By default draw 2 squares more in each direction
        //        if (startingSqXY[0] == 0 || startingSqXY[1] == 0)   // Starting on edge
        //        {
        //            _edgeDrawOffsetXY[0] = -Math.Max(Math.Min(startingSqXY[0], 2), 0);
        //            _edgeDrawOffsetXY[1] = -Math.Max(Math.Min(startingSqXY[1], 2), 0);
        //        }
        //        if (startingSqXY[0] == 1 || startingSqXY[1] == 1)  // Starting in 1st row/column
        //        {
        //            _edgeDrawOffsetXY[0] = -1;
        //            _edgeDrawOffsetXY[1] = -1;
        //        }
        //        if (startingSqXY[0] + drawingSqXY[0] == 2 * Map.Xdim)  // On right edge
        //        {
        //            _edgeDrawOffsetXY[2] = 0;
        //            _edgeDrawOffsetXY[3] = Math.Min(Map.Ydim - drawingSqXY[1] - startingSqXY[1], 2);
        //        }
        //        if (startingSqXY[1] + drawingSqXY[1] == Map.Ydim)  // On bottom edge
        //        {
        //            _edgeDrawOffsetXY[2] = Math.Min(2 * Map.Xdim - drawingSqXY[0] - startingSqXY[0], 2);
        //            _edgeDrawOffsetXY[3] = 0;
        //        }
        //        if (startingSqXY[0] + drawingSqXY[0] == 2 * Map.Xdim - 1)  // 1 column left of right edge
        //        {
        //            _edgeDrawOffsetXY[2] = 1;
        //            _edgeDrawOffsetXY[3] = Math.Min(Map.Ydim - drawingSqXY[1] - startingSqXY[1], 2);
        //        }
        //        if (startingSqXY[1] + drawingSqXY[1] == Map.Ydim - 1)  // 1 column up of bottom edge
        //        {
        //            _edgeDrawOffsetXY[2] = Math.Min(2 * Map.Xdim - drawingSqXY[0] - startingSqXY[0], 2);
        //            _edgeDrawOffsetXY[3] = 1;
        //        }

        //        // Now convert it to pixels
        //        _edgePxDrawOffsetXY = new int[] { 32 * _edgeDrawOffsetXY[0], 16 * _edgeDrawOffsetXY[1] };
        //        if (startingSqXY[0] + drawingSqXY[0] == 2 * Map.Xdim)
        //            _edgePxDrawOffsetXY[0] = DrawPanel.Width - (32 + 32 * drawingSqXY[0] - 32 * _edgeDrawOffsetXY[0]);
        //        if (startingSqXY[1] + drawingSqXY[1] == Map.Ydim)
        //            _edgePxDrawOffsetXY[1] = DrawPanel.Height - (16 + 16 * drawingSqXY[1] - 16 * _edgeDrawOffsetXY[1]);

        //        return _edgePxDrawOffsetXY;
        //    }
        //}

        // Squares to be drawn on the panel
        private int[] DrawingSqXY => new int[] { 2 * (int)Math.Ceiling((double)DrawPanel.Width / (8 * (8 + Game.Zoom))), 2 * (int)Math.Ceiling((double)DrawPanel.Height / (4 * (8 + Game.Zoom))) };

        // Center square on the map
        private int[] _centerSqXY;
        private int[] CenterSqXY
        {
            get { return _centerSqXY; }
            set
            {
                int[] centerDistanceXY = Ext.PxToCoords(DrawPanel.Width / 2, DrawPanel.Height / 2, Game.Zoom); // Offset of central tile from panel NW corner
                int[] _startingSqXY = new int[] { value[0] - centerDistanceXY[0], value[1] - centerDistanceXY[1] };
                int[] drawingSqXY = DrawingSqXY;

                int mapWidth = 4 * (8 + Game.Zoom) * (2 * Map.Xdim + 1);
                int mapHeight = 2 * (8 + Game.Zoom) * (2 * Map.Ydim + 1);

                // First determine the Y-central coordinate
                if (mapWidth < DrawPanel.Height)    // Center the map in panel center
                {
                    
                }

                // Limit movement so that map limits are not exceeded
                if (Game.Options.FlatEarth)
                {
                    if (_startingSqXY[0] < 0 && _startingSqXY[1] < 0)    // Movement beyond upper & left edge
                        _startingSqXY = new int[] { 0, 0 };
                    else if ((_startingSqXY[0] + drawingSqXY[0] >= 2 * Map.Xdim) && _startingSqXY[1] < 0)    // Movement beyond upper & right edge
                        _startingSqXY = new int[] { 2 * Map.Xdim - drawingSqXY[0], 0 };
                    else if (_startingSqXY[0] < 0 && (_startingSqXY[1] + drawingSqXY[1] >= Map.Ydim))    // Movement beyond lower & left edge
                        _startingSqXY = new int[] { 0, Map.Ydim - drawingSqXY[1] };
                    else if ((_startingSqXY[0] + drawingSqXY[0] >= 2 * Map.Xdim) && (_startingSqXY[1] + drawingSqXY[1] >= Map.Ydim))    // Movement beyond lower & right edge
                        _startingSqXY = new int[] { 2 * Map.Xdim - drawingSqXY[0], Map.Ydim - drawingSqXY[1] };
                    else if (_startingSqXY[0] < 0)     // Movement beyond left edge
                        _startingSqXY = new int[] { _startingSqXY[1] % 2, _startingSqXY[1] };
                    else if (_startingSqXY[1] < 0)     // Movement beyond upper edge
                        _startingSqXY = new int[] { _startingSqXY[0], _startingSqXY[0] % 2 };
                    else if (_startingSqXY[0] + drawingSqXY[0] >= 2 * Map.Xdim)     // Movement beyond right edge
                        _startingSqXY = new int[] { 2 * Map.Xdim - drawingSqXY[0] - _startingSqXY[1] % 2, _startingSqXY[1] };
                    else if (_startingSqXY[1] + drawingSqXY[1] >= Map.Ydim)     // Movement beyond bottom edge
                        _startingSqXY = new int[] { _startingSqXY[0], Map.Ydim - drawingSqXY[1] - _startingSqXY[0] % 2 };
                }
                else    // ROUND EARTH
                {
                    // Check only movement beyond upper and lower edge
                    if (_startingSqXY[1] < 0)    // Upper edge
                        _startingSqXY[1] = 0;
                    else if (_startingSqXY[1] + drawingSqXY[1] >= Map.Ydim)    // Lower edge
                        _startingSqXY[1] = Map.Ydim - drawingSqXY[1];
                }

                _centerSqXY = new int[] { centerDistanceXY[0] + _startingSqXY[0], centerDistanceXY[1] + _startingSqXY[1] };
            }
        }

        // Currently active box (active unit or viewing piece), civ2 coords
        private int[] _activeXY;
        public int[] ActiveXY 
        {
            get 
            {
                if (Game.ActiveUnit != null)
                    return new int[] { Game.ActiveUnit.X, Game.ActiveUnit.Y };
                else
                    return _activeXY;
            }
            set { _activeXY = value; }
        }

        // Representation in pixels for drawing
        private int[] StartingSqXYpx => new int[] { StartingSqXY[0] * 4 * (8 + Game.Zoom), StartingSqXY[1] * 2 * (8 + Game.Zoom) };
        private int MapXdimPx => 2 * Map.Xdim * 4 * (8 + Game.Zoom);
        private int MapYdimPx => Map.Ydim * 2 * (8 + Game.Zoom);
        private int[] ActiveXYpx => new int[] { 4 * (Game.Zoom + 8) * ActiveXY[0], 2 * (Game.Zoom + 8) * ActiveXY[1] };
        #endregion

        public static bool ViewPiecesMode { get; set; }

        public int ToggleMapGrid()
        {
            MapGridVar++;
            if (MapGridVar > 3) MapGridVar = 0;
            //Options.Grid = (MapGridVar != 0) ? true : false;
            Refresh();
            return MapGridVar;
        }

        public void ZoomOUTclicked(Object sender, EventArgs e) { Game.Zoom--; DrawPanel.Refresh(); }
        public void ZoomINclicked(Object sender, EventArgs e) { Game.Zoom++; DrawPanel.Refresh(); }
        public void MaxZoomINclicked(Object sender, EventArgs e) { Game.Zoom = 16; DrawPanel.Refresh(); }
        public void MaxZoomOUTclicked(Object sender, EventArgs e) { Game.Zoom = 1; DrawPanel.Refresh(); }
        public void StandardZOOMclicked(Object sender, EventArgs e) { Game.Zoom = 8; DrawPanel.Refresh(); }
        public void MediumZoomOUTclicked(Object sender, EventArgs e) { Game.Zoom = 5; DrawPanel.Refresh(); }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DrawAnimation();
            TimerCounter++;
        }

        private void MapEventHappened(object sender, MapEventArgs e)
        {
            switch (e.EventType)
            {
                case MapEventType.MapViewChanged:
                    {
                        MapViewChange(e.CenterSqXY);
                        break;
                    }
                case MapEventType.SwitchViewMovePieces:
                    {
                        if (ViewPiecesMode) StartAnimation(AnimationType.ViewPieces);
                        else StartAnimation(AnimationType.UnitWaiting);
                        break;
                    }
                case MapEventType.ViewPieceMoved:
                    {
                        StartAnimation(AnimationType.ViewPieces);
                        break;
                    }
                case MapEventType.ToggleBetweenCurrentEntireMapView:
                    {
                        DrawPanel.Invalidate(new Rectangle(0, 0, DrawPanel.Width, DrawPanel.Height));
                        Update();
                        break;
                    }
                default: break;
            }
        }

        private void PlayerEventHappened(object sender, PlayerEventArgs e)
        {
            switch (e.EventType)
            {
                case PlayerEventType.NewTurn:
                    {
                        if (Game.ActiveUnit != null) ViewPiecesMode = false;
                        Timer.Stop();
                        TimerCounter = 0;
                        Timer.Start();
                        break;
                    }
            }
        }

        private void UnitEventHappened(object sender, UnitEventArgs e)
        {
            switch (e.EventType)
            {
                // Unit movement animation event was raised
                case UnitEventType.MoveCommand:
                    {
                        TimerCounter = Game.ActiveUnit.MovementCounter;
                        if (TimerCounter == 0) StartAnimation(AnimationType.UnitMoving);
                        DrawAnimation();
                        break;
                    }
                case UnitEventType.StatusUpdate:
                    {
                        StartAnimation(AnimationType.UnitWaiting);
                        break;
                    }
                case UnitEventType.NewUnitActivated:
                    {
                        ActiveXY = new int[] { Game.ActiveUnit.X, Game.ActiveUnit.Y };
                        CenterSqXY = ActiveXY;
                        StartAnimation(AnimationType.UnitWaiting);
                        break;
                    }
            }
        }

        private void InitiateWaitAtTurnEnd(object sender, WaitAtTurnEndEventArgs e)
        {
            ViewPiecesMode = true;
            Timer.Stop();
            TimerCounter = 0;
            Timer.Start();
        }

        // If ENTER pressed when view piece above city --> enter city view
        private void CheckIfCityCanBeViewed(object sender, CheckIfCityCanBeViewedEventArgs e)
        {
            if (ViewPiecesMode && Game.GetCities.Any(city => city.X == ActiveXY[0] && city.Y == ActiveXY[1]))
            {
                //CityForm cityForm = new CityForm(this, Game.Cities.Find(city => city.X == ActiveXY[0] && city.Y == ActiveXY[1]));
                //cityForm.Show();
            }
        }

        private void StartAnimation(AnimationType anim)
        {
            switch (anim)
            {
                case AnimationType.None:
                    Timer.Stop();
                    TimerCounter = 0;
                    break; 
                case AnimationType.UnitWaiting:
                    AnimType = AnimationType.UnitWaiting;
                    Timer.Stop();
                    AnimationBitmap = GetAnimationFrames.UnitWaiting();
                    TimerCounter = 0;
                    Timer.Interval = 200;    // ms                    
                    Timer.Start();
                    break;
                case AnimationType.UnitMoving:
                    AnimType = AnimationType.UnitMoving;
                    AnimationBitmap = GetAnimationFrames.UnitMoving();
                    break;
                case AnimationType.ViewPieces:
                    AnimType = AnimationType.ViewPieces;
                    Timer.Stop();
                    TimerCounter = 0;
                    Timer.Interval = 200;    // ms                    
                    Timer.Start();
                    break;
            }            
        }

        private void DrawAnimation()
        {
            int[] startingSqXY = StartingSqXY;
            int[] activeXY = ActiveXY;

            switch (AnimType)
            {
                case AnimationType.UnitWaiting:
                    {
                        // At new unit turn initially re-draw the whole map
                        if (TimerCounter == 0)
                            DrawPanel.Invalidate(new Rectangle(0, 0, DrawPanel.Width, DrawPanel.Height));
                        else
                            DrawPanel.Invalidate(new Rectangle((activeXY[0] - startingSqXY[0]) * 32, (activeXY[1] - startingSqXY[1]) * 16 - 16, 64, 48));
                        Update();
                        break;
                    }
                case AnimationType.UnitMoving:
                    {
                        DrawPanel.Invalidate(new Rectangle((activeXY[0] - startingSqXY[0]) * 32 - 64, (activeXY[1] - startingSqXY[1]) * 16 - 48, 3 * 64, 3 * 32 + 16));
                        Update();
                        if (TimerCounter == 7)  // Unit has completed movement
                        {
                            // First update world map with new visible tiles
                            Game.UpdateWorldMapAfterUnitHasMoved();

                            // Update the original world map image with image of new location of unit & redraw whole map
                            IUnit unit = Game.Instance.ActiveUnit;
                            // Game.CivsMap[Game.Instance.ActiveCiv.Id] = ModifyImage.MergedBitmaps(Game.CivsMap[Game.Instance.ActiveCiv.Id], AnimationBitmap[TimerCounter], 32 * unit.LastXY[0] - 64, 16 * unit.LastXY[1] - 48);
                            DrawPanel.Invalidate(new Rectangle(0, 0, DrawPanel.Width, DrawPanel.Height));
                            Update();

                            // Then stop animation
                            StartAnimation(AnimationType.None);

                            // Check if unit moved outside map view -> map view needs to be updated
                            if (UnitMovedOutsideMapView)
                            {
                                CenterSqXY = ActiveXY;
                                DrawPanel.Invalidate(new Rectangle(0, 0, DrawPanel.Width, DrawPanel.Height));
                                Update();
                            }
                        }
                        break;
                    }
                case AnimationType.ViewPieces:
                    {
                        // At new unit turn initially re-draw the whole map
                        if (TimerCounter == 0)
                            DrawPanel.Invalidate(new Rectangle(0, 0, DrawPanel.Width, DrawPanel.Height));
                        else
                            DrawPanel.Invalidate(new Rectangle((activeXY[0] - startingSqXY[0]) * 32, (activeXY[1] - startingSqXY[1]) * 16, 64, 32));
                        Update();
                        break;
                    }
            }
        }

        private bool UnitMovedOutsideMapView
        {
            get
            {
                if (ActiveXY[0] >= StartingSqXY[0] + DrawingSqXY[0] ||
                ActiveXY[0] <= StartingSqXY[0] ||
                ActiveXY[1] >= StartingSqXY[1] + DrawingSqXY[1] ||
                ActiveXY[1] <= StartingSqXY[1])
                    return true;
                else
                    return false;
            }
        }

        // Function which sets various XY variables for drawing map on grid
        private void ReturnCoordsAtMapViewChange(int[] proposedCentralCoords)
        {

        }
    }
}
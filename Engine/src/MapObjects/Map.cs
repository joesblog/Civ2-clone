using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Terrains;

namespace Civ2engine.MapObjects
{
    public class Map
    {
        private readonly bool _flat;

        public Map(bool flat, int index)
        {
            _flat = flat;
            MapIndex = index;
        }
        public int MapIndex { get; } = 0;

        public int XDim
        {
            get => _xDim;
            internal set
            {
                _xDim = value;
                XDimMax = value * 2;
            }
        }

        public int XDimMax { get; private set; }
        public int YDim { get; internal set; }
        public int ResourceSeed { get; internal set; }
        public int LocatorXdim { get; internal set; }
        public int LocatorYdim { get; internal set; }
        public bool MapRevealed { get; set; }
        public int WhichCivsMapShown { get; set; }
        public Tile[,] Tile { get; set; }
        public bool IsValidTileC2(int xC2, int yC2)
        {
            var maxX = XDimMax;
            var x = (((xC2 + maxX) % maxX) - yC2 % 2);
            return -1 < x && x < maxX && -1 < yC2 && yC2 < YDim;
        }
        public Tile TileC2(int xC2, int yC2) =>
            Tile
            [((xC2 + XDimMax) % XDimMax - yC2 % 2) / 2,
                yC2]; // Accepts tile coords in civ2-style and returns the correct Tile (you can index beyond E/W borders for drawing round world)
        public bool IsTileVisibleC2(int xC2, int yC2, int civ) => MapRevealed || Tile[( (xC2 + XDimMax) % XDimMax - yC2 % 2 ) / 2, yC2].Visibility[civ];   // Returns V

        private int _zoom;
        private int _xDim;

        public int Zoom     // -7 (min) ... 8 (max), 0=std.
        {
            get => _zoom;
            set => _zoom = Math.Max(Math.Min(value, 8), -7);
        }
        public int Xpx => 4 * (_zoom + 8);    // Length of 1 map square in X
        public int Ypx => 2 * (_zoom + 8);    // Length of 1 map square in Y
        public int[] StartingClickedXY { get; set; }    // Last tile clicked with your mouse on the map. Gives info where the map should be centered (further calculated in MapPanel).
        public List<IslandDetails> Islands { get; set; }
        public double ScaleFactor => XDim * YDim / 4000d;
        
        public IEnumerable<Tile> CityRadius(Tile tile, bool nullForInvalid = false)
        {
            var odd = tile.Odd;
            var offsets = new List<int[]>
            {
                new[] { 0, -2 },
                new[] { -1 + odd, -1 },
                new[] { -1, 0 },
                new[] { -1 + odd, 1 },
                new[] { 0, 2 },
                new[] { odd, 1 },
                new[] { 1, 0 },
                new[] { odd, -1 },
                new[] { odd, 3 },
                new[] { 1 + odd, 1 },
                new[] { 1 + odd, -1 },
                new[] { odd, -3 },
                new[] { -1, -2 },
                new[] { -1, 2 },
                new[] { 1, 2 },
                new[] { 1, -2 },
                new[] { 0, 0 },
                new[] { -1 + odd, -3 },
                new[] { -2 + odd, -1 },
                new[] { -2 + odd, 1 },
                new[] { -1 + odd, 3 },
            };

            return TilesAround(tile, offsets, nullForInvalid);
        }


        public IEnumerable<Tile> DirectNeighbours(Tile candidate)
        {
            var evenOdd = candidate.Odd;
            var offsets = new List<int[]>
            {
                new[] {0 + evenOdd, -1},
                new[] {0 + evenOdd, 1},
                new[] {-1 + evenOdd, 1},
                new[] {-1 + evenOdd, -1}
            };
            return TilesAround(candidate, offsets);
        }

        public IEnumerable<Tile> Neighbours(Tile candidate)
        {
            var odd = candidate.Odd;
            var offsets = new List<int[]>
            {
                new[] {odd, -1},
                new[] {1, 0},
                new[] {odd, 1},
                new[] {0, 2},
                new[] {-1+odd, 1},
                new[] {-1, 0},
                new[] {-1+odd, -1},
                new[] {0, -2},
            };
            return TilesAround(candidate, offsets);
        }

        private IEnumerable<Tile> TilesAround(Tile centre, IEnumerable<int[]> offsets, bool nullForInvalid = false)
        {
            var coords = new [] { (centre.X - centre.Odd) / 2, centre.Y };
            foreach (var offset in offsets)
            {
                var x = coords[0] + offset[0];
                var y = coords[1] + offset[1];

                if (y < 0 || y >= YDim)
                {
                    if (nullForInvalid)
                    {
                        yield return null;
                    }
                    continue;
                }
                if (x < 0 || x >= XDim)
                {
                    if (_flat)
                    {
                        if (nullForInvalid)
                        {
                            yield return null;
                        }

                        continue;
                    }

                    if (x < 0)
                    {
                        x += XDim;
                    }
                    else
                    {
                        x -= XDim;
                    }
                }
                yield return Tile[x, y];
            }
        }

        public void SetAsStartingLocation(Tile tile, int ownerId)
        {
            tile.Visibility[ownerId] = true;
            
            foreach (var radiusTile in CityRadius(tile))
            {
                radiusTile.Visibility[ownerId] = true;
            }
            
            this.AdjustFertilityForCity(tile);
        }

        public bool IsCurrentlyVisible(Tile tile)
        {
            return MapRevealed || tile.UnitsHere.Any(u=> u.Owner.Id == WhichCivsMapShown) ||
                   Neighbours(tile).Any(l => l.UnitsHere.Any(u => u.Owner.Id == WhichCivsMapShown)) ||
                   CityRadius(tile)
                       .Any(t => t.CityHere != null && t.CityHere.Owner.Id == WhichCivsMapShown);
        }
    }
}

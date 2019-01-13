using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PlatformerProject.Effects;
using PlatformerProject.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;

namespace PlatformerProject.Core
{
    class TileMap
    {
        TmxMap map;
        Texture2D tileset;
        Texture2D[] tilesets;
        int tileWidth, tileHeight, tilesetTilesWide, tilesetTilesHigh;
        
        public TileMap(TmxMap map, Texture2D tileset)
        {
            this.map = map;
            this.tileset = tileset;

            tileWidth = this.map.Tilesets[0].TileWidth;
            tileHeight = this.map.Tilesets[0].TileHeight;

            tilesetTilesWide = tileset.Width / tileWidth;
            tilesetTilesHigh = tileset.Height / tileHeight;            
        }

        public TileMap(TmxMap map, Texture2D[] tilesets)
        {
            this.map = map;
            this.tilesets = tilesets;

            tileWidth = this.map.Tilesets[0].TileWidth;
            tileHeight = this.map.Tilesets[0].TileHeight;

            tilesetTilesWide = tilesets[0].Width / tileWidth;
            tilesetTilesHigh = tilesets[0].Height / tileHeight;
        }


        public void DrawTileLayer(GameTime gameTime, SpriteBatch spriteBatch, string layerName, float lightness = 1f)
        {
            var layer = map.Layers[layerName];
            for (var i = 0; i < layer.Tiles.Count; i++)
            {                
                int gid = layer.Tiles[i].Gid;
                
                if (gid == 0) continue;

                int tileFrame = gid - 1;
                int column = tileFrame % tilesetTilesWide;
                int row = (int)Math.Floor((double)tileFrame / tilesetTilesWide);                

                float x = (i % map.Width) * map.TileWidth;
                float y = (float)Math.Floor(i / (double)map.Width) * map.TileHeight;

                if (layer.OffsetX != null) x += (float)layer.OffsetX;
                if (layer.OffsetY != null) y += (float)layer.OffsetY;

                Rectangle tilesetRec = new Rectangle(tileWidth * column + 1, tileHeight * row, tileWidth - 1, tileHeight);
                
                spriteBatch.Draw(tileset, new Rectangle((int)x , (int)y, tileWidth, tileHeight), tilesetRec, new Color(lightness, lightness, lightness));
            }
        }

        public List<Rectangle> GetColls()
        {
            var colls = new List<Rectangle>();
            
            foreach (var coll in map.ObjectGroups["collision"].Objects)
                colls.Add(new Rectangle((int)coll.X, (int)coll.Y, (int)coll.Width, (int)coll.Height));

            for (var i = 0; i < map.Layers["Foreground"].Tiles.Count; i++)
            {                
                if (map.Layers["Foreground"].Tiles[i].Gid == 0) continue;                                

                float x = (i % map.Width) * map.TileWidth;
                float y = (float)Math.Floor(i / (double)map.Width) * map.TileHeight;                

                colls.Add(new Rectangle((int)x, (int)y, tileWidth, tileHeight));                
            }

            return colls;
        }

        public List<Rectangle> GetRectObjects(string str)
        {
            var rects = new List<Rectangle>();
            foreach (var rect in map.ObjectGroups[str].Objects)
                rects.Add(new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height));
            return rects;
        }        

        public List<Vector2> GetPoints(string str)
        {            
            var points = new List<Vector2>();
            foreach (var point in map.ObjectGroups[str].Objects)
                points.Add(new Vector2((float)point.X, (float)point.Y));
            return points;
        }

        public void AddEnemies(GameObjectManager manager)
        {            
            foreach (var enemy in map.ObjectGroups["enemies"].Objects)
            {
                Vector2 pos = new Vector2((float)enemy.X, (float)enemy.Y);

                bool facingRight = false;
                if (enemy.Properties.ContainsKey("direction") && enemy.Properties["direction"] == "right")
                    facingRight = true;
                
                if (enemy.Type == "skeleton")
                {                    
                    bool waitingForever = false;
                    if (enemy.Properties.ContainsKey("waiting") && enemy.Properties["waiting"] == "true")
                        waitingForever = true;
                    manager.CreateSkeleton(pos, facingRight, waitingForever);
                }

                if (enemy.Type == "goblin")
                {
                    manager.CreateGoblin(pos, facingRight);
                }

                if (enemy.Type == "blobman")
                {
                    manager.CreateBlobMan(pos, facingRight);
                }

                if (enemy.Type == "blob")
                {
                    manager.CreateBlob(pos, facingRight);
                }

            }                        
        }
    }
}

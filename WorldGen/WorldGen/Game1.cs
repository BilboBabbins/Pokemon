using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.XPath;
using WorldGen;
using Color = Microsoft.Xna.Framework.Color;

namespace pokemon
{ 
    
    //reigons: halloween, fall, christmas, spring, underwater 
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        const int screenLength = 1000;
        const int screenWidth = 500;
        const int graphLength = 30;
        const int graphWidth = 15;
        const int sqSize = screenLength / graphLength;
        Tile[,] graph = new Tile[graphLength, graphWidth];
        Tile[][] saveGraph = new Tile[graphWidth][];
        Texture2D pixel;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferHeight = screenWidth;
            graphics.PreferredBackBufferWidth = screenLength;
            graphics.ApplyChanges();
            pixel = new Texture2D(graphics.GraphicsDevice, 1, 1);
            pixel.SetData(new Color[] { Color.White });
            int posY = 0;
            int posX = 0;
            for (int a = 0; a < graphWidth; a++)
            {
                for (int b = 0; b < graphLength; b++)
                {
                    graph[b, a] = new Tile(pixel, new Vector2(posX, posY), Color.White, sqSize);
                    posX += sqSize;
                }
                posY += sqSize;
                posX = 0;
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            //deocration : w = water, g = grass, b = tile/reg background, t = tree, d = dirt/path, f = flower
            //buildings/states: s = start, l = loading tile, p = pokecenter, h = house
            MouseState ms = Mouse.GetState();
            KeyboardState ks = Keyboard.GetState();

            for (int a = 0; a < graphWidth; a++)
            {
                for (int b = 0; b < graphLength; b++)
                {
                    graph[b, a].Pressed(ms);
                    if (graph[b, a].clicked && ks.IsKeyDown(Keys.W))
                    {
                        graph[b, a].state = "water";
                        graph[b, a].Tint = Color.DodgerBlue;
                    }
                    else if (graph[b, a].clicked && ks.IsKeyDown(Keys.G))
                    {
                        graph[b, a].state = "grass";
                        graph[b, a].Tint = Color.SpringGreen;
                    }
                    else if (graph[b, a].clicked && ks.IsKeyDown(Keys.B))
                    {
                        graph[b, a].state = "reg background";
                        graph[b, a].Tint = Color.Gray;
                    }
                    else if (graph[b, a].clicked && ks.IsKeyDown(Keys.T))
                    {
                        graph[b, a].state = "tree";
                        graph[b, a - 1].state = "tree";
                        graph[b, a].Tint = Color.ForestGreen;
                        graph[b, a - 1].Tint = Color.ForestGreen;
                    }
                    else if (graph[b, a].clicked && ks.IsKeyDown(Keys.D))
                    {
                        graph[b, a].state = "path";
                        graph[b, a].Tint = Color.Beige;
                    }
                    else if (graph[b, a].clicked && ks.IsKeyDown(Keys.F))
                    {
                        graph[b, a].state = "flower";
                        graph[b, a].Tint = Color.Pink;
                    }
                    else if (graph[b, a].clicked && ks.IsKeyDown(Keys.S))
                    {
                        graph[b, a].state = "starting pos";
                        graph[b, a].Tint = Color.Yellow;
                    }
                    else if (graph[b, a].clicked && ks.IsKeyDown(Keys.L))
                    {
                        graph[b, a].state = "load new";
                        graph[b, a].Tint = Color.AntiqueWhite;
                    }
                    else if (graph[b, a].clicked && ks.IsKeyDown(Keys.P))
                    {
                        graph[b - 1, a].state = "pokecenter";
                        graph[b + 1, a].state = "pokecenter";
                        graph[b, a].state = "pokecenter entrance";
                        graph[b, a - 1].state = "pokecenter";
                        graph[b - 1, a - 1].state = "pokecenter";
                        graph[b + 1, a - 1].state = "pokecenter";

                        graph[b - 1, a].Tint = Color.Red;
                        graph[b + 1, a].Tint = Color.Red;
                        graph[b, a - 1].Tint = Color.Red;
                        graph[b - 1, a - 1].Tint = Color.Red;
                        graph[b + 1, a - 1].Tint = Color.Red;
                        graph[b, a].Tint = Color.PaleVioletRed;
                    }
                    else if (graph[b, a].clicked && ks.IsKeyDown(Keys.H))
                    {
                        graph[b, a].state = "house entrnace";
                        graph[b - 1, a].state = "house";
                        graph[b - 1, a - 1].state = "house";
                        graph[b, a - 1].state = "house";

                        graph[b, a].Tint = Color.SandyBrown;
                        graph[b - 1, a].Tint = Color.Brown;
                        graph[b - 1, a - 1].Tint = Color.Brown;
                        graph[b, a - 1].Tint = Color.Brown;
                    }
                }
            }
          
           
            //save as 2d array


            if (ks.IsKeyDown(Keys.S) && ks.IsKeyDown(Keys.LeftControl))
            {
                string result = JsonConvert.SerializeObject(graph);
                File.WriteAllText("baseMap.txt", result);
            }
            
            //string result = JsonSerializer.Serialize<List<Tile>>(temp);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            // TODO: Add your drawing code here
            for (int a = 0; a < graphWidth; a++)
            {
                for (int b = 0; b < graphLength; b++)
                {
                    graph[b, a].drawSquare(spriteBatch, 3);

                }
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmashBros.Model;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace SmashBros.System
{
    /// <summary>
    /// Serializing loads different models from json textfiles. 
    /// The models folder is defined at top of document
    /// 
    /// During development phase 
    /// this class is also used to generate the models. And save to the modles folder
    /// </summary>
    public static class Serializing
    {
        private const string MapFolder = "Maps";
        private const string CharacterFolder = "Characters";
        private const string ControllersFolder = "PlayerControllers";

        /// <summary>
        /// Loads all the map models
        /// </summary>
        /// <returns>List with maps</returns>
        public static List<Map> LoadMaps()
        {
            List<Map> models = new List<Map>();
            foreach (var file in Directory.GetFiles(MapFolder))
            {
                models.Add(JsonConvert.DeserializeObject<Map>(Read(file)));
            }

            return models;
        }

        /// <summary>
        /// Loads all the character models
        /// </summary>
        /// <returns>List with all the character models</returns>
        public static List<Character> LoadCharacters()
        {
            
            List<Character> models = new List<Character>();
            foreach (var file in Directory.GetFiles(CharacterFolder))
            {
                models.Add(JsonConvert.DeserializeObject<Character>(Read(file)));
            }

            return models;
        }

        /// <summary>
        /// Loads the players c
        /// </summary>
        /// <returns>List with players</returns>
        public static List<Player> LoadPlayerControllers()
        {
            List<Player> models = new List<Player>();
            foreach (var file in Directory.GetFiles(ControllersFolder))
            {
                models.Add(JsonConvert.DeserializeObject<Player>(Read(file)));
            }

            return models;
        }

        /// <summary>
        /// Method for generating the models
        /// Only for the testing phase of the game
        /// </summary>
        public static void GenereateModels()
        {
            CreateCharacterModel();
            CreateMapModels();
            CreateControllerModels();
        }

        /// <summary>
        /// Genereates some charactermodels
        /// </summary>
        private static void CreateCharacterModel()
        {
            if (!Directory.Exists(CharacterFolder))
            {
                Directory.CreateDirectory(CharacterFolder);
            }
            for (int i = 0; i < 10; i++)
            {
                Character c = new Character();
                c.animations = "Characters/SpidermanThumb" + i;
                c.thumbnail = "Characters/WolverineThumb";
                c.image = "Characters/WolverineImage";
                
                if (i == 1)
                {
                    c.thumbnail = "Characters/SpidermanThumb";
                    c.image = "Characters/SpidermanImage";


                }
                Write(c,CharacterFolder,"Character"+i);
            }
        }

        /// <summary>
        /// Genereates some map models
        /// </summary>
        private static void CreateMapModels()
        {
            if (!Directory.Exists(MapFolder))
            {
                Directory.CreateDirectory(MapFolder);
            }

            for (int i = 0; i < 10; i++)
            {
                Map map = new Map();
                map.name = "New Place City";
                map.bgImage = "Maps/CityMap";
                
                map.AddBox(600, 980, 330, 100, -9.5f);
                map.AddBox(1100, 1270, 540, 450);
                map.AddBox(1640, 1230, 560, 540);
                map.AddBox(100/*630*/, 1200, 1000/*420*/, 570);

                map.AddFloatBox(1120, 830, 560);
                map.AddFloatBox(1160, 630, 740);


                map.AddStartPos(650, 800);
                map.AddStartPos(1700, 850);
                map.AddStartPos(850, 700);
                map.AddStartPos(1400, 700);

                Write(map, MapFolder, "Map" + i);
            }

        }

        public static void CreateControllerModels()
        {
            if (!Directory.Exists(ControllersFolder))
            {
                Directory.CreateDirectory(ControllersFolder);
            }
            for (int i = 0; i < 4; i++)
            {
                Player player = new Player();
                player.PlayerIndex = i;

                //Creating keyboard for player 1 and 2
                if (i == 0 || i == 1)
                {
                    player.KeyboardEnabled = true;
                    player.KeyboardBack = Keys.Escape;
                    player.KeyboardStart = Keys.Enter;
                }
                if (i == 0)
                {
                    player.Color = Color.Blue;
                    player.KeyboardUp = Keys.W;
                    player.KeyboardDown = Keys.S;
                    player.KeyboardLeft = Keys.A;
                    player.KeyboardRight = Keys.D;
                    player.KeyboardHit = Keys.V;
                    player.KeyboardSuper = Keys.B;
                    player.KeyboardSheild = Keys.N;
                }
                else if (i == 1)
                {
                    player.Color = Color.Green;
                    player.KeyboardUp = Keys.Up;
                    player.KeyboardDown = Keys.Down;
                    player.KeyboardLeft = Keys.Left;
                    player.KeyboardRight = Keys.Right;
                    player.KeyboardHit = Keys.J;
                    player.KeyboardSuper = Keys.K;
                    player.KeyboardSheild = Keys.L;
                }
                else if (i == 2)
                {

                }
                else if (i == 3)
                {

                }

                Write(player, ControllersFolder, "Player" + i);
            }
        }

        /// <summary>
        /// Serializes the models to json and wirtes the to files
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="folder"></param>
        /// <param name="fileNoExt">File without extension</param>
        private static void Write(object obj, string folder, string fileNoExt)
        {
            
            using (StreamWriter outfile = new StreamWriter(Path.Combine(folder,fileNoExt+".txt")))
            {
                outfile.Write(JsonConvert.SerializeObject(obj, Formatting.Indented));
            }
        }

        private static string Read(string filename)
        {
            string text;

            using (StreamReader streamReader = new StreamReader(filename))
            {
                text = streamReader.ReadToEnd();
                streamReader.Close();
            }

            return text;
        }

    }
}

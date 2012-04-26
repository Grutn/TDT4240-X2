using System;
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
                Character c = new Character() { 
                    maxSpeed = 5,
                    acceleration = 10,
                    weight = 100,
                    jumpStartVelocity = 10,
                    gravity = 20,

                    ani_noneStart = 0,
                    ani_noneEnd = 2,
                    ani_runStart = 2,
                    ani_runEnd = 13,
                    ani_brake = 44,
                    ani_jumpStart = 18,
                    ani_jumpEnd = 24,
                    ani_fallStart = 24,
                    ani_fallEnd = 26,
                    ani_landStart = 26,
                    ani_landEnd = 28,
                    ani_takeHitStart = 14,
                    ani_takeHitEnd = 16
                };

                c.animations = "Characters/SpidermanAnimation";
                c.thumbnail = "Characters/WolverineThumb";
                c.image = "Characters/WolverineImage";
                c.a = new Move()
                {
                    range = false,
                    adjustable = false,
                    minWait = 0,
                    maxWait = 0,
                    minPower = 10,
                    maxPower = 10,
                    minDamage = 10,
                    maxDamage = 10,
                    sqRange = 0.8f,
                    bodyRange = 1,
                    sqWidth = 10,
                    sqHeight = 10,
                    duration = 250,
                    aniFrom = 28,
                    aniTo = 35
                };
                
                if (i == 1)
                {
                    c.thumbnail = "Characters/SpidermanThumb";
                    c.image = "Characters/SpidermanImage";
                    c.sound_selected = "Sound/Spiderman/selected";
                    c.sound_jump = "Sound/Spiderman/jump";
                    c.sound_kill = "Sound/Wolwerine/kill";
                    c.sound_punch = "Sound/Spiderman/punch";
                }

                if (i == 2)
                {
                    c.thumbnail = "Characters/RoketRacoonThumb";
                    c.image = "Characters/RoketRacoonPose";
                    c.animations = "Characters/RoketRacoonAnimation";
                    c.ani_noneStart = 0;
                    c.ani_noneEnd = 0;
                    c.ani_runStart = 1;
                    c.ani_runEnd = 12;
                    c.ani_brake = 35;
                    c.ani_jumpStart = 16;
                    c.ani_jumpEnd = 20;
                    c.ani_fallStart = 20;
                    c.ani_fallEnd = 20;
                    c.ani_landStart = 20;
                    c.ani_landEnd = 22;
                    c.ani_takeHitStart = 12;
                    c.ani_takeHitEnd = 14;

                    c.a = new Move()
                    {
                        range = false,
                        adjustable = false,
                        minWait = 0,
                        maxWait = 0,
                        minPower = 1,
                        maxPower = 1,
                        minDamage = 70,
                        maxDamage = 100,
                        sqRange = 0.8f,
                        bodyRange = 1,
                        sqWidth = 10,
                        sqHeight = 10,
                        duration = 250,
                        aniFrom = 42,
                        aniTo = 53
                    };

                    c.aUp = new Move()
                    {
                        range = false,
                        adjustable = false,
                        minWait = 0,
                        maxWait = 0,
                        minPower = 10,
                        maxPower = 10,
                        minDamage = 10,
                        maxDamage = 10,
                        sqRange = 0.8f,
                        bodyRange = 1,
                        sqWidth = 10,
                        sqHeight = 10,
                        duration = 250,
                        aniFrom = 29,
                        aniTo = 34
                    };
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
                map.bgImage = "Maps/CityBg";
                map.mapImage = "Maps/CityMap";
                map.backgroundMusic = "Sound/main";
                map.size = new Box(3500, 1600, -800, 0);
                map.zoomBox = new Box(3000, 1300, -800, 100);

                int mx = -420;

                map.AddBox(610+mx, 960, 300, 100, -9.3f);
                map.AddBox(790 + mx, 955, 120, 100, 17f);
                map.AddBox(1100 + mx, 1270, 560, 450);
                map.AddBox(1670 + mx, 1230, 580, 540);
                map.AddBox(650 + mx, 1240, 410, 570);

                map.AddFloatBox(1170 + mx, 620, 550);
                map.AddFloatBox(1125 + mx, 815, 730);


                map.AddStartPos(650 + mx, 800);
                map.AddStartPos(1700 + mx, 850);
                map.AddStartPos(850 + mx, 700);
                map.AddStartPos(1400 + mx, 700);

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

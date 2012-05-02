using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Reflection;
using SmashBros.Models;

namespace SmashBros.MySystem
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
        private const string PowerUpFolder = "PowerUps";

        #region Load & Save Models

        /// <summary>
        /// Loads all the map models
        /// </summary>
        /// <returns>List with maps</returns>
        static List<Map> maps;
        public static List<Map> LoadMaps()
        {
            maps = LoadFolder<Map>(MapFolder);
            return maps;
        }

        /// <summary>
        /// Loads all the character models
        /// </summary>
        /// <returns>List with all the character models</returns>
        static List<CharacterStats> charlist;
        public static List<CharacterStats> LoadCharacters()
        {
            charlist =  LoadFolder<CharacterStats>(CharacterFolder);
            return charlist;
        }

        /// <summary>
        /// Loads the players c
        /// </summary>
        /// <returns>List with players</returns>
        public static List<Player> LoadPlayerControllers()
        {
            return LoadFolder<Player>(ControllersFolder);
        }

        /// <summary>
        /// Loads the game options that where used last game
        /// </summary>
        /// <returns></returns>
        public static GameOptions LoadGameOptions()
        {
            return JsonConvert.DeserializeObject<GameOptions>(Read("gameOptions.txt"));
        }

        public static List<PowerUp> LoadPowerUps()
        {
            return LoadFolder<PowerUp>(PowerUpFolder);
        }

        private static List<T> LoadFolder<T>(string folder)
        {
            List<T> models = new List<T>();
            foreach (var file in Directory.GetFiles(folder))
            {
                models.Add(JsonConvert.DeserializeObject<T>(Read(file)));
            }

            return models;
        }

        /// <summary>
        /// Saves game options
        /// </summary>
        /// <param name="options"></param>
        public static void SaveGameOptions(GameOptions options)
        {
            SaveFile("gameOptions.txt", options);
        }

        #endregion
        
        #region Model Generating Code

        /// <summary>
        /// Method for generating the models
        /// Only for the testing phase of the game
        /// </summary>
        public static void GenereateModels()
        {
            if (!File.Exists("gameOptions.txt"))
            {
                using (StreamWriter outfile = new StreamWriter("gameOptions.txt"))
                {
                    outfile.Write(JsonConvert.SerializeObject(new GameOptions(), Formatting.Indented));
                }
            }
            CreateCharacterModel();
            CreateMapModels();
            CreateControllerModels();
            CreatePowerUpModels();
        }

        /// <summary>
        /// Genereates some charactermodels
        /// </summary>
        /// 

        private static void CreateCharacterModel()
        {
            if (!Directory.Exists(CharacterFolder))
            {
                Directory.CreateDirectory(CharacterFolder);
            }
            for (int i = 0; i < 4; i++)
            {
                CharacterStats c = new CharacterStats()
                {
                    maxSpeed = 5,
                    acceleration = 10,
                    weight = 100,
                    jumpStartVelocity = 10,
                    gravity = 20,
                    name = "",
                    size = new Vector2(50, 120),

                    sound_jump = null,
                    sound_kill = null,
                    sound_punch = null,
                    sound_selected = null,
                    sound_won = null,

                    ani_noneStart = 0,
                    ani_noneEnd = 0,
                    ani_runStart = 0,
                    ani_runEnd = 11,
                    ani_brake = 42,
                    ani_jumpStart = 16,
                    ani_jumpEnd = 22,
                    ani_fallStart = 22,
                    ani_fallEnd = 24,
                    ani_landStart = 24,
                    ani_landEnd = 26,
                    ani_takeHitStart = 12,
                    ani_takeHitEnd = 1
                };

                c.animations = "Characters/WolverineAnimation";
                c.thumbnail = "Characters/WolverineThumb";
                c.image = "Characters/WolverinePose";
                c.moveAnimations = "GameStuff/RangeAttacks";

                //c.a = new MoveStats(10, 5000, 1500, 3500, 26, 35, 0, new Vector2(10, 0), new Vector2(20, 15), new Vector2(50, -5), new Vector2(10, 10));
                //c.aUp = new ChargeMove(30, 5000, 0, 250, 0, 0, 0, new Vector2(0, -20), new Vector2(0, -10), new Vector2(0, -50), new Vector2(10, 10),
                //    400, 2000, 0, 0, 0, 0);
                //c.aLR = new ChargeMove(30, 5000, 0, 250, 0, 0, 0, new Vector2(16, -3), new Vector2(10, 0), new Vector2(50, 0), new Vector2(10, 10),
                //    400, 2000, 0, 0, 0, 0);
                //c.aDown = new MoveStats(10, 5000, 0, 250, 28, 35, 0, new Vector2(5, 10), new Vector2(10, 10), new Vector2(30, 50), new Vector2(10, 10));

                //c.x = new RangeMove(3, 5000, 200, 0, 0, 0, new Vector2(1, 0), new Vector2(40, 0), new Vector2(10, 10), new Vector2(20, 0), 0, 0);
                //c.xLR = new MoveStats(10, 5000, 0, 100, 0, 0, 0, new Vector2(10, 10), new Vector2(0, 0), new Vector2(100, 100), new Vector2(50, 50));
                //c.xUp = new BodyMove(10, 5000, 300, 1400, 0, 0, 0, new Vector2(5, 5), new Vector2(0, 0), new Vector2(0, 0), new Vector2(30, 30), new Vector2(10, 20), 100, 1000);
                //c.aDown = new MoveStats(10, 5000, 0, 100, 0, 0, 0, new Vector2(10, 10), new Vector2(0, 0), new Vector2(100, 100), new Vector2(50, 50));
                

                if (i == 0)
                {
                    c.animations = "Characters/WolverineAnimation";
                    c.thumbnail = "Characters/WolverineThumb";
                    c.image = "Characters/WolverinePose";
                    c.sound_selected = "Sound/Wolverine/selected";
                    c.sound_jump = "Sound/Spiderman/jump";
                    c.sound_kill = "Sound/Wolwerine/kill";
                    c.sound_punch = "Sound/Spiderman/punch";
                    c.size = new Vector2(40, 120);
                    c.moveAnimations = "GameStuff/RangeAttacks";

                    c.ani_noneStart = 0;
                    c.ani_noneEnd = 0;
                    c.ani_runStart = 1;
                    c.ani_runEnd = 13;
                    c.ani_jumpStart = 14;
                    c.ani_jumpEnd = 19;
                    c.ani_fallStart = 19;
                    c.ani_fallEnd = 22;
                    c.ani_landStart = 24;
                    c.ani_landEnd = 26;
                    c.ani_takeHitStart = 26;
                    c.ani_takeHitEnd = 27;
                    c.ani_brake = 26;

                    c.size = new Vector2(40, 120);

                    c.a = new MoveStats(10, 500, 150, 300, 28, 34, 0, new Vector2(10, 0), new Vector2(20, 15), new Vector2(50, -5), new Vector2(10, 10));
                    c.xDown = new BodyMove(30, 1000, 450, 700, 40, 52, 0, new Vector2(0, -10), new Vector2(0, -10), new Vector2(0, -50), new Vector2(100, 10), new Vector2(0, -3), 450, 750);
                    //c.aLR = new BodyMove(30, 1000, 100, 250, 67, 80, 0, new Vector2(0, -20), new Vector2(0, 20), new Vector2(0, -40), new Vector2(100, 10), new Vector2(0, -5), 450, 600);
                    c.xUp = new BodyMove(10, 500, 275, 360, 53, 71, 0, new Vector2(-3, 20), new Vector2(0, -50), new Vector2(0, -50), new Vector2(100, 10), new Vector2(0, -12), 100, 150);
                    c.xLR = new BodyMove(20, 1000, 300, 650, 72, 86, 0, new Vector2(20, -1), new Vector2(50, -10), new Vector2(50, -10), new Vector2(10, 30), new Vector2(10.0f, -1.0f), 220, 700);
                }

                if (i == 1)
                {
                    c.thumbnail = "Characters/SpidermanThumb";
                    c.image = "Characters/SpidermanPose";
                    c.animations = "Characters/SpidermanAnimation";
                    c.sound_selected = "Sound/Spiderman/selected";
                    c.sound_jump = "Sound/Spiderman/jump";
                    c.sound_kill = "Sound/Wolwerine/kill";
                    c.sound_punch = "Sound/Spiderman/punch";

                    c.size = new Vector2(40, 120);

                    c.a = new MoveStats(10, 500, 150, 300, 26, 32, 0, new Vector2(10, 0), new Vector2(20, 15), new Vector2(50, -5), new Vector2(10, 10));
                    c.aUp = new BodyMove(10, 500, 275, 360, 32, 42, 0, new Vector2(-3, -20), new Vector2(0, -60), new Vector2(0, -60), new Vector2(10, 10), new Vector2(0, -5.0f), 100, 300);
                    c.aLR = new ChargeMove(30, 500, 100, 250, 73, 80, 0, new Vector2(20, -20), new Vector2(40, 40), new Vector2(45, -20), new Vector2(10, 10), 50, 250, 67, 69, 69, 73);
                    c.x = new RangeMove(5, 500, 340, 43, 50, 0, new Vector2(1, 1), new Vector2(40, 0), new Vector2(30, 100), new Vector2(13, -3), 0, 0, true);
                    c.xLR = new BodyMove(20, 1000, 500, 850, 51, 66, 0, new Vector2(20, -1), new Vector2(50, 0), new Vector2(50, 0), new Vector2(10, 30), new Vector2(10.0f, -1.0f), 220, 700);
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

                    c.size = new Vector2(30, 80);

                    c.a = new MoveStats(100, 250, 0, 250, 23, 28, 0, new Vector2(1, 0), new Vector2(0, 0), new Vector2(1, 0), new Vector2(10, 10));
                    c.aUp = new MoveStats(10, 250, 0, 250, 29, 34, 0, new Vector2(2, 10), new Vector2(-10, 0), new Vector2(10, -40), new Vector2(10, 10));
                    c.x = new RangeMove(10, 500, 310, 36, 42, 0, new Vector2(3, 0), new Vector2(25, 0), new Vector2(5, 5), new Vector2(20, 0), 3, 3);
                    c.xLR = new RangeMove(25, 500, 350, 42, 54, 0, new Vector2(0, 0), new Vector2(30, -5), new Vector2(27, 20), new Vector2(3, 0), 1, 1, false, false, null, true, -1, (float)Math.PI, 54, 55);
                    c.xDown = new RangeMove(25, 500, 350, 56, 62, 0, new Vector2(0, 0), new Vector2(0, 37), new Vector2(10, 5), new Vector2(0, 0), 2, 2);
                }
                else if (i == 3)
                {
                    c.thumbnail = "Characters/SupermanFatThumb";
                    c.image = "Characters/SupermanFatPose";
                    c.animations = "Characters/SupermanFatAnimation";
                    c.ani_noneStart = 0;
                    c.ani_noneEnd = 0;
                    c.ani_runStart = 1;
                    c.ani_runEnd = 10;
                    c.ani_brake = 40;
                    c.ani_jumpStart = 11;
                    c.ani_jumpEnd = 16;
                    c.ani_fallStart = 16;
                    c.ani_fallEnd = 16;
                    c.ani_landStart = 17;
                    c.ani_landEnd = 19;
                    c.ani_takeHitStart = 57;
                    c.ani_takeHitEnd = 59;

                    c.a = new MoveStats(10, 600, 300, 400, 20, 33, 0, new Vector2(1, 0.5f), new Vector2(0,0), new Vector2(50,-10), new Vector2(10,10));
                    c.aLR = new ChargeMove(10, 500, 100, 400, 25, 33, 0, new Vector2(10, 1), new Vector2(30, -10), new Vector2(60, -10), new Vector2(30, 5), 500, 3000, 20, 23, 23, 25);
                    c.aUp = new MoveStats(10, 500, 200, 500, 33, 39, 0, new Vector2(10, 10), new Vector2(0, 0), new Vector2(30, 30), new Vector2(10, 10));
                    c.xUp = new BodyMove(20, 10000, 100, 9000, 11, 16, 0, new Vector2(10, 10), new Vector2(0, -40), new Vector2(0, -40), new Vector2(30, 5), new Vector2(0, -10), 1000, -1, (float)Math.PI, Math.PI / 2, 16, 16);// after = 17 - 19);
                    c.x = new MoveStats(10, 500, 100, 400, 47, 56, 0, new Vector2(5, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(10, 10));
                    c.xLR = new BodyMove(10, 500, 100, 400, 41, 46, 0, new Vector2(10, 1), new Vector2(0, 0), new Vector2(0, 0), new Vector2(10, 10), new Vector2(5, 2));
                    c.xDown = new BodyMove(10, 1000, 100, 1000, 60, 74, 0, new Vector2(10, 10), new Vector2(0, 40), new Vector2(0, 40), new Vector2(50, 14), new Vector2(0, 12f));
                }
                Write(c, CharacterFolder, "Character" + i);
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

            for (int i = 0; i < 5; i++)
            {
                Map map = new Map();
                map.name = "New Place City";
                map.bgImage = "Maps/CityBg";
                map.mapImage = "Maps/CityMap";
                map.thumbImage = "Maps/CityMapThumb";
                map.backgroundMusic = "Sound/Game/HeroStadium";
                map.size = new Box(3500, 1600, -800, 0);
                map.zoomBox = new Box(10000, 10000, -5000, -5000);
                map.DropZone = new Box(200, 800, 500, 100);


                if (i == 0)
                {
                    map.name = "Hero Stadium";
                    map.bgImage = "Maps/CityBg";
                    map.mapImage = "Maps/HeroStadium";
                    map.thumbImage = "Maps/HeroStadiumThumb";
                    map.size = new Box(3400, 2000, -1100, -1200);
                    map.zoomBox = new Box(2500, 1370, -570, -570);
                    map.DropZone = new Box(1300, 415, 50, -330);

                    map.AddBox(690, 270, 1300, 70);
                    map.AddBox(705, 376, 1155, 145);
                    map.AddBox(700, 636, 250, 370);

                    map.AddFloatBox(700, 50, 515);

                    map.AddStartPos(280, 140);
                    map.AddStartPos(1170, 140);
                    map.AddStartPos(530, -70);
                    map.AddStartPos(870, -70);
                }
                else if (i == 1)
                {
                    map.name = "Grass Town";
                    map.bgImage = "Maps/CloudBg";
                    map.mapImage = "Maps/GrassTown";
                    map.thumbImage = "Maps/GrassTownThumb";
                    map.size = new Box(4000, 2600, -1000, -800);
                    map.zoomBox = new Box(3160, 2300, -560, -580);
                    map.DropZone = new Box(1800, 900, 75, -176);

                    map.AddBox(205,850, 310, 250);
                    map.AddBox(980,880, 805, 265);
                    map.AddBox(1755, 860, 315, 245);

                    map.AddFloatBox(545, 512, 325);
                    map.AddFloatBox(1385, 524, 445);

                    map.AddStartPos(750, 614);
                    map.AddStartPos(1260, 614);
                    map.AddStartPos(192, 610);
                    map.AddStartPos(1750, 630);
                }
                else
                {

                    map.AddBox(680, 1270, 620, 450);
                    map.AddBox(1245, 990, 540, 60);
                    map.AddBox(1235, 1290, 520, 540);
                    map.AddBox(245, 1200, 360, 570);

                    map.AddFloatBox(765, 620, 485);
                    map.AddFloatBox(705, 815, 670);


                    int mx = -420;
                    map.AddStartPos(650 + mx, 800);
                    map.AddStartPos(1700 + mx, 850);
                    map.AddStartPos(850 + mx, 700);
                    map.AddStartPos(1400 + mx, 700);
                }

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
                }
                if (i == 0)
                {
                    player.KeyboardStart = Keys.Enter;
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
                    player.KeyboardBack = Keys.Back;
                    player.KeyboardStart = Keys.D9;

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
                    player.KeyboardBack = Keys.Back;
                    player.Color = Color.Yellow;
                    player.KeyboardUp = Keys.NumPad8;
                    player.KeyboardDown = Keys.NumPad5;
                    player.KeyboardLeft = Keys.NumPad4;
                    player.KeyboardRight = Keys.NumPad6;
                    player.KeyboardHit = Keys.NumPad1;
                    player.KeyboardSuper = Keys.NumPad2;
                    player.KeyboardSheild = Keys.NumPad3;
                }
                else if (i == 3)
                {

                }

                Write(player, ControllersFolder, "Player" + i);
            }
        }

        public static void CreatePowerUpModels()
        {
            if (!Directory.Exists(PowerUpFolder))
            {
                Directory.CreateDirectory(PowerUpFolder);
            }

            Random r = new Random();
            for (int i = 0; i < 21; i++)
            {
                PowerUp p = new PowerUp()
                {
                    acceleration = r.Next(0, 10),
                    duration = r.Next(5, 15),
                    jumpStartVelocity = r.Next(0, 10),
                    maxSpeed = r.Next(0, 10),
                    weight = r.Next(0, 100),
                    imageFrame = i
                };

                Write(p, PowerUpFolder, "PowerUp" + i);
            }
        }

        #endregion

        #region WriteFiles & Debug runtime Update

        /// <summary>
        /// Method used for saving file to fileName
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="obj"></param>
        private static void SaveFile(string filename, object obj)
        {
            using (StreamWriter outfile = new StreamWriter(filename))
            {
                outfile.Write(JsonConvert.SerializeObject(obj, Formatting.Indented));
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

            using (StreamWriter outfile = new StreamWriter(Path.Combine(folder, fileNoExt + ".txt")))
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


        public static void Reload()
        {
            int i = 0;
            foreach (var file in Directory.GetFiles(CharacterFolder))
            {
                CharacterStats newC = (CharacterStats)JsonConvert.DeserializeObject<CharacterStats>(Read(file));
                CharacterStats c = charlist[i];
                Update(c, newC);


                i++;
            }


            i = 0;
            foreach (var file in Directory.GetFiles(MapFolder))
            {
                Map newC = (Map)JsonConvert.DeserializeObject<Map>(Read(file));
                Map c = maps[i];
                Update(c, newC);


                i++;
            }

        }

        private static void Update(object copyObject, object o)
        {
            Type type = o.GetType();

            while (type != null)
            {

                FieldInfo[] myObjectFields = type.GetFields(
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                foreach (FieldInfo fi in myObjectFields)
                {
                    fi.SetValue(copyObject, fi.GetValue(o));
                }

                type = type.BaseType;
            }
        } 
        #endregion

    }
}

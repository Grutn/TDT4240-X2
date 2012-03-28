using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmashBros.Model;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;

namespace SmashBros.System
{
    public static class Serializing
    {
        private const string MapPlace = "Maps";
        private const string CharacterPlace = "Characters";

        /// <summary>
        /// Loads all the map models
        /// </summary>
        /// <returns>List with maps</returns>
        public static List<Map> LoadMaps()
        {
            List<Map> models = new List<Map>();
            foreach (var file in Directory.GetFiles(MapPlace))
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
            foreach (var file in Directory.GetFiles(CharacterPlace))
            {
                models.Add(JsonConvert.DeserializeObject<Character>(Read(file)));
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
            CreateMapModel();
        }

        /// <summary>
        /// Genereates some charactermodels
        /// </summary>
        private static void CreateCharacterModel()
        {
            if (!Directory.Exists(CharacterPlace))
            {
                Directory.CreateDirectory(CharacterPlace);
            }
            for (int i = 0; i < 10; i++)
            {
                Character c = new Character();
                c.animations = "CharAnimation" + i;
                c.thumbnail = "Characters/WolverineThumb";
                if (i == 1)
                {
                    c.thumbnail = "Characters/SpidermanThumb";

                }
                Write(c,CharacterPlace,"Character"+i);
            }
        }

        /// <summary>
        /// Genereates some map models
        /// </summary>
        private static void CreateMapModel()
        {
            if (!Directory.Exists(MapPlace))
            {
                Directory.CreateDirectory(MapPlace);
            }
            for (int i = 0; i < 10; i++)
            {
                Map map = new Map();
                
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

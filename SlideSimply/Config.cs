using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;


namespace SlideSimply
{
    [Serializable]
    class Config
    {
        public string Directory { get; set; }
        public bool ScaleOnFullScreen { get; set; }
        public int SlideInterval { get; set; }

        public void Save(string filename)
        {
            var formatter = new SoapFormatter();
            using (var s = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write))
                formatter.Serialize(s, this);
        }

        public static Config Load(string filename)
        {
            Config cfg;
            if(!File.Exists(filename))
            {
                cfg = new Config();
                cfg.Directory = "";
                cfg.ScaleOnFullScreen = false;
                cfg.SlideInterval = 5;
                cfg.Save(filename);
            }
            else
            {
                var formatter = new SoapFormatter();
                using (var s = new FileStream(filename, FileMode.Open))
                    cfg = (Config)formatter.Deserialize(s);
            }
            return cfg;
        }
        
    }
}

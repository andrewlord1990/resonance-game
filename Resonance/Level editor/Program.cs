﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using ResonanceLibrary;
using System.Xml;
using System.IO;


namespace Level_editor
{
    class Program
    {
        static void Main(string[] args)
        {
           /* Boolean ok = true;
            StoredObjects list = new StoredObjects();
            while (ok == true)
            {
                StoredObject obj = new StoredObject();
                Console.WriteLine("Please specify the identifier");
                obj.identifier = Console.ReadLine();
                Console.WriteLine("Please specify the type");
                obj.type = Console.ReadLine();
                Console.WriteLine("Please specify XWorldCoord");
                obj.xWorldCoord = float.Parse(Console.ReadLine());
                Console.WriteLine("Please specify YWorldCoord");
                obj.yWorldCoord = float.Parse(Console.ReadLine());
                Console.WriteLine("Please specify ZWorldCoord");
                obj.zWorldCoord = float.Parse(Console.ReadLine());
                Console.WriteLine("Please specify game model number");
                obj.gameModelNum = int.Parse(Console.ReadLine());
                
                list.addObject(obj);
                Console.WriteLine("Do you wish to continue? y/n");
                String response = Console.ReadLine();
                if(response.Equals("n"))
                    ok = false;
            }
            Serialize(list, "Level1.xml");
            */
            
        }
        static void Serialize(StoredObjects obj, string filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(filename, settings))
            {
                IntermediateSerializer.Serialize<StoredObjects>(writer, obj, null);
            }
        }
        static void Deserialize(string filename)
        {
            StoredObjects data = null;
            using (FileStream stream = new FileStream(filename, FileMode.Open))
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    data = IntermediateSerializer.Deserialize<StoredObjects>(reader, null);
                }
            }
            Console.WriteLine(data.list[0].identifier);
        }
    }
}

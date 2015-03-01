﻿// Copyright (c) 2015 Davide Iuffrida
// License: Academic Free License ("AFL") v. 3.0
// AFL License page: http://opensource.org/licenses/AFL-3.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SteinsGate_Text_Extractor
{
    class Program
    {
        public struct Line
        {
            public UInt16 Magic;
            public string[] param;
        }

        static Line[] Source = new Line[0];
        static List<int> index = new List<int>();

        static void Main(string[] args)
        {
            Console.WriteLine(
                @"
                      
                      ###################################
                      #     NSB Dialogues Extractor     #
                      ###################################
                      #         Made by Daviex          #
                      ###################################
                      #   Italian Steins;Gate VN Team   #
                      ###################################
                      #           Version 1.5           #
                      ###################################
                      #            Codename:            #
                      ###################################
                      #         El Psy Congroo          #
                      ###################################
                      
                           Press any key to start...    
                                                         ");
            Console.ReadLine();
            Console.WriteLine("After analyzed your NSB Files, I will start extract your dialogs!!");
            Console.WriteLine();


            if (Directory.Exists("nss"))
            {
                foreach (string file in Directory.EnumerateFiles("nss"))
                {
                    if (file.Contains(".nsb")) // && file.Contains("sg") && !file.Contains("function"))
                    {
                        BinaryReader br = new BinaryReader(File.OpenRead(file));

                        Analyzer(br);

                        ExtractText(file);
                    }
                }

                Console.WriteLine();
                Console.WriteLine("Tutturu, I finished!");
            }
            else
            {
                Console.WriteLine("There's no 'nss' folder! Put nss folder in directory with the executable!");
                Console.WriteLine("Press any key to close...");
                Console.ReadLine();
            }
        }

        static void Analyzer(BinaryReader nsbFile)
        {
            uint Entry, Length;
            ushort numParam;
            Line currLine;
            int point = 0;
            byte[] buffer;
            string Text = String.Empty;

            while (point < nsbFile.BaseStream.Length)
            {
                Entry = nsbFile.ReadUInt32();
                Entry -= 1;
                Array.Resize(ref Source, Source.Length + 1);
                currLine = Source[Entry];
                currLine.Magic = nsbFile.ReadUInt16();
                numParam = nsbFile.ReadUInt16();
                currLine.param = new string[numParam];

                for (int i = 0; i < numParam; i++)
                {
                    Length = nsbFile.ReadUInt32();
                    buffer = nsbFile.ReadBytes((int)Length);
                    Text = Encoding.Unicode.GetString(buffer);
                    if (Text.Contains("<PRE") && !Text.Contains("<PRE>.</PRE>"))
                        index.Add((int)Entry);

                    currLine.param[i] = Text;
                }

                point = (int)nsbFile.BaseStream.Position;

                Source[Entry] = currLine;
            }
        }
        
        static void ExtractText(string path)
        {   
            string fileName = path.Substring(path.LastIndexOf('\\') + 1);

            Console.Write("[+] " + fileName + " -> ");

            fileName = fileName.Replace("nsb", "txt");

            Console.Write(fileName + " [+]\n");

            if(!Directory.Exists("extracted\\nss"))
                Directory.CreateDirectory("extracted\\nss");

            path = "extracted\\nss\\" + fileName;
            StreamWriter sw = new StreamWriter(path);

            int countText = index.Count;

            for(int i = 0 ; i < countText; i++)
            {
                sw.Write(Source[index[i]].param[2] + '\n' + '\n');
            }

            Array.Clear(Source, 0, Source.Length);
            index.Clear();

            sw.Flush();
            sw.Close();
        }
    }
}

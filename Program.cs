using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using aviationLib;

namespace starDpLoader
{
    class Program
    {
        static Char[] recordType_001_01 = new Char[01];
        static Char[] facilityTypeCode_011_02 = new Char[02];
        static Char[] identifier_039_13 = new Char[13];
        static Char[] transition_052_110 = new Char[110];
        static Char[] ident_031_06 = new Char[06];

        static Phonetic phonetic = new Phonetic();

        static StreamWriter ofileSTARDP = new StreamWriter("starDpf.txt");

        static List<String> points = new List<String>();
        static List<String> airports = new List<String>();

        static Int32 c = 0;

        static void Main(String[] args)
        {
            String userprofileFolder = Environment.GetEnvironmentVariable("USERPROFILE");
            
            String[] fileEntries = Directory.GetFiles(userprofileFolder + "\\Downloads\\", "28DaySubscription*.zip");

            ZipArchive archive = ZipFile.OpenRead(fileEntries[0]);
            
            ZipArchiveEntry entry = archive.GetEntry("STARDP.txt");
            
            entry.ExtractToFile("STARDP.txt", true);

            StreamReader file = new StreamReader("STARDP.txt");

            String rec = file.ReadLine();

            identifier_039_13 = rec.ToCharArray(38, 13);

            while (!file.EndOfStream)
            {
                ProcessRecord(rec);
                
                rec = file.ReadLine();
            }

            ProcessRecord(rec);

            if (points.Count > 0)
            {
                for (Int32 p = 0; p < (points.Count - 1); p++)
                {
                    ofileSTARDP.Write(points[p]);
                    ofileSTARDP.Write(' ');
                }
                
                ofileSTARDP.Write(points[points.Count - 1]);
                
                points.Clear();
            }

            ofileSTARDP.Write('~');

            if (airports.Count > 0)
            {
                for (Int32 p = 0; p < (airports.Count - 1); p++)
                {
                    ofileSTARDP.Write(airports[p]);
                    ofileSTARDP.Write(' ');
                }
                
                ofileSTARDP.Write(airports[airports.Count - 1]);
                
                airports.Clear();
            }

            ofileSTARDP.Write(ofileSTARDP.NewLine);

            file.Close();

            ofileSTARDP.Close();
        }

        static void ProcessRecord(String record)
        {
            identifier_039_13 = record.ToCharArray(38, 13);
            
            if (identifier_039_13[0] != ' ')
            {
                if (c == 0)
                {
                    c = 1;
                }
                else
                {
                    if (points.Count > 0)
                    {
                        for (Int32 p = 0; p < (points.Count - 1); p++)
                        {
                            ofileSTARDP.Write(points[p]);
                            ofileSTARDP.Write(' ');
                        }
                        
                        ofileSTARDP.Write(points[points.Count - 1]);
                        
                        points.Clear();
                    }

                    ofileSTARDP.Write('~');

                    if (airports.Count > 0)
                    {
                        for (Int32 p = 0; p < (airports.Count - 1); p++)
                        {
                            ofileSTARDP.Write(airports[p]);
                            ofileSTARDP.Write(' ');
                        }
                        
                        ofileSTARDP.Write(airports[airports.Count - 1]);
                        
                        airports.Clear();
                    }

                    ofileSTARDP.Write(ofileSTARDP.NewLine);
                }

                recordType_001_01 = record.ToCharArray(0, 1);
                
                String s = new String(recordType_001_01).Trim();
                
                ofileSTARDP.Write(s);
                ofileSTARDP.Write('~');

                s = new String(identifier_039_13).Trim();
                
                String[] ss = s.Split('.');
                
                if (String.Compare(s, "NOT ASSIGNED") == 0)
                {
                    ofileSTARDP.Write("NOT ASSIGNED~NOT ASSIGNED~");
                }
                else
                {
                    if (recordType_001_01[0] == 'S')
                    {
                        ofileSTARDP.Write(ss[1]);
                        ofileSTARDP.Write('~');

                        ofileSTARDP.Write(ss[0]);
                        ofileSTARDP.Write('~');
                    }
                    else
                    {
                        ofileSTARDP.Write(ss[0]);
                        ofileSTARDP.Write('~');

                        ofileSTARDP.Write(ss[1]);
                        ofileSTARDP.Write('~');
                    }
                }

                transition_052_110 = record.ToCharArray(51, 110);
                
                s = new String(transition_052_110).Trim();
                
                ofileSTARDP.Write(s);
                ofileSTARDP.Write('~');

                s = new String(identifier_039_13).Trim();
                
                ofileSTARDP.Write(s);
                ofileSTARDP.Write('~');
            }

            ident_031_06 = record.ToCharArray(30, 6);
            
            String wp = new String(ident_031_06).Trim();

            facilityTypeCode_011_02 = record.ToCharArray(10, 2);
            
            String ft = new String(facilityTypeCode_011_02).Trim();
            
            switch(ft)
            {
                case "AA":
                {
                    airports.Add(wp);
                    
                    break;
                }
                
                case "NA":
                {
                    points.Add(wp);
                    
                    break;
                }
                
                case "P":
                case "R":
                {
                    points.Add(wp);
                    
                    break;
                }
                
                //ND VOR/DME
                case "ND":
                {
                    points.Add(wp);
                    
                    break;
                }
                
                //NO DME
                case "NO":
                {
                    points.Add(wp);
                    
                    break;
                }
                
                //NT TACAN
                case "NT":
                {
                    points.Add(wp);
                    
                    break;
                }
                
                //NV VOR
                case "NV":
                {
                    points.Add(wp);
                    
                    break;
                }
                
                //NW VORTAC
                case "NW":
                {
                    points.Add(wp);
                    
                    break;
                }
                
                //NX NDB
                case "NX":
                {
                    points.Add(wp);
                    
                    break;
                }
                
                default:
                {
                    points.Add(wp + ";UNK");
                    
                    break;
                }

            }
        }

    }
}

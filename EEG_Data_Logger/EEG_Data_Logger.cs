using System;
using System.Collections.Generic;
using Emotiv;
using System.IO;
using System.Threading;
using System.Reflection;

namespace EEG_Data_Logger
{
    class EEG_Logger
    {
        EmoEngine engine;   // Access to the EDK is via the EmoEngine 
        int userID = -1;    // userID is used to uniquely identify a user's headset

        EEG_Data_Analyzer af3_analyzer;
        EEG_Data_Analyzer af4_analyzer;
        EEG_Data_Analyzer f7_analyzer;
        EEG_Data_Analyzer f8_analyzer;
        EEG_Data_Analyzer f3_analyzer;
        EEG_Data_Analyzer f4_analyzer;
        EEG_Data_Analyzer fc5_analyzer;
        EEG_Data_Analyzer fc6_analyzer;
        EEG_Data_Analyzer t7_analyzer;
        EEG_Data_Analyzer t8_analyzer;
        EEG_Data_Analyzer p7_analyzer;
        EEG_Data_Analyzer p8_analyzer;
        EEG_Data_Analyzer o1_analyzer;
        EEG_Data_Analyzer o2_analyzer;

        EEG_Logger()
        {
            // create the engine
            engine = EmoEngine.Instance;
            engine.UserAdded += new EmoEngine.UserAddedEventHandler(engine_UserAdded_Event);

            // connect to Emoengine.            
            engine.Connect();

            // create a header for our output file
            //WriteHeader();

            // create a analyzer for o1 channel
            af3_analyzer = new EEG_Data_Analyzer();

            // create a analyzer for o2 channel
            af4_analyzer = new EEG_Data_Analyzer();

            // create a analyzer for o1 channel
            f7_analyzer = new EEG_Data_Analyzer();

            // create a analyzer for o2 channel
            f8_analyzer = new EEG_Data_Analyzer();

            // create a analyzer for o1 channel
            f3_analyzer = new EEG_Data_Analyzer();

            // create a analyzer for o2 channel
            f4_analyzer = new EEG_Data_Analyzer();

            // create a analyzer for o1 channel
            fc5_analyzer = new EEG_Data_Analyzer();

            // create a analyzer for o2 channel
            fc6_analyzer = new EEG_Data_Analyzer();

            // create a analyzer for o1 channel
            t7_analyzer = new EEG_Data_Analyzer();

            // create a analyzer for o2 channel
            t8_analyzer = new EEG_Data_Analyzer();

            // create a analyzer for o1 channel
            p7_analyzer = new EEG_Data_Analyzer();

            // create a analyzer for o2 channel
            p8_analyzer = new EEG_Data_Analyzer();

            // create a analyzer for o1 channel
            o1_analyzer = new EEG_Data_Analyzer();

            // create a analyzer for o2 channel
            o2_analyzer = new EEG_Data_Analyzer();
        }

        void engine_UserAdded_Event(object sender, EmoEngineEventArgs e)
        {
            Console.WriteLine("User Added Event has occured");

            // record the user 
            userID = (int)e.userId;

            // enable data aquisition for this user.
            engine.DataAcquisitionEnable((uint)userID, true);

            // ask for up to 1 second of buffered data
            engine.DataSetBufferSizeInSec(1);

        }
        void Run()
        {
            // Handle any waiting events
            engine.ProcessEvents();

            // If the user has not yet connected, do not proceed
            if ((int)userID == -1)
                return;

            Dictionary<EdkDll.IEE_DataChannel_t, double[]> data = engine.GetData((uint)userID);

            if (data == null)
            {
                return;
            }

            int _bufferSize = data[EdkDll.IEE_DataChannel_t.IED_TIMESTAMP].Length;

            //Console.WriteLine("Writing " + _bufferSize.ToString() + " sample of data ");
            //Console.WriteLine(" ");

            // Write the data to a file
            //TextWriter file = new StreamWriter(filename,true);

            for (int i = 0; i < _bufferSize; i++)
            {
                // now analyze the data
                foreach (EdkDll.IEE_DataChannel_t channel in data.Keys)
                {
                    if (Convert.ToString(channel) == "IED_AF3")
                    {
                        af3_analyzer.AddData(data[channel][i]);

                    }
                    else if (Convert.ToString(channel) == "IED_AF4")
                    {
                        af4_analyzer.AddData(data[channel][i]);
                    }
                    else if (Convert.ToString(channel) == "IED_F7")
                    {
                        f7_analyzer.AddData(data[channel][i]);
                    }
                    else if (Convert.ToString(channel) == "IED_F8")
                    {
                        f8_analyzer.AddData(data[channel][i]);
                    }
                    else if (Convert.ToString(channel) == "IED_F3")
                    {
                        f3_analyzer.AddData(data[channel][i]);
                    }
                    else if (Convert.ToString(channel) == "IED_F4")
                    {
                        f4_analyzer.AddData(data[channel][i]);
                    }
                    else if (Convert.ToString(channel) == "IED_FC5")
                    {
                        fc5_analyzer.AddData(data[channel][i]);
                    }
                    else if (Convert.ToString(channel) == "IED_FC6")
                    {
                        fc6_analyzer.AddData(data[channel][i]);
                    }
                    else if (Convert.ToString(channel) == "IED_T7")
                    {
                        t7_analyzer.AddData(data[channel][i]);
                    }
                    else if (Convert.ToString(channel) == "IED_T8")
                    {
                        t8_analyzer.AddData(data[channel][i]);
                    }
                    else if (Convert.ToString(channel) == "IED_P7")
                    {
                        p7_analyzer.AddData(data[channel][i]);
                    }
                    else if (Convert.ToString(channel) == "IED_P8")
                    {
                        p8_analyzer.AddData(data[channel][i]);
                    }
                    else if (Convert.ToString(channel) == "IED_O1")
                    {
                        o1_analyzer.AddData(data[channel][i]);
                    }
                    else if (Convert.ToString(channel) == "IED_O2")
                    {
                        o2_analyzer.AddData(data[channel][i]);
                    }

                }

                // now write the data
                //foreach (EdkDll.IEE_DataChannel_t channel in data.Keys)
                //    file.Write(data[channel][i] + ",");
                //file.WriteLine("");

            }
            //file.Close();

        }

        public void Start()
        {
            f7_analyzer.ClearData();
            f7_analyzer.ClearSplit();
            f8_analyzer.ClearData();
            f8_analyzer.ClearSplit();
        }

        public string Analysis(string mode)
        {
            double[] erp_af3 = af3_analyzer.Analysis(mode);
            double[] erp_af4 = af4_analyzer.Analysis(mode);
            double[] erp_f7 = f7_analyzer.Analysis(mode);
            double[] erp_f8 = f8_analyzer.Analysis(mode);
            double[] erp_f3 = f3_analyzer.Analysis(mode);
            double[] erp_f4 = f4_analyzer.Analysis(mode);
            double[] erp_fc5 = fc5_analyzer.Analysis(mode);
            double[] erp_fc6 = fc6_analyzer.Analysis(mode);
            double[] erp_t7 = t7_analyzer.Analysis(mode);
            double[] erp_t8 = t8_analyzer.Analysis(mode);
            double[] erp_p7 = p7_analyzer.Analysis(mode);
            double[] erp_p8 = p8_analyzer.Analysis(mode);
            double[] erp_o1 = o1_analyzer.Analysis(mode);
            double[] erp_o2 = o2_analyzer.Analysis(mode);


            // Calculate average of erp
            //double erp_A = (erp_o1[0] + erp_o2[0]) / 2.0;
            //double erp_B = (erp_o1[1] + erp_o2[1]) / 2.0;
            double erp_af3_A = erp_af3[0];
            double erp_af3_B = erp_af3[1];
            double erp_af4_A = erp_af4[0];
            double erp_af4_B = erp_af4[1];
            double erp_f7_A = erp_f7[0];
            double erp_f7_B = erp_f7[1];
            double erp_f7_C = erp_f7[2];
            double erp_f7_D = erp_f7[3];
            double erp_f7_E = erp_f7[4];
            double erp_f7_F = erp_f7[5];
            double erp_f7_G = erp_f7[6];
            double erp_f8_A = erp_f8[0];
            double erp_f8_B = erp_f8[1];
            double erp_f8_C = erp_f8[2];
            double erp_f8_D = erp_f8[3];
            double erp_f8_E = erp_f8[4];
            double erp_f8_F = erp_f8[5];
            double erp_f8_G = erp_f8[6];
            double erp_f3_A = erp_f3[0];
            double erp_f3_B = erp_f3[1];
            double erp_f4_A = erp_f4[0];
            double erp_f4_B = erp_f4[1];
            double erp_fc5_A = erp_fc5[0];
            double erp_fc5_B = erp_fc5[1];
            double erp_fc6_A = erp_fc6[0];
            double erp_fc6_B = erp_fc6[1];
            double erp_t7_A = erp_t7[0];
            double erp_t7_B = erp_t7[1];
            double erp_t8_A = erp_t8[0];
            double erp_t8_B = erp_t8[1];
            double erp_p7_A = erp_p7[0];
            double erp_p7_B = erp_p7[1];
            double erp_p8_A = erp_p8[0];
            double erp_p8_B = erp_p8[1];
            double erp_o1_A = erp_o1[0];
            double erp_o1_B = erp_o1[1];
            double erp_o2_A = erp_o2[0];
            double erp_o2_B = erp_o2[1];
            //double erp_A = erp_f7[0];
            //double erp_B = erp_f7[1];

            // Compare value
            double[] rank = { 0, 0, 0, 0, 0, 0, 0 };
            int k=0;
            string selected_item = "0";
            if (mode.Equals("T"))
            {
                if (erp_f8[0] == 4)
                    selected_item = "H";
                else if (erp_f7[0] == (double)2)
                    selected_item = "B";
                else if (erp_f7[0] == (double)3)
                    selected_item = "C";
                else if (erp_f7[0] == (double)4)
                    selected_item = "D";
                else if (erp_f8[0] == (double)1)
                    selected_item = "E";
                else if (erp_f8[0] == (double)2)
                    selected_item = "F";
                else if (erp_f8[0] == (double)3)
                selected_item = "G";
                else if (erp_f7[0]==(double)1)
                selected_item = "A";
                return selected_item;
            }
            //n100 f7
            else if (mode.Equals("B")||mode.Equals("D"))
            {
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        if (rank[i] <= Math.Abs(erp_f7[j]))
                        {
                            rank[i] = Math.Abs(erp_f7[j]);
                            k = j;
                        }
                    }
                    erp_f7[k] = -1;
                }
                if (rank[0] == Math.Abs(erp_f7_A))
                    selected_item = "3";
                else if (rank[0] == Math.Abs(erp_f7_B))
                    selected_item = "4";
                else if (rank[0] == Math.Abs(erp_f7_C))
                    selected_item = "5";
                else if (rank[0] == Math.Abs(erp_f7_D))
                    selected_item = "6";
                else if (rank[0] == Math.Abs(erp_f7_E))
                    selected_item = "7";
                else if (rank[0] == Math.Abs(erp_f7_F))
                    selected_item = "8";
                else if (rank[0] == Math.Abs(erp_f7_G))
                    selected_item = "9";
                Console.WriteLine("1: " + Math.Abs(erp_f7_A) + " 2: " + Math.Abs(erp_f7_B) + " 3: " + Math.Abs(erp_f7_C) + " 4: " + Math.Abs(erp_f7_D) + " 5: " + Math.Abs(erp_f7_E) + " 6: " + Math.Abs(erp_f7_F) + " 7: " + Math.Abs(erp_f7_G));
                Console.Write("\n");
                if (selected_item == "3")
                    Console.WriteLine(" *첫번째 음식 선호* \n");
                else if (selected_item == "4")
                    Console.WriteLine(" *두번째 음식 선호* \n");
                else if (selected_item == "5")
                    Console.WriteLine(" *세번째 음식 선호* \n");
                else if (selected_item == "6")
                    Console.WriteLine(" *네번째 음식 선호* \n");
                else if (selected_item == "7")
                    Console.WriteLine(" *다섯번째 음식 선호* \n");
                else if (selected_item == "8")
                    Console.WriteLine(" *여섯번째 음식 선호* \n");
                else if (selected_item == "9")
                    Console.WriteLine(" *일곱번째 음식 선호* \n");
            }
            //p300 f8
            else if (mode.Equals("E")||mode.Equals("G"))
            {
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        if (rank[i] <= erp_f8[j])
                        {
                            rank[i] = erp_f8[j];
                            k = j;
                        }
                    }
                    erp_f7[k] = -1;
                }
                if (rank[0] == erp_f8_A)
                    selected_item = "3";
                else if (rank[0] == erp_f8_B)
                    selected_item = "4";
                else if (rank[0] == erp_f8_C)
                    selected_item = "5";
                else if (rank[0] == erp_f8_D)
                    selected_item = "6";
                else if (rank[0] == erp_f8_E)
                    selected_item = "7";
                else if (rank[0] == erp_f8_F)
                    selected_item = "8";
                else if (rank[0] == erp_f8_G)
                    selected_item = "9";
                Console.WriteLine("1: " + erp_f8_A + " 2: " + erp_f8_B + " 3: " + erp_f8_C + " 4: " + erp_f8_D + " 5: " + erp_f8_E + " 6: " + erp_f8_F + " 7: " + erp_f8_G);
                Console.Write("\n");
                if (selected_item == "3")
                    Console.WriteLine(" *첫번째 음식 선호* \n");
                else if (selected_item == "4")
                    Console.WriteLine(" *두번째 음식 선호* \n");
                else if (selected_item == "5")
                    Console.WriteLine(" *세번째 음식 선호* \n");
                else if (selected_item == "6")
                    Console.WriteLine(" *네번째 음식 선호* \n");
                else if (selected_item == "7")
                    Console.WriteLine(" *다섯번째 음식 선호* \n");
                else if (selected_item == "8")
                    Console.WriteLine(" *여섯번째 음식 선호* \n");
                else if (selected_item == "9")
                    Console.WriteLine(" *일곱번째 음식 선호* \n");
            }
            //n100 f8
            else if (mode.Equals("F")||mode.Equals("H"))
            {
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        if (rank[i] <= Math.Abs(erp_f8[j]))
                        {
                            rank[i] = Math.Abs(erp_f8[j]);
                            k = j;
                        }
                    }
                    erp_f7[k] = -1;
                }
                if (rank[0] == Math.Abs(erp_f8_A))
                    selected_item = "3";
                else if (rank[0] == Math.Abs(erp_f8_B))
                    selected_item = "4";
                else if (rank[0] == Math.Abs(erp_f8_C))
                    selected_item = "5";
                else if (rank[0] == Math.Abs(erp_f8_D))
                    selected_item = "6";
                else if (rank[0] == Math.Abs(erp_f8_E))
                    selected_item = "7";
                else if (rank[0] == Math.Abs(erp_f8_F))
                    selected_item = "8";
                else if (rank[0] == Math.Abs(erp_f8_G))
                    selected_item = "9";
                Console.WriteLine("1: " + Math.Abs(erp_f8_A) + " 2: " + Math.Abs(erp_f8_B) + " 3: " + Math.Abs(erp_f8_C) + " 4: " + Math.Abs(erp_f8_D) + " 5: " + Math.Abs(erp_f8_E) + " 6: " + Math.Abs(erp_f8_F) + " 7: " + Math.Abs(erp_f8_G));
                Console.Write("\n");
                if (selected_item == "3")
                    Console.WriteLine(" *첫번째 음식 선호* \n");
                else if (selected_item == "4")
                    Console.WriteLine(" *두번째 음식 선호* \n");
                else if (selected_item == "5")
                    Console.WriteLine(" *세번째 음식 선호* \n");
                else if (selected_item == "6")
                    Console.WriteLine(" *네번째 음식 선호* \n");
                else if (selected_item == "7")
                    Console.WriteLine(" *다섯번째 음식 선호* \n");
                else if (selected_item == "8")
                    Console.WriteLine(" *여섯번째 음식 선호* \n");
                else if (selected_item == "9")
                    Console.WriteLine(" *일곱번째 음식 선호* \n");
            }
            //p300 f7
            else if(mode.Equals("A") || mode.Equals("C"))
            {
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        if (rank[i] <= erp_f7[j])
                        {
                            rank[i] = erp_f7[j];
                            k = j;
                        }
                    }
                    erp_f7[k] = -1;
                }
                if (rank[0] == erp_f7_A)
                    selected_item = "3";
                else if (rank[0] == erp_f7_B)
                    selected_item = "4";
                else if (rank[0] == erp_f7_C)
                    selected_item = "5";
                else if (rank[0] == erp_f7_D)
                    selected_item = "6";
                else if (rank[0] == erp_f7_E)
                    selected_item = "7";
                else if (rank[0] == erp_f7_F)
                    selected_item = "8";
                else if (rank[0] == erp_f7_G)
                    selected_item = "9";
                Console.WriteLine("1: " + erp_f7_A + " 2: " + erp_f7_B + " 3: " + erp_f7_C + " 4: " + erp_f7_D + " 5: " + erp_f7_E + " 6: " + erp_f7_F + " 7: " + erp_f7_G);
                Console.Write("\n");
                if (selected_item == "3")
                    Console.WriteLine(" *첫번째 음식 선호* \n");
                else if (selected_item == "4")
                    Console.WriteLine(" *두번째 음식 선호* \n");
                else if (selected_item == "5")
                    Console.WriteLine(" *세번째 음식 선호* \n");
                else if (selected_item == "6")
                    Console.WriteLine(" *네번째 음식 선호* \n");
                else if (selected_item == "7")
                    Console.WriteLine(" *다섯번째 음식 선호* \n");
                else if (selected_item == "8")
                    Console.WriteLine(" *여섯번째 음식 선호* \n");
                else if (selected_item == "9")
                    Console.WriteLine(" *일곱번째 음식 선호* \n");
            }


            //Console.WriteLine("-----다른 지점들-----\n");
            //Console.WriteLine(" 첫번째 음식)");
            //Console.WriteLine("af3: " + erp_af3_A + ", af4: " + erp_af4_A + ", f8: " + erp_f8_A + ", f3: " + erp_f3_A + ", f4: " + erp_f4_A + ", fc5: " + erp_fc5_A + ", fc6: " + erp_fc6_A + ", t7: " + erp_af3_A + ", t8: " + erp_t8_A + ", p7: " + erp_p7_A + ", p8: " + erp_p8_A + ", o1: " + erp_o1_A + ", o2: " + erp_o2_A + "\n");
            //Console.WriteLine(" 두번째 음식)");
            //Console.WriteLine("af3: " + erp_af3_B + ", af4: " + erp_af4_B + ", f8: " + erp_f8_B + ", f3: " + erp_f3_B + ", f4: " + erp_f4_B + ", fc5: " + erp_fc5_B + ", fc6: " + erp_fc6_B + ", t7: " + erp_af3_B + ", t8: " + erp_t8_B + ", p7: " + erp_p7_B + ", p8: " + erp_p8_B + ", o1: " + erp_o1_B + ", o2: " + erp_o2_B + "\n");

            return selected_item;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("EEG Data Reader Example");

            EEG_Logger p = new EEG_Logger();
            EEG_Data_Server server = new EEG_Data_Server(p);
            server.Open();

            while (true)
            {
                if (Console.KeyAvailable)
                    break;
                //Example for set marker to data stream and set sychronization signal
                //if (i % 37 == 0)
                //{
                //    p.engine.DataSetMarker((uint)p.userID, i);
                //    p.engine.DataSetSychronizationSignal((uint)p.userID, i);

                //}
                p.Run();
                Thread.Sleep(10);
            }

            server.Close();
        }
    }
}

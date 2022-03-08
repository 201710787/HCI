using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections;


namespace EEG_Data_Logger
{
    class EEG_Data_Analyzer
    {
        // 몇번째 반복인지 알아야함
        // EEG 데이터를 저장해야함
        // Reference 데이터를 저장해야함
        ArrayList dataList;
        ArrayList refList;
        ArrayList eegList;
        ArrayList refList1; 
        ArrayList refList2; 
        double[] totalData_A = new double[128];
        double[] totalData_B = new double[128];
        double[] totalData_C = new double[128];
        double[] totalData_D = new double[128];
        double[] totalData_E = new double[128];
        double[] totalData_F = new double[128];
        double[] totalData_G = new double[128];
        double[] totalData_A1 = new double[128];
        double[] totalData_A2 = new double[128];
        double[] totalData_A3 = new double[128];
        double[] totalData_A4 = new double[128];
        double[] totalData_B1 = new double[128];
        double[] totalData_B2 = new double[128];
        double[] totalData_B3 = new double[128];
        double[] totalData_B4 = new double[128];
        int ref_Cnt = 0;
        int trash_Cnt = 0;
        int eeg_Cnt = 0;
        int sampling_rate = 128; // Hz
        int count = 21;


        public EEG_Data_Analyzer()
        {
            refList = new ArrayList();
            eegList = new ArrayList();
            dataList = new ArrayList();
            refList1 = new ArrayList();
            refList2 = new ArrayList();
            for (int i = 0; i < 128; i++)
            {
                totalData_A[i] = 0;
                totalData_B[i] = 0;
                totalData_C[i] = 0;
                totalData_D[i] = 0;
                totalData_E[i] = 0;
                totalData_F[i] = 0;
                totalData_G[i] = 0;
                totalData_A1[i] = 0;
                totalData_A2[i] = 0;
                totalData_A3[i] = 0;
                totalData_A4[i] = 0;
                totalData_B1[i] = 0;
                totalData_B2[i] = 0;
                totalData_B3[i] = 0;
                totalData_B4[i] = 0;
            }
        }
        ///////////
        public void AddData(double data)
        {
            dataList.Add(data);
        }

        public double[] Analysis(string mode)
        {
            try
            {
                if (mode.Equals("T"))
                {
                    for (int j = 0; j < 6; j++)
                    {
                        for (int i = 0; i < 128; i++)
                        {
                            if (i > 21 && i < 28)
                            {
                                refList2.Add(dataList[i + j * (128)]);
                                refList1.Add(dataList[i + j * (128)]);
                            }
                            else if (i < 28)
                            {
                                refList2.Add(dataList[i + j * (128)]);
                            }
                            else if (i > 21 && i < 64) // 0.7s~ 1s reference
                            {
                                refList1.Add(dataList[i + j * (128)]);
                            }
                            
                            // refList[i] = data;
                            else if (i >= 64)
                            {
                                eegList.Add(dataList[i + j * (128)]);
                                eeg_Cnt += 1;
                            }
                        }

                        double[] bandRefData1 = (Double[])refList1.ToArray(typeof(Double));
                        double[] bandRefData2 = (Double[])refList2.ToArray(typeof(Double));
                        double[] bandData = (Double[])eegList.ToArray(typeof(Double));

                        // 3. Calculate average of reference
                        double reference1 = CalculateReference(bandRefData1);
                        double reference2 = CalculateReference(bandRefData2);

                        // 4. Normalize data based on reference
                        double[] normData1 = Normalization(bandData, reference1);
                        double[] normData2 = Normalization(bandData, reference2);
                        for (int i = 0; i < 64; i++)
                        {
                            if (i % 2 == 0)
                            {
                                totalData_A1[i] += normData1[i];
                                totalData_A3[i] += normData2[i];
                            }
                            else
                            {
                                totalData_B1[i] += normData1[i];
                                totalData_B3[i] += normData2[i];
                            }
                        }
                        for(int i = 0; i <42; i++)
                        {
                            if (i % 2 == 0)
                                totalData_A2[i] += bandRefData1[i];
                            else
                                totalData_B2[i] += bandRefData1[i];
                        }
                        for (int i = 0; i <28; i++)
                        {
                            if (i % 2 == 0)
                                totalData_A4[i] += bandRefData2[i];
                            else
                                totalData_B4[i] += bandRefData2[i];
                        }
                        refList1.Clear();
                        refList2.Clear();
                        eegList.Clear();
                    }

                }
                else
                    //데이터 저장
                    //double[] data = ReadData("data.txt");
                    for (int j = 0; j < count; j++)
                    {
                        // 1. Split data into reference and eeg
                        // normdata p300
                        if (mode.Equals("A") || mode.Equals("E"))
                        {
                            for (int i = 0; i < 128; i++)
                            {
                                if (i > 21 && i < 64) // 0.7s~ 1s reference
                                {
                                    refList.Add(dataList[i + j * (128)]);
                                    ref_Cnt += 1;
                                }
                                // refList[i] = data;
                                else if (i >= 64)
                                {
                                    eegList.Add(dataList[i + j * (128)]);
                                    eeg_Cnt += 1;
                                }

                                else
                                    trash_Cnt += 1;
                                continue;
                            }
                            // 2. Bandpass filter (5~30Hz)
                            //double[] bandRefData = BandPass((Double[])refList.ToArray(typeof(Double)), sampling_rate, 5, 30, 10000000);
                            //double[] bandData = BandPass((Double[])eegList.ToArray(typeof(Double)), sampling_rate, 5, 30, 10000000);
                            double[] bandRefData = (Double[])refList.ToArray(typeof(Double));
                            double[] bandData = (Double[])eegList.ToArray(typeof(Double));

                            // 3. Calculate average of reference
                            double reference = CalculateReference(bandRefData);

                            // 4. Normalize data based on reference
                            double[] normData = Normalization(bandData, reference);

                            // 5. Add data

                            for (int i = 0; i < 63; i++)
                            {
                                //A
                                if (j % 7 == 0)
                                    totalData_A[i] += normData[i];
                                //B
                                else if (j % 7 == 1)
                                    totalData_B[i] += normData[i];
                                else if (j % 7 == 2)
                                    totalData_C[i] += normData[i];
                                else if (j % 7 == 3)
                                    totalData_D[i] += normData[i];
                                else if (j % 7 == 4)
                                    totalData_E[i] += normData[i];
                                else if (j % 7 == 5)
                                    totalData_F[i] += normData[i];
                                else if (j % 7 == 6)
                                    totalData_G[i] += normData[i];
                            }

                            // 6. Clear lists
                            ClearSplit();
                        }
                        //normdata n100
                        else if (mode.Equals("B") || mode.Equals("F"))
                        {
                            for (int i = 0; i < 128; i++)
                            {
                                if (i < 28) // 0.7s~ 1s reference
                                {
                                    refList.Add(dataList[i + j * (128)]);
                                    ref_Cnt += 1;
                                }
                                // refList[i] = data;
                                else if (i >= 64)
                                {
                                    eegList.Add(dataList[i + j * (128)]);
                                    eeg_Cnt += 1;
                                }

                                else
                                    trash_Cnt += 1;
                                continue;
                            }
                            // 2. Bandpass filter (5~30Hz)
                            //double[] bandRefData = BandPass((Double[])refList.ToArray(typeof(Double)), sampling_rate, 5, 30, 10000000);
                            //double[] bandData = BandPass((Double[])eegList.ToArray(typeof(Double)), sampling_rate, 5, 30, 10000000);
                            double[] bandRefData = (Double[])refList.ToArray(typeof(Double));
                            double[] bandData = (Double[])eegList.ToArray(typeof(Double));

                            // 3. Calculate average of reference
                            double reference = CalculateReference(bandRefData);

                            // 4. Normalize data based on reference
                            double[] normData = Normalization(bandData, reference);

                            // 5. Add data

                            for (int i = 0; i < 63; i++)
                            {
                                //A
                                if (j % 7 == 0)
                                    totalData_A[i] += normData[i];
                                //B
                                else if (j % 7 == 1)
                                    totalData_B[i] += normData[i];
                                else if (j % 7 == 2)
                                    totalData_C[i] += normData[i];
                                else if (j % 7 == 3)
                                    totalData_D[i] += normData[i];
                                else if (j % 7 == 4)
                                    totalData_E[i] += normData[i];
                                else if (j % 7 == 5)
                                    totalData_F[i] += normData[i];
                                else if (j % 7 == 6)
                                    totalData_G[i] += normData[i];
                            }

                            // 6. Clear lists
                            ClearSplit();
                        }
                        //refdata p300
                        else if (mode.Equals("C") || mode.Equals("G"))
                        {
                            for (int i = 0; i < 128; i++)
                            {
                                if (i > 21 && i < 64) // 0.7s~ 1s reference
                                {
                                    refList.Add(dataList[i + j * (128)]);
                                    ref_Cnt += 1;
                                }
                                // refList[i] = data;
                                else if (i >= 64)
                                {
                                    eegList.Add(dataList[i + j * (128)]);
                                    eeg_Cnt += 1;
                                }

                                else
                                    trash_Cnt += 1;
                                continue;
                            }
                            // 2. Bandpass filter (5~30Hz)
                            //double[] bandRefData = BandPass((Double[])refList.ToArray(typeof(Double)), sampling_rate, 5, 30, 10000000);
                            //double[] bandData = BandPass((Double[])eegList.ToArray(typeof(Double)), sampling_rate, 5, 30, 10000000);
                            double[] bandRefData = (Double[])refList.ToArray(typeof(Double));
                            double[] bandData = (Double[])eegList.ToArray(typeof(Double));

                            // 3. Calculate average of reference
                            double reference = CalculateReference(bandRefData);

                            // 4. Normalize data based on reference
                            double[] normData = Normalization(bandData, reference);

                            // 5. Add data

                            for (int i = 0; i < 42; i++)
                            {
                                //A
                                if (j % 7 == 1)
                                    totalData_A[i] += bandRefData[i];
                                //B
                                else if (j % 7 == 2)
                                    totalData_B[i] += bandRefData[i];
                                else if (j % 7 == 3)
                                    totalData_C[i] += bandRefData[i];
                                else if (j % 7 == 4)
                                    totalData_D[i] += bandRefData[i];
                                else if (j % 7 == 5)
                                    totalData_E[i] += bandRefData[i];
                                else if (j % 7 == 6)
                                    totalData_F[i] += bandRefData[i];
                                else if (j % 7 == 0)
                                    totalData_G[i] += bandRefData[i];
                            }

                            // 6. Clear lists
                            ClearSplit();
                        }

                        else if (mode.Equals("D") || mode.Equals("H"))
                        {
                            for (int i = 0; i < 128; i++)
                            {
                                if (i < 28) // 0.7s~ 1s reference
                                {
                                    refList.Add(dataList[i + j * (128)]);
                                    ref_Cnt += 1;
                                }
                                // refList[i] = data;
                                else if (i >= 64)
                                {
                                    eegList.Add(dataList[i + j * (128)]);
                                    eeg_Cnt += 1;
                                }

                                else
                                    trash_Cnt += 1;
                                continue;
                            }
                            // 2. Bandpass filter (5~30Hz)
                            //double[] bandRefData = BandPass((Double[])refList.ToArray(typeof(Double)), sampling_rate, 5, 30, 10000000);
                            //double[] bandData = BandPass((Double[])eegList.ToArray(typeof(Double)), sampling_rate, 5, 30, 10000000);
                            double[] bandRefData = (Double[])refList.ToArray(typeof(Double));
                            double[] bandData = (Double[])eegList.ToArray(typeof(Double));

                            // 3. Calculate average of reference
                            double reference = CalculateReference(bandRefData);

                            // 4. Normalize data based on reference
                            double[] normData = Normalization(bandData, reference);

                            // 5. Add data

                            for (int i = 0; i < 28; i++)
                            {
                                //A
                                if (j % 7 == 0)
                                    totalData_A[i] += bandRefData[i];
                                //B
                                else if (j % 7 == 1)
                                    totalData_B[i] += bandRefData[i];
                                else if (j % 7 == 2)
                                    totalData_C[i] += bandRefData[i];
                                else if (j % 7 == 3)
                                    totalData_D[i] += bandRefData[i];
                                else if (j % 7 == 4)
                                    totalData_E[i] += bandRefData[i];
                                else if (j % 7 == 5)
                                    totalData_F[i] += bandRefData[i];
                                else if (j % 7 == 6)
                                    totalData_G[i] += bandRefData[i];
                            }

                            // 6. Clear lists
                            ClearSplit();
                        }
                    }

                //데이터 분석
                // 1. Average data
                if (mode.Equals("T"))
                {
                    for (int i = 0; i < 64; i++)
                    {
                        totalData_A1[i] /= 3;
                        totalData_A2[i] /= 3;
                        totalData_B1[i] /= 3;
                        totalData_B2[i] /= 3;
                    }
                    for (int i = 0; i < 42; i++)
                    {
                        totalData_A3[i] /= 3;
                        totalData_B3[i] /= 3;
                    }
                    for (int i = 0; i < 28; i++)
                    {
                        totalData_A4[i] /= 3;
                        totalData_B4[i] /= 3;
                    }
                }
                else if (mode.Equals("C") || mode.Equals("G"))
                {
                    for (int i = 0; i < 42; i++)
                    {
                        totalData_A[i] /= 3;
                        totalData_B[i] /= 3;
                        totalData_C[i] /= 3;
                        totalData_D[i] /= 3;
                        totalData_E[i] /= 3;
                        totalData_F[i] /= 3;
                        totalData_G[i] /= 3;
                    }
                }
                else if (mode.Equals("D") || mode.Equals("H"))
                {
                    for (int i = 0; i < 28; i++)
                    {
                        totalData_A[i] /= 3;
                        totalData_B[i] /= 3;
                        totalData_C[i] /= 3;
                        totalData_D[i] /= 3;
                        totalData_E[i] /= 3;
                        totalData_F[i] /= 3;
                        totalData_G[i] /= 3;
                    }
                }
                else
                    for (int i = 0; i < 63; i++)
                    {
                        totalData_A[i] /= 3;
                        totalData_B[i] /= 3;
                        totalData_C[i] /= 3;
                        totalData_D[i] /= 3;
                        totalData_E[i] /= 3;
                        totalData_F[i] /= 3;
                        totalData_G[i] /= 3;
                    }


                // 2. Detect ERP

                double[] test = new double[7];
                if (DetectERP(totalData_A1) >= DetectERP(totalData_B1))
                    test[0] = 1;
                else if (DetectERP(totalData_A2) >= DetectERP(totalData_B2))
                    test[0] = 2;
                else if (DetectERP(totalData_A3) >= DetectERP(totalData_B4))
                    test[0] = 3;
                else if (DetectERP(totalData_A4) >= DetectERP(totalData_B4))
                    test[0] = 4;

                double erp_A = DetectERP(totalData_A);
                double erp_B = DetectERP(totalData_B);
                double erp_C = DetectERP(totalData_C);
                double erp_D = DetectERP(totalData_D);
                double erp_E = DetectERP(totalData_E);
                double erp_F = DetectERP(totalData_F);
                double erp_G = DetectERP(totalData_G);

                double[] erp = { erp_A, erp_B, erp_C, erp_D, erp_E, erp_F, erp_G };


                // 3. Clear data
                ClearData();
                double[] result;
                if (mode.Equals("T"))
                {
                    result = test;
                }
                else
                {
                    result = erp;
                }
                return result;

            }
            catch (Exception e)
            {
                double[] erp_except = { 0, 0, 0, 0, 0, 0, 0 };
                return erp_except;
            }
        }

        public void ClearData()
        {
            dataList.Clear();
        }

        public void ClearSplit()
        {
            refList.Clear();
            eegList.Clear();
        }

        [DllImport("coclib", CallingConvention = CallingConvention.Cdecl)]
        extern public static IntPtr band_pass_filter(double[] data, int length, int sampling_rate, double low_cut, double high_cut, int mag_alpha);

        public static double[] BandPass(double[] data, int sampling_rate, double low_cut, double high_cut, int mag_alpha)
        {
            IntPtr bandPointer = band_pass_filter(data, data.Length, sampling_rate, low_cut, high_cut, mag_alpha);
            int[] bandData = new int[data.Length];
            double[] outData = new double[data.Length];
            Marshal.Copy(bandPointer, bandData, 0, data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                outData[i] = bandData[i] / (double)mag_alpha;
            }
            return outData;
        }

        public static double CalculateReference(double[] reference)
        {
            double sum = 0;
            for (int i = 0; i < reference.Length; i++)
            {
                sum += reference[i];
            }
            double avg = sum / reference.Length;
            return avg;
        }

        private static double[] Normalization(double[] data, double reference)
        {
            double[] normData = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                normData[i] = data[i] - reference;
            }
            return normData;
        }

        private static double DetectERP(double[] data)
        {
            double max = 0;
            for (int i = 0; i < data.Length; i++)
            {
                double power = Math.Abs(data[i]);
                if (power >= max)
                {
                    max = power;
                }
            }
            return max;
        }
    }
}
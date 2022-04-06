/*
Amber D. 2019  C#

Tool tracking app for Zoller measurement PC
This class is triggered by listener in main 
Loops through file generated at end of measurement
captures data and returns to be saved to DB
*/
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Plt8_ToolTracking
{
    class FileReader
    {
        public object[] read()
        { 
           //Variables
            object[] arrayData = new object[30];
            string date = "";
            string time = "";
            

            StreamReader sr = new StreamReader("C:/Zoller Log Files/Log.txt");//Instantiate reader with path

            try
            {
                // Create an instance of StreamReader to read from a file
                using (sr)
                {
                    string line;

                    // Read lines from the file until the end of the file is reached. 
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] words = line.Split();//Split each line into words
                        {
                            foreach (string word in words)//Analyze each word
                            {
                                if (words.Contains("Identity"))//Get toolID
                                {
                                    string zollerID = "";
                                    for (int i=2;i<words.Length;i++)
                                    {                                     
                                        zollerID += words[i] + " ";//Get rest of line as zoller ID
                                    }

                                    arrayData[0] = zollerID;//Save zoller ID to array
                                }                              
                                if (word.Contains("/"))//Get Date 
                                {
                                    date = word + " ";
                                }
                                if (word.Contains(":"))//Get Time
                                {
                                    time = words[4] + words[5];
                                    date += time;//Combine Date and Time
                                    arrayData[1] = date;
                                }                              
                                if (word.Contains("Lengthways"))//Get Lengthways
                                {
                                    try
                                    {
                                        arrayData[2] = decimal.Parse(words[words.Length - 2]);
                                    }
                                    catch (FormatException ex)
                                    {
                                        MessageBox.Show("Could not save Length");
                                    }
                                }
                                if (word.Contains("Crossways"))//Get Crossways
                                {
                                    try
                                    {
                                        arrayData[3] = decimal.Parse(words[words.Length - 2]);
                                    }
                                    catch (FormatException ex)
                                    {
                                        MessageBox.Show("Could not save Crossways");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                // Let the user know what went wrong.
                MessageBox.Show("FILE NOT FOUND ERROR!");
                sr.Close();//Close File
            }

            sr.Close();//Close File
            return arrayData;//Return filled array
        }
    }
}

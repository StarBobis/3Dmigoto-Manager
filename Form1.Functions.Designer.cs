using IniParser.Model;
using IniParser;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;

namespace _3DmigotoManager
{
    partial class Form1
    {
        

        


        private void initializeGameConfig()
        {
            checkBoxCleanFaAuto.Checked = true;
            checkBoxCleanFaAuto.Checked = false;

            textBoxCleanFaNumber.Text = "3";
            textBoxGameStarterPath.Text = string.Empty;
            textBoxMainProgramPath.Text = string.Empty;


        }

        private void readGameConfig(string GameName)
        {
            string jsonContent = File.ReadAllText(gameSettingsPath);
            JObject jsonObject = JObject.Parse(jsonContent);
            if (string.IsNullOrEmpty(current_game))
            {
                MessageBox.Show("Please select game first");
                return;
            }
            if (!jsonObject.ContainsKey(GameName))
            {
                //MessageBox.Show("Please select game first");
                return;
            }
            JObject gameObject = (JObject)jsonObject[GameName];

            bool CleanFrameAnalysisFolderAuto = (bool)gameObject["CleanFrameAnalysisFolderAuto"];
            int CleanFrameAnalysisFolderNumber = (int)gameObject["CleanFrameAnalysisFolderNumber"];
            string GameMainProgramPath = (string)gameObject["GameMainProgramPath"];
            string GameStarterPath = (string)gameObject["GameStarterPath"];

            checkBoxCleanFaAuto.Checked = CleanFrameAnalysisFolderAuto;
            textBoxCleanFaNumber.Text = CleanFrameAnalysisFolderNumber.ToString();
            textBoxMainProgramPath.Text = GameMainProgramPath;
            textBoxGameStarterPath.Text = GameStarterPath;
        }

        private void saveGameConfig(bool showSuccessBox = false)
        {
            //检查是否初始化了3DmigotoFolders
            if (string.IsNullOrEmpty(loaders_folder_path))
            {
                MessageBox.Show("Please initialize your 3DmigotoLoaders first!");
                return;
            }

            string jsonContent = File.ReadAllText(gameSettingsPath);
            JObject jsonObject = JObject.Parse(jsonContent);
            if (string.IsNullOrEmpty(current_game))
            {
                MessageBox.Show("Please select game first");
                return;
            }

            JObject gameObject = new JObject();
            gameObject["CleanFrameAnalysisFolderAuto"] = checkBoxCleanFaAuto.Checked;
            int cleanNumber = 0;
            if (!int.TryParse(textBoxCleanFaNumber.Text,out cleanNumber))
            {
                MessageBox.Show("The FrameAnalysisFolder reserve number must be a number.");
                return;
            }

            gameObject["CleanFrameAnalysisFolderNumber"] = int.Parse(textBoxCleanFaNumber.Text);
            gameObject["GameMainProgramPath"] = textBoxMainProgramPath.Text;
            gameObject["GameStarterPath"] = textBoxGameStarterPath.Text;

            jsonObject[current_game] = gameObject;

            File.WriteAllText(gameSettingsPath, jsonObject.ToString());

            //Then we save LoadersFolder and GameName
            string jsonContentMain = File.ReadAllText(mainSettingPath);
            JObject jsonObjectMain = JObject.Parse(jsonContentMain);
            jsonObjectMain["GameName"] = current_game;
            File.WriteAllText(mainSettingPath, jsonObjectMain.ToString());


            //Then we need save to d3dx.ini.
            string gameD3dxPath = current_3Dmigoto_folder + "\\d3dx.ini";


            //data["Loader"]["target"] = textBoxMainProgramPath.Text; 
            //data["Loader"]["module"] = "d3d11.dll"; 

            Dictionary<string, string> replaceKVPairDict = new Dictionary<string, string>();
            replaceKVPairDict.Add("target", textBoxMainProgramPath.Text);
            replaceKVPairDict.Add("module", "d3d11.dll");
            replaceKVPairDict.Add("require_admin", "true");

            update3DmigotoConfig(gameD3dxPath,"Loader", replaceKVPairDict);

            if (showSuccessBox)
            {
                MessageBox.Show("Save success!");

            }


        }
        private void openLatestFrameAnalysisFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool result = checkIfInitialized();
            if (result)
            {
                try
                {
                    string[] directories = Directory.GetDirectories(current_3Dmigoto_folder);
                    List<string> frameAnalysisFileList = new List<string>();
                    foreach (string directory in directories)
                    {
                        string directoryName = Path.GetFileName(directory);

                        if (directoryName.StartsWith("FrameAnalysis-"))
                        {
                            frameAnalysisFileList.Add(directoryName);
                        }
                    }

                    //
                    if (frameAnalysisFileList.Count > 0)
                    {
                        frameAnalysisFileList.Sort();

                        string latestFrameAnalysisFolder = current_3Dmigoto_folder + "\\" + frameAnalysisFileList.Last();
                        OpenFolder(latestFrameAnalysisFolder);
                        //MessageBox.Show(latestFrameAnalysisFolderName);
                    }
                    else
                    {
                        MessageBox.Show("Target directory didn't have any FrameAnalysisFolder.");
                    }


                }
                catch (IOException ex)
                {
                    MessageBox.Show("An IO exception has occurred: " + ex.Message);
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show("You do not have permission to access one or more folders: " + ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An unexpected exception has occurred: " + ex.Message);
                }
            }
        }

        private void deleteFrameAnalysisFolders(bool deleteAll)
        {
            saveGameConfig();
            if (deleteAll)
            {
                
                string[] directories = Directory.GetDirectories(current_3Dmigoto_folder);
                foreach (string directory in directories)
                {
                    string directoryName = Path.GetFileName(directory);

                    if (directoryName.StartsWith("FrameAnalysis-"))
                    {
                        string latestFrameAnalysisFolder = current_3Dmigoto_folder + "\\" + directoryName;
                        FileSystem.DeleteDirectory(latestFrameAnalysisFolder, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                        //Directory.Delete(latestFrameAnalysisFolder,true);
                    }
                }

            }
            else
            {
                string[] directories = Directory.GetDirectories(current_3Dmigoto_folder);
                List<string> frameAnalysisFileList = new List<string>();
                foreach (string directory in directories)
                {
                    string directoryName = Path.GetFileName(directory);

                    if (directoryName.StartsWith("FrameAnalysis-"))
                    {
                        frameAnalysisFileList.Add(directoryName);
                    }
                }

                //Get FA numbers to reserve
                int cleanNumber = 0;
                if (!int.TryParse(textBoxCleanFaNumber.Text, out cleanNumber))
                {
                    MessageBox.Show("The FrameAnalysisFolder reserve number must be a number.");
                    return;
                }

                frameAnalysisFileList.Sort();

                int n = int.Parse(textBoxCleanFaNumber.Text); // 你想移除的元素数量
                if (n > 0 && frameAnalysisFileList.Count > n)
                {
                    frameAnalysisFileList.RemoveRange(frameAnalysisFileList.Count - n, n);

                }
                else if (frameAnalysisFileList.Count <= n)
                {
                    // 如果 n 大于等于列表的长度，就清空整个列表
                    frameAnalysisFileList.Clear();
                }
                if (frameAnalysisFileList.Count > 0)
                {
                    foreach (string directoryName in frameAnalysisFileList)
                    {
                        string latestFrameAnalysisFolder = current_3Dmigoto_folder + "\\" + directoryName;
                        FileSystem.DeleteDirectory(latestFrameAnalysisFolder, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                        //Directory.Delete(latestFrameAnalysisFolder, true);
                    }
                }


            }

            //MessageBox.Show("Clean FrameAnalysisFolder successful!");


        }


        public void autoClean()
        {
            if (checkBoxCleanFaAuto.Checked)
            {
                bool result = checkIfInitialized();
                if (result)
                {
                    deleteFrameAnalysisFolders(false);
                }
            }
            
        }


    }
}

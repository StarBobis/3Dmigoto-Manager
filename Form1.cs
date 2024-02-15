using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3DmigotoManager
{
    public partial class Form1 : Form
    {
        string startupPath;
        string migotoTemplatePath;
        string migotoD3dxiniPath;
        List<string> supportedGameList = new List<string> {
            "GIMI_DEV","GIMI_PLAY",
            "SRMI_DEV","SRMI_PLAY",
            "HI3_DEV","HI3_PLAY",
            "Unknown_DEV","Unknown_PLAY",
            "ZZZ_DEV","ZZZ_PLAY"
        };
        //string releaseDate = "2023-12-25";
        string mainSettingPath;
        string gameSettingsPath;
        string current_game = "";

        string loaders_folder_path = "";
        string current_3Dmigoto_folder = "";



        public Form1()
        {
            InitializeComponent();
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            //Start up path
            this.startupPath = Application.StartupPath;
            migotoTemplatePath = startupPath + "\\3Dmigoto";
            migotoD3dxiniPath = startupPath + "\\game_configs";
            mainSettingPath = startupPath + "\\manager_settings\\MainSetting.json";
            gameSettingsPath = startupPath + "\\manager_settings\\GameSettings.json";

            //Can't located at chinese character location for compatible reason.
            //MessageBox.Show(this.startupPath);
            if (ContainsChinese(this.startupPath))
            {
                MessageBox.Show("Sorry, you can't put this program in a path contains Chinese characters, please switch to another path and try again.");
                Application.Exit();
            }

            //TODO Check time to make sure they always use the afdian version.
            //if (IsCurrentDateMoreThan30DaysFromGivenDate(releaseDate)) {
            //    MessageBox.Show("Your application is expired, Please get the latest one from NicoMico.");
            //    Application.Exit();
            //}


            initializeGameConfig();

            string jsonContent = File.ReadAllText(mainSettingPath);
            JObject jsonObject = JObject.Parse(jsonContent);

            string loadersFolderPath = (string)jsonObject["LoadersFolder"];
            this.loaders_folder_path = loadersFolderPath;

            string gameName = (string)jsonObject["GameName"];
            if (!string.IsNullOrEmpty(gameName))
            {
                setGameName(gameName);
            }

        }

        private void setGameName(string gameName)
        {
            current_game = gameName;
            this.Text = "NicoMico's 3Dmigoto Manager CurrentGame: " + current_game;
            initializeGameConfig();
            readGameConfig(gameName);

            if (!string.IsNullOrEmpty(loaders_folder_path))
            {
                current_3Dmigoto_folder = loaders_folder_path + "\\" + gameName;
                //MessageBox.Show(current_3Dmigoto_folder);
            }
        }




        private void initializeConfigsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            initializeGameConfig();
        }

        private void saveConfigsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveGameConfig(true);
            
        }


   


        private bool checkIfInitialized()
        {
            if (string.IsNullOrEmpty(current_3Dmigoto_folder))
            {
                MessageBox.Show("Please initialize your 3DmigotoLoaders first!");
                return false;
            }
            else
            {
                return true;
            }
        }

        public void OpenFolder(string folderPath)
        {
            Process.Start("explorer.exe", folderPath);
        }

        private void buttonOpen3DmigotoFolder_Click(object sender, EventArgs e)
        {
            bool result = checkIfInitialized();
            if (result)
            {
                OpenFolder(current_3Dmigoto_folder);

            }
        }

 

        private void cleanFrameAnalysisFoldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool result = checkIfInitialized();
            if (result)
            {
                deleteFrameAnalysisFolders(false);
            }
        }

        private void deleteAllFrameAnalysisFoldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool result = checkIfInitialized();
            if (result)
            {
                deleteFrameAnalysisFolders(true);
            }
        }

        private void buttonChooseMainProgram_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Executable files (*.exe)|*.exe";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Title = "Browse for Executable File";

            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;

                textBoxMainProgramPath.Text = filePath;
            }
        }


        private void buttonChooseStarterPath_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Executable files (*.exe)|*.exe";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Title = "Browse for Executable File";

            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;

                textBoxGameStarterPath.Text = filePath;
            }
        }


        private void buttonOpenMainProgramPath_Click(object sender, EventArgs e)
        {
            string mainProgramPath = textBoxMainProgramPath.Text;
            if (string.IsNullOrEmpty(mainProgramPath))
            {
                MessageBox.Show("Please select your game's main program's location first.");
                return;
            }
            
            string directoryPath = Path.GetDirectoryName(mainProgramPath);
            OpenFolder(directoryPath);
            
        }


        private void start3Dmigoto()
        {
            string migotoLoaderPath = current_3Dmigoto_folder + "\\3Dmigoto Loader.exe";
            //MessageBox.Show(migotoLoaderPath);
            ProcessStartInfo startInfo = new ProcessStartInfo(migotoLoaderPath);
            string workingDirectory = Path.GetDirectoryName(migotoLoaderPath);
            startInfo.WorkingDirectory = workingDirectory;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = true;
            Process.Start(startInfo);
        }

        private void start3DmigotoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            start3Dmigoto();

        }

        private void startGameStarter()
        {
            string gameStarterPath = textBoxGameStarterPath.Text;

            if (string.IsNullOrEmpty(gameStarterPath))
            {
                MessageBox.Show("Please set your game's starter path before run.");
                return;
            }

            if (!File.Exists(gameStarterPath))
            {
                MessageBox.Show("Your game's starter path didn't exists.");
                return;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo(gameStarterPath);
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = true;
            Process.Start(startInfo);
        }


        private void startGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startGameStarter();
        }


        private void automaticStartToolStripMenuItem_Click(object sender, EventArgs e)
        {

            start3Dmigoto();
            
            startGameStarter();


        }


        private void initialize3DmigotoLoadersToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            autoClean();
        }

        private void initialize3DmigotoLoadersToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

            //Initialize 3Dmigoto Loaders
            string migotoFolderPath = "";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                migotoFolderPath = folderBrowserDialog1.SelectedPath;

                if (ContainsChinese(migotoFolderPath))
                {
                    MessageBox.Show("Sorry, you can't put 3DmigotoLoaders in a path contains Chinese characters, please switch to another path and try again.");
                    return;
                }

                string jsonContent = File.ReadAllText(mainSettingPath);
                JObject jsonObject = JObject.Parse(jsonContent);
                string LoadersFolder = (string)jsonObject["LoadersFolder"];
                if (!string.IsNullOrEmpty(LoadersFolder) && LoadersFolder == migotoFolderPath)
                {
                    var result = MessageBox.Show(
                        "Your selected folder is already your LoadersFolder location,do you want to delete all content and initialize all?",
                        "Warning",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result != DialogResult.Yes)
                    {
                        return;
                    }
                    else
                    {
                        FileSystem.DeleteDirectory(migotoFolderPath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                        //Directory.Delete(migotoFolderPath, true);
                    }
                }

                Directory.CreateDirectory(migotoFolderPath);
                foreach (string gameName in supportedGameList)
                {
                    //Here we copy entire directory
                    string gameTargetFolder = migotoFolderPath + "\\" + gameName + "\\";

                    string gameConfigSourcePath = migotoD3dxiniPath + "\\" + gameName ;


                    Directory.CreateDirectory(gameTargetFolder);

                    CopyDirectory(migotoTemplatePath, gameTargetFolder);
                    CopyDirectory(gameConfigSourcePath, gameTargetFolder);

                }
                jsonObject["LoadersFolder"] = migotoFolderPath;

                loaders_folder_path = migotoFolderPath;
                File.WriteAllText(mainSettingPath, jsonObject.ToString());

                MessageBox.Show("3Dmigoto Loaders initialize success!");

            }

        }


        private void select3DmigotoLoadersFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Initialize 3Dmigoto Loaders
            string migotoFolderPath = "";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                migotoFolderPath = folderBrowserDialog1.SelectedPath;

                if (ContainsChinese(migotoFolderPath))
                {
                    MessageBox.Show("Sorry, you can't put 3DmigotoLoaders in a path contains Chinese characters, please switch to another path and try again.");
                    return;
                }

                string jsonContent = File.ReadAllText(mainSettingPath);
                JObject jsonObject = JObject.Parse(jsonContent);
                jsonObject["LoadersFolder"] = migotoFolderPath;

                loaders_folder_path = migotoFolderPath;
                File.WriteAllText(mainSettingPath, jsonObject.ToString());

                //这里还需要更新设置一下GameName，不然已有的GameName的current_3Dmigoto_loaders这个变量会失效
                setGameName(current_game);

                MessageBox.Show("3Dmigoto Loaders select success!");
            }
        }

        private void update3DmigotoFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string jsonContent = File.ReadAllText(mainSettingPath);
            JObject jsonObject = JObject.Parse(jsonContent);
            string LoadersFolder = (string)jsonObject["LoadersFolder"];

            Directory.CreateDirectory(LoadersFolder);
            foreach (string gameName in supportedGameList)
            {
                //Here we copy entire directory
                string gameTargetFolder = LoadersFolder + "\\" + gameName + "\\";

                string gameConfigSourcePath = migotoD3dxiniPath + "\\" + gameName;


                Directory.CreateDirectory(gameTargetFolder);

                CopyDirectory(migotoTemplatePath, gameTargetFolder);
                CopyDirectory(gameConfigSourcePath, gameTargetFolder);

                //对每个游戏，都要更新d3dx.ini
                string gameD3dxPath = LoadersFolder + "\\" + gameName + "\\d3dx.ini";
                if (File.Exists(gameD3dxPath))
                {

                    string jsonContent2 = File.ReadAllText(gameSettingsPath);
                    JObject jsonObject2 = JObject.Parse(jsonContent2);

                    if (!jsonObject2.ContainsKey(gameName))
                    {
                        //MessageBox.Show("Please select game first");
                        continue;
                    }
                    JObject gameObject = (JObject)jsonObject2[gameName];

                    string GameMainProgramPath = (string)gameObject["GameMainProgramPath"];

                    //data["Loader"]["target"] = textBoxMainProgramPath.Text; 
                    //data["Loader"]["module"] = "d3d11.dll"; 

                    Dictionary<string, string> replaceKVPairDict = new Dictionary<string, string>();
                    replaceKVPairDict.Add("target", GameMainProgramPath);
                    replaceKVPairDict.Add("module", "d3d11.dll");
                    replaceKVPairDict.Add("require_admin", "true");

                    update3DmigotoConfig(gameD3dxPath, "Loader", replaceKVPairDict);
                }
                

            }

            MessageBox.Show("Update Success!");
        }


        private void buttonOpenModFolder_Click(object sender, EventArgs e)
        {
            bool result = checkIfInitialized();
            if (result)
            {
                string latestFrameAnalysisFolder = current_3Dmigoto_folder + "\\Mods" ;
                OpenFolder(latestFrameAnalysisFolder);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool result = checkIfInitialized();
            if (result)
            {
                string latestFrameAnalysisFolder = current_3Dmigoto_folder + "\\ShaderFixes";
                OpenFolder(latestFrameAnalysisFolder);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string mainProgramPath = textBoxGameStarterPath.Text;
            if (string.IsNullOrEmpty(mainProgramPath))
            {
                MessageBox.Show("Please select your game's starter's location first.");
                return;
            }

            string directoryPath = Path.GetDirectoryName(mainProgramPath);
            OpenFolder(directoryPath);
        }


        private void buttonBlender3DmigootLight_Click(object sender, EventArgs e)
        {
            openLink("https://github.com/StarBobis/Blender3DmigotoLight");
        }

        private void buttonNPP_HLSL_Syntax_Click(object sender, EventArgs e)
        {
            openLink("https://github.com/StarBobis/NPP_HLSL_Syntax");
        }

        private void button3DmigotoSword_Click(object sender, EventArgs e)
        {
            openLink("https://github.com/StarBobis/Sword");
        }

        private void button3DmigotoTailor_Click(object sender, EventArgs e)
        {
            openLink("https://github.com/StarBobis/Tailor");

        }

        private void buttonTangentFix_Click(object sender, EventArgs e)
        {
            openLink("https://github.com/StarBobis/TangentFix");

        }


        private void buttonMaterialCombiner_Click(object sender, EventArgs e)
        {
            openLink("https://github.com/Grim-es/material-combiner-addon");
        }



        //open website link
        private void openLink(string link)
        {
            Process.Start(link);
        }

        private void buttonBlender_Click(object sender, EventArgs e)
        {
            openLink("https://www.blender.org/");
        }

        private void buttonGIMI_Click(object sender, EventArgs e)
        {
            openLink("https://github.com/SilentNightSound/GI-Model-Importer");
        }

        private void buttonSRMI_Click(object sender, EventArgs e)
        {
            openLink("https://github.com/SilentNightSound/SR-Model-Importer");
        }

        private void button3DFixes_Click(object sender, EventArgs e)
        {
            openLink("https://github.com/DarkStarSword/3d-fixes");
        }

        private void button3Dmigoto_Click(object sender, EventArgs e)
        {
            openLink("https://github.com/bo3b/3Dmigoto");
        }

        private void button3DmigotoArmor_Click(object sender, EventArgs e)
        {
            openLink("https://github.com/StarBobis/3Dmigoto-Armor-23-12-24-fork");
        }

        private void buttonPhotoshop_Click(object sender, EventArgs e)
        {
            openLink("https://www.adobe.com/products/photoshop.html");
        }



        private void buttonNVDIATextureTool_Click(object sender, EventArgs e)
        {
            openLink("https://developer.nvidia.com/nvidia-texture-tools-exporter");
        }

        private void buttonPaintDotNet_Click(object sender, EventArgs e)
        {
            openLink("https://github.com/paintdotnet/release");
        }



        private void button3DmigotoManager_Click(object sender, EventArgs e)
        {
            openLink("https://github.com/StarBobis/3DmigotoManager/releases");

        }

        private void buttonLeoTools_Click(object sender, EventArgs e)
        {
            openLink("https://github.com/leotorrez/LeoTools");
        }

        private void buttonIniDocumentation_Click(object sender, EventArgs e)
        {
            openLink("https://github.com/MurrenMods/IniDocumentation");
        }


        private void buttonAutoReload_Click(object sender, EventArgs e)
        {
            openLink("https://github.com/samytichadou/Auto_Reload_Blender_addon");
        }




        private void gIMIDEVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setGameName("GIMI_DEV");
        }

        private void gIMIPLAYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setGameName("GIMI_PLAY");

        }

        private void sRMIDEVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setGameName("SRMI_DEV");

        }

        private void sRMIPLAYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setGameName("SRMI_PLAY");
        }

        private void nBPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setGameName("Unknown_DEV");
        }

        private void nBPPLAYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setGameName("Unknown_PLAY");
        }


        private void hI3DEVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setGameName("HI3_DEV");
        }

        private void hI3PLAYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setGameName("HI3_PLAY");
        }


        private void zZZDEVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setGameName("ZZZ_DEV");
        }

        private void zZZPLAYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setGameName("ZZZ_PLAY");

        }





    }
}

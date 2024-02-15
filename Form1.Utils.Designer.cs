using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3DmigotoManager
{
    partial class Form1
    {

        public bool ContainsChinese(string input)
        {
            return Regex.IsMatch(input, @"[\u4e00-\u9fff]");
        }

        public void CopyDirectory(string sourceDirPath, string destDirPath)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirPath);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory not found: {sourceDirPath}");
            }

            // 如果目标目录不存在，则创建它
            if (!Directory.Exists(destDirPath))
            {
                Directory.CreateDirectory(destDirPath);
            }

            // 复制所有文件
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirPath, file.Name);
                file.CopyTo(temppath, true);
            }

            // 复制所有子目录
            DirectoryInfo[] dirs = dir.GetDirectories();
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirPath, subdir.Name);
                CopyDirectory(subdir.FullName, temppath);
            }
        }

        public bool IsCurrentDateMoreThan30DaysFromGivenDate(string dateString)
        {
            DateTime givenDate;

            // 尝试将给定的日期字符串转换为DateTime对象
            if (DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out givenDate))
            {
                // 计算当前日期和给定日期之间的差异
                TimeSpan difference = DateTime.Now - givenDate;

                // 判断差值天数是否大于30天
                return difference.TotalDays > 30;
            }
            else
            {
                throw new ArgumentException("Can't initialize date.");
            }
        }

        public List<string> updateIniLine(List<string> lines,string sectionName ,string inputKey,string inputValue)
        {
            List<string> newLines = new List<string>();

            bool isInsideLoaderSection = false;
            bool targetExists = false;

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];

                if (line.StartsWith("[" + sectionName + "]"))
                {
                    //MessageBox.Show(line);
                    isInsideLoaderSection = true;
                }
                else if (isInsideLoaderSection
                    && !line.StartsWith(";") && line.Contains(inputKey) && line.Contains("=")

                    )
                {
                    //MessageBox.Show(line);
                    targetExists = true;
                    break;
                }
                else if (isInsideLoaderSection && line.StartsWith("["))
                {
                    // 离开当前部分，停止查找
                    isInsideLoaderSection = false;
                    break;
                }
            }

            //overwrite to file
            isInsideLoaderSection = false;
            bool add_newLine = false;
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                string newline = line;
                if (line.StartsWith("[" + sectionName + "]"))
                {
                    isInsideLoaderSection = true;
                }
                else if (isInsideLoaderSection)
                {
                    //如果存在target = 且当前行为target = 时
                    if (targetExists &&  !line.StartsWith(";") && line.Contains(inputKey) && line.Contains("="))
                    {
                        newline = inputKey + " = " + inputValue;
                        newLines.Add(newline);
                        add_newLine = true;
                        continue;

                    }

                    if (!add_newLine && !targetExists)
                    {
                        //MessageBox.Show("Line: " + line);
                        //MessageBox.Show("Update: " + inputKey + " = " + inputValue);

                        //在添加之前必须得先把原本的行添加进去，不然就会漏一行
                        newLines.Add(line);
                        newline = inputKey + " = " + inputValue;
                        newLines.Add(newline);
                        add_newLine = true;
                        continue;
                    }

                }
                newLines.Add(newline);
            }
            return newLines;
        }


        public void update3DmigotoConfig(string filepath,string sectionName ,Dictionary<string, string> replaceKVPairDict)
        {
            //MessageBox.Show(filepath);
            string[] lines = File.ReadAllLines(filepath);

            List<string> newLines = new List<string>(lines);
            List<string> newLinesUpdated = new List<string>(newLines);
            // 遍历字典中的键值对
            foreach (KeyValuePair<string, string> kvp in replaceKVPairDict)
            {
                
                newLinesUpdated = updateIniLine(newLinesUpdated, sectionName,kvp.Key, kvp.Value);
                //MessageBox.Show("Lines after update:" + newLinesUpdated.Count);
            }

            File.WriteAllLines(filepath, newLinesUpdated);
            
        }


    }
}

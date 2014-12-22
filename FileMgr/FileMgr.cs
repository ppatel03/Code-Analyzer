///////////////////////////////////////////////////////////////////////
// FileMgr.cs - Filemanager retrieves file from the                  //
//              specified directory                                  //
// ver 1.0                                                           //
// Language:    C#, 2013, .Net Framework 4.5                         //
// Platform:    Lenovo Y40, Win8.1                                   //
// Application: Code Analyzer for CSE681, Project #2, Fall 2014      //
// Author:      Dhaval N Dholakiya, Syracuse University              //
//              (315) 447-7644, ddholaki@syr.edu                     //
///////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ------------------
 * This module defines the following class:
 *   FileMgr  - Responsible for sending files back to Executive.cs
 */
/* 
 *   
 * Maintenance History:
 * --------------------
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CodeAnalysis
{

    /// /////////////////////////////////
    // Retrieves the file and sends it to the Executive 
    public class FileMgr
    {
        private List<string> files = new List<string>();
        private List<string> patterns = new List<string>();
        private static bool recurse = false;

        //-------------<checks whether to find files in subdirectories>---------------
        public void subDirectoriesCheck(List<string> options)
        {
            if (options.Contains("/S"))
            {
                recurse = true;
            }
        }

        //------------------<finds the files from a given path>-----------------------
        public void findFiles(string path)
        {
            if (patterns.Count == 0)
                addPattern("*.*");
            foreach (string pattern in patterns)
            {
                string[] newFiles = Directory.GetFiles(path, pattern);
                for (int i = 0; i < newFiles.Length; ++i)
                    newFiles[i] = Path.GetFullPath(newFiles[i]);
                files.AddRange(newFiles);
            }
            if (recurse)
            {
                string[] dirs = Directory.GetDirectories(path);
                foreach (string dir in dirs)
                    findFiles(dir);
            }
        }

        //-----------------<adds the patterns>----------------------------
        public void addPattern(string pattern)
        {
            patterns.Add(pattern);
        }

        //--------------<returns the files to the executive>--------------
        public List<string> getFiles1()
        {
            return files;
        }

        //-----------<gets the files from specified path and pattern>-----------------
        public string[] getFiles(string path, List<string> patterns)
        {
            FileMgr fm = new FileMgr();
            foreach (string pattern in patterns)
                fm.addPattern(pattern);
            fm.findFiles(path);
            return fm.getFiles1().ToArray();
        }



        //------------------<Test Stub for File Manager>----------------------

#if(TEST_FILEMGR)
        static void Main(string[] args)
        {
            Console.Write("\n  Testing FileMgr Class");
            Console.Write("\n =======================\n");

            string path = "../../";
            List<string> patterns = new List<string>();
            List<string> options = new List<string>();
            options.Add("/S");
            patterns.Add("*.cs");

            FileMgr fm = new FileMgr();
            fm.addPattern("*.cs");
            fm.findFiles("../../");
            fm.subDirectoriesCheck(options);


            List<string> files = fm.getFiles1();
            string[] files1 = fm.getFiles(path, patterns);
            for (int i = 0; i < files1.Length; i++)
            {
                Console.WriteLine("\n{0}", files1[i]);
            }
            Console.Write("\n\n");


        }
#endif
    }
}

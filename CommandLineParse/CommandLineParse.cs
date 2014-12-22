///////////////////////////////////////////////////////////////////////
// CommandLineParse.cs - CommandLineParse validates and returns      //
//                       the input parameters                        //
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
 *   CommandLineParsing  - it validates and returns the input parameters
 */
/* 
 *   
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
using System.Text.RegularExpressions;

namespace CodeAnalysis
{
    /// ///////////////////////////////////////
    //Parses the command line and returns values to the Executive
    public class CommandLineParsing
    {
        //---------<checks for the path from the command line and sends it to the Executive>------------
        public string processArgumentsPath(string[] arg)
        {

            string path = null;


            for (int i = 0; i < arg.Length; i++)
            {
                if (Directory.Exists(arg[i]) || arg[i].Contains("../") || arg[i].Contains("./") || arg[i].Contains(":/"))
                {
                    path = arg[i];
                    break;
                }

            }
            return path;
        }

        //----------<checks for the patterns from the command line and sends it to the Executive>-----------
        public List<string> processArgumentsPattern(string[] arg)
        {

            List<string> patterns = new List<string>();

            for (int i = 0; i < arg.Length; i++)
            {

                string input = arg[i];
                Match match = Regex.Match(input, @"[A-Za-z0-9\*]+\.[A-Za-z\*]+$");
                if (match.Success)
                {
                    patterns.Add(arg[i]);
                }

            }
            return patterns;
        }

        //--------------<checks for the options from the command line and sends it to the Executive>-----------
        public List<string> processArgumentsOptions(string[] arg)
        {

            List<string> options = new List<string>();

            for (int i = 0; i < arg.Length; i++)
            {
                if (arg[i].Contains("/X") || arg[i].Contains("/R") || arg[i].Contains("/S") || arg[i].Contains("/x") || arg[i].Contains("/r") || arg[i].Contains("/s"))
                {
                    options.Add(arg[i]);
                }


            }
            return options;
        }

        //--------------<to display Command Line Args>----------------------------
        static void ShowCommandLine(string[] args)
        {
            Console.Write("\n  Commandline args are:\n");
            foreach (string arg in args)
            {
                Console.Write("  {0}", arg);
            }
            Console.Write("\n\n  current directory: {0}", System.IO.Directory.GetCurrentDirectory());
            Console.Write("\n\n");
        }

        //--------------------< Test Stub for CommandLineParse>-------------------------

#if(TEST_COMMANDLINEPARSING)

        static void Main(string[] args)
        {
            string path = null;
            List<string> patterns = new List<string>();
            List<string> options = new List<string>();

            string[] arg = { "../", "CommandLineParse.cs", "/S" };

            CommandLineParsing passArguments = new CommandLineParsing();
            CommandLineParsing.ShowCommandLine(arg);
            path = passArguments.processArgumentsPath(arg);
            patterns = passArguments.processArgumentsPattern(arg);
            options = passArguments.processArgumentsOptions(arg);

            Console.WriteLine("The path is {0}", path);

            foreach (string pattern in patterns)
            {
                Console.WriteLine("The Pattern Provided is {0}", pattern);
            }
            foreach (string option in options)
            {
                Console.WriteLine("The Option Provided is {0}", option);
            }
            Console.ReadLine();

        }
#endif

    }
}

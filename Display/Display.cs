///////////////////////////////////////////////////////////////////////
// Display.cs - Display is responsible to present the output         //
//              to the user via a command prompt                     //
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
 *   Display  - It provides output to the User in Command Prompt
 */
/* Required Files:
 *   Parser.cs
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

namespace CodeAnalysis
{

    /// /////////////////////////////////////
    // Used to Display the Types and Relationships on the Command Prompt
    public class Display
    {
        //----------------<gets files for test stub>------------------------
        private static string[] getFiles(string path, List<string> patterns)
        {
            List<string> files = new List<string>();
            foreach (string pattern in patterns)
            {
                string[] newFiles = Directory.GetFiles(path, pattern);
                for (int i = 0; i < newFiles.Length; ++i)
                    newFiles[i] = Path.GetFullPath(newFiles[i]);

                files.AddRange(newFiles);
            }
            return files.ToArray();
        }

        //--------------<Display the Summary Header>------------------------
        public void display(String[] files, string path, List<string> pattern, List<string> options)
        {
            Console.Write("\n====================================CODE ANALYZER==============================\n\n\n");
            Console.Write("\nPath provided by User     :");

            Console.Write(path);

            Console.Write("\n\nOptions provided by User  :");
            foreach (String o in options)
            {
                Console.Write(Convert.ToString(o));
            }
            Console.Write("\n\nPatterns provided by User :");
            foreach (String p in pattern)
            {
                Console.Write(" ");
                Console.Write(Convert.ToString(p));
            }
            Console.Write("\n\nFiles to be Analyzed :\n");

            foreach (String e in files)
            {
                Console.Write(Convert.ToString(e));
                Console.Write("\n");
            }
            Console.Write("\n =============================================================================  ");
            Console.Write(" =============================================================================  ");
            Console.Write("                               SUMMARY FILE SET");
            Console.Write("\n =============================================================================  ");
            Console.Write(" =============================================================================  \n\n");

        }

        //----------------<Display the no of functions, namespaces,enums etc>-------------------
        public void displayCounters(int cnamespace, int cclass, int cinterface, int cfunction, int cstruct, int cenum, int cdelegate, bool b)
        {
            if (b)
            {
                Console.Write("\n\n---------------------------------------------\n");
                Console.Write("File Summary\n");
                Console.Write("---------------------------------------------\n");
                Console.Write("\n\n\n Number of namespace: {0}", cnamespace);
                Console.Write("\n Number of class    : {0}", cclass);
                Console.Write("\n Number of interface: {0}", cinterface);
                Console.Write("\n Number of function : {0}", cfunction);
                Console.Write("\n Nmber of struct    : {0}", cstruct);
                Console.Write("\n Number of enum     : {0}", cenum);
                Console.Write("\n Number of delegate : {0}", cdelegate);
            }
        }

        //---------------------<Displays the end part of the summary>------------------------------------
        public void displaySummaryEnd(string[] files, int i)
        {
            for (int z = i + 1; z <= files.Length - 1; z++)
            {
                Console.Write("\n\n\n =============================================================================  ");
                Console.Write(" Processing File: \n {0}", files[z]);
                Console.Write("\n =============================================================================  \n");
                Console.Write("***No Data Found in this File*** ");

            }
            Console.WriteLine();
            Console.Write("\n\n\n\n");

        }

        //--------------------<Displays the Types Header>----------------------------------
        public void displayHeader(string[] files)
        {
            Console.Write("\n =============================================================================  ");
            Console.Write(" Processing File:\n {0}", files[0]);
            Console.Write("\n =============================================================================  \n");
            Console.Write("Entering  Leaving           Type                        Name    ");
            Console.Write("\nLine No.  Line No.                                                      ");
            Console.Write("\n-------------------------------------------------------------------------------");

        }

        //-----------------<Displays the Summary>-------------------------
        public void displaySummary(string[] files)
        {
            int cnamespace = 0;
            int cclass = 0;
            int cinterface = 0;
            int cfunction = 0;
            int cstruct = 0;
            int cenum = 0;
            int cdelegate = 0;
            List<Elem> outputList = OutputRepository.output_;
            displayHeader(files);
            int i = 0; bool b = false;
            foreach (Elem e in outputList)
            {
                if (e.type.Contains("namespace")) cnamespace++;
                if (e.type.Contains("class")) cclass++;
                if (e.type.Contains("interfcae")) cinterface++;
                if (e.type.Contains("function")) cfunction++;
                if (e.type.Contains("struct")) cstruct++;
                if (e.type.Contains("enum")) cenum++;
                if (e.type.Contains("delegte")) cdelegate++;
                if (!e.fileName.Equals(files[i]))
                {
                    i++;
                    displayCounters(cnamespace, cclass, cinterface, cfunction, cstruct, cenum, cdelegate, b);
                    cnamespace = 0; cclass = 0; cinterface = 0; cfunction = 0; cstruct = 0; cenum = 0; cdelegate = 0;
                    Console.Write("\n\n\n =============================================================================  ");
                    Console.Write(" Processing File: \n {0}", files[i]);
                    Console.Write("\n =============================================================================  \n");
                    Console.Write("Entering  Leaving           Type                        Name    ");
                    Console.Write("\nLine No.  Line No.                                                      ");
                    Console.Write("\n-------------------------------------------------------------------------------");
                }
                if (!e.fileName.Equals(files[i]))
                {
                    i++;
                    Console.Write(" ***No Data Found in this File*** ");
                    Console.Write("\n\n\n =============================================================================  ");
                    Console.Write(" Processing File: \n {0}", files[i]);
                    Console.Write("\n =============================================================================  \n");
                    Console.Write("Entering  Leaving           Type                        Name    ");
                    Console.Write("\nLine No.  Line No.                                                      ");
                    Console.Write("\n-------------------------------------------------------------------------------");
                }
                b = true;
                Console.Write("\n{0,8} {1,8}{2,15}{3, 28}", e.begin, e.end, e.type, e.name);
            }
            displayCounters(cnamespace, cclass, cinterface, cfunction, cstruct, cenum, cdelegate, b);
            displaySummaryEnd(files, i);
        }

        //---------------<Displays the types Summary>------------------------
        public void displayFunctionComplexity(string[] files)
        {
            Console.Write("\n =============================================================================  ");
            Console.Write(" =============================================================================  ");
            Console.Write("                               FUNCTIONAL ANALYSIS");
            Console.Write("\n =============================================================================  ");
            Console.Write(" =============================================================================  \n\n");
            Console.Write("\n =============================================================================  ");
            Console.Write(" Processing File:\n {0}", files[0]);
            Console.Write("\n =============================================================================  \n");
            Console.Write("         Type                           Name      Size   Complexity    ");
            Console.Write("\n-------------------------------------------------------------------------------");
            int i = 0;
            List<Elem> outputList = OutputRepository.output_;

            foreach (Elem e in outputList)
            {
                if (!e.fileName.Equals(files[i]))
                {
                    i++;
                    Console.Write("\n\n\n =============================================================================  ");
                    Console.Write(" Processing File: \n {0}", files[i]);
                    Console.Write("\n =============================================================================  \n");
                    Console.Write("         Type                           Name      Size   Complexity    ");
                    Console.Write("\n-------------------------------------------------------------------------------");
                }
                if (!e.fileName.Equals(files[i]))
                {
                    i++;
                    Console.Write(" ***No Data Found in this File*** ");
                    Console.Write("\n\n\n =============================================================================  ");
                    Console.Write(" Processing File: \n {0}", files[i]);
                    Console.Write("\n =============================================================================  \n");
                    Console.Write("         Type                           Name      Size   Complexity    ");
                    Console.Write("\n-------------------------------------------------------------------------------");
                }
                if (e.type.Contains("function"))
                    Console.Write("\n{0,15} {1,28}{2,8}{3,8}", e.type, e.name, e.end - e.begin, e.functionComplexity);
            }
            for (int z = i + 1; z <= files.Length - 1; z++)
            {
                Console.Write("\n\n\n =============================================================================  ");
                Console.Write(" Processing File: \n {0}", files[z]);
                Console.Write("\n =============================================================================  \n");
                Console.Write("***No Data Found in this File*** ");

            }
            Console.Write("\n\n\n");
        }

        //---------------<Displays the relation Header>------------------
        public void displayRelationHeader(string[] files)
        {
            Console.Write("\n =============================================================================  ");
            Console.Write(" =============================================================================  ");
            Console.Write("                               TYPE RELATION ANALYSIS");
            Console.Write("\n =============================================================================  ");
            Console.Write(" =============================================================================  \n\n");
            Console.Write("\n =============================================================================  ");
            Console.Write(" Processing File:\n {0}", files[0]);
            Console.Write("\n =============================================================================  \n");
            Console.Write("                     From          Relationship                   To     ");
            Console.Write("\n-------------------------------------------------------------------------------");

        }

        //------------<Displays the Relationships>-------------------
        public void displayRelation(string[] files)
        {
            List<ElemRelation> table = RelationshipRepository.relationship_;
            displayRelationHeader(files);
            int i = 0;
            string name = null;
            foreach (ElemRelation e in table)
            {
                if (!e.fileName.Equals(files[i]))
                {
                    i++;
                    Console.Write("\n\n\n =============================================================================  ");
                    Console.Write(" Processing File: \n {0}", files[i]);
                    Console.Write("\n =============================================================================  \n");
                    Console.Write("                     From          Relationship                   To     ");
                    Console.Write("\n-------------------------------------------------------------------------------");
                }
                if (!e.fileName.Equals(files[i]))
                {
                    i++;
                    Console.Write("***No Data Found in this File*** ");
                    Console.Write("\n\n\n =============================================================================  ");
                    Console.Write(" Processing File: \n {0}", files[i]);
                    Console.Write("\n =============================================================================  \n");
                    Console.Write("                     From          Relationship                   To     ");
                    Console.Write("\n-------------------------------------------------------------------------------");
                }
                if (e.fromClass != name)
                {
                    Console.Write("\n{0,26} {1,19}{2,24}", e.fromClass, e.relationType, e.toClass);
                    name = e.fromClass;
                }
                else
                    Console.Write("\n{0,46} {1,23}", e.relationType, e.toClass);
            }
            for (int z = i + 1; z <= files.Length - 1; z++)
            {
                Console.Write("\n\n\n =============================================================================  ");
                Console.Write(" Processing File: \n {0}", files[z]);
                Console.Write("\n =============================================================================  \n");
                Console.Write("***No Data Found in this File*** ");
            }
            Console.Write("\n\n\n\n");
        }


        //--------------------------<Test Stub for Display>------------------------

#if(TEST_DISPLAY)

        static void Main(string[] args)
        {
            string path = "../../";
            string[] arg = { "../../", "*.cs" };
            List<string> patterns = new List<string>();
            patterns.Add("*.cs");
            List<string> options = new List<string>();
            string[] files = getFiles(path, patterns);

            foreach (string f in files)                  //to populate element Relation
            {
                ElemRelation e = new ElemRelation();
                e.fromClass = "Derived";
                e.toClass = "Original";
                e.relationType = "Inheritance";
                e.fileName = f;
                RelationshipRepository.relationship_.Add(e);
            }
            foreach (string f1 in files)         //to populate types
            {
                Elem elem = new Elem();
                elem.fileName = f1;
                elem.type = "function";
                elem.name = "Derived";
                elem.begin = 1;
                elem.end = 10;
                elem.functionComplexity = 2;
                OutputRepository.output_.Add(elem);
            }

            options.Add("/S");
            patterns.Add("*.cs");
            Display d = new Display();
            d.display(files, path, patterns, options);
            d.displaySummary(files);
            d.displayFunctionComplexity(files);
            d.displayRelation(files);
        }
#endif
    }
}

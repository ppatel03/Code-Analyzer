///////////////////////////////////////////////////////////////////////
// RulesAndActions.cs - Parser rules specific to an application      //
// ver 2.1                                                           //
// Language:    C#, 2008, .Net Framework 4.0                         //
// Platform:    Dell Precision T7400, Win7, SP1                      //
// Application: Demonstration for CSE681, Project #2, Fall 2011      //
// Author:      Jim Fawcett, CST 4-187, Syracuse University          //
//              (315) 443-3948, jfawcett@twcny.rr.com                //
// Modified by: Dhaval N Dholakiya, Syracuse University              //
//              (315) 447-7644, ddholaki@syr.edu                     //
///////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * RulesAndActions package contains all of the Application specific
 * code required for most analysis tools.
 *
 * It defines the following Four rules which each have a
 * grammar construct detector and also a collection of IActions:
 *   - DetectNameSpace rule
 *   - DetectClass rule
 *   - DetectFunction rule
 *   - DetectScopeChange
 *   - DetectEnum
 *   - Detect Complexity
 *   - Detect Inheritance
 *   - Detect Aggregation
 *   - Detect Composition
 *   - Detect Using
 *   
 *   Three actions - some are specific to a parent rule:
 *   - Print
 *   - PrintFunction
 *   - PrintScope
 * 
 * The package also defines a Repository class for passing data between
 * actions and uses the services of a ScopeStack, defined in a package
 * of that name.
 *
 * Note:
 * This package does not have a test stub since it cannot execute
 * without requests from Parser.
 *  
 */
/* Required Files:
 *   IRuleAndAction.cs, RulesAndActions.cs, Parser.cs, ScopeStack.cs,
 *   Semi.cs, Toker.cs
 *   
 * Build command:
 *   csc /D:TEST_PARSER Parser.cs IRuleAndAction.cs RulesAndActions.cs \
 *                      ScopeStack.cs Semi.cs Toker.cs
 *   
 * Maintenance History:
 * --------------------
 * ver 2.3 : 8 Oct 2014
 * Added different types like delegate and enum
 * Calculated the complexity and the scope of the fucntions
 * Identified Relationships between different types
 * Inheritance
 * Aggregation
 * Composition
 * Using
 * ver 2.2 : 24 Sep 2011
 * - modified Semi package to extract compile directives (statements with #)
 *   as semiExpressions
 * - strengthened and simplified DetectFunction
 * - the previous changes fixed a bug, reported by Yu-Chi Jen, resulting in
 * - failure to properly handle a couple of special cases in DetectFunction
 * - fixed bug in PopStack, reported by Weimin Huang, that resulted in
 *   overloaded functions all being reported as ending on the same line
 * - fixed bug in isSpecialToken, in the DetectFunction class, found and
 *   solved by Zuowei Yuan, by adding "using" to the special tokens list.
 * - There is a remaining bug in Toker caused by using the @ just before
 *   quotes to allow using \ as characters so they are not interpreted as
 *   escape sequences.  You will have to avoid using this construct, e.g.,
 *   use "\\xyz" instead of @"\xyz".  Too many changes and subsequent testing
 *   are required to fix this immediately.
 * ver 2.1 : 13 Sep 2011
 * - made BuildCodeAnalyzer a public class
 * ver 2.0 : 05 Sep 2011
 * - removed old stack and added scope stack
 * - added Repository class that allows actions to save and 
 *   retrieve application specific data
 * - added rules and actions specific to Project #2, Fall 2010
 * ver 1.1 : 05 Sep 11
 * - added Repository and references to ScopeStack
 * - revised actions
 * - thought about added folding rules
 * ver 1.0 : 28 Aug 2011
 * - first release
 *
 * Planned Modifications (not needed for Project #2):
 * --------------------------------------------------
 * - add folding rules:
 *   - CSemiExp returns for(int i=0; i<len; ++i) { as three semi-expressions, e.g.:
 *       for(int i=0;
 *       i<len;
 *       ++i) {
 *     The first folding rule folds these three semi-expression into one,
 *     passed to parser. 
 *   - CToker returns operator[]( as four distinct tokens, e.g.: operator, [, ], (.
 *     The second folding rule coalesces the first three into one token so we get:
 *     operator[], ( 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CodeAnalysis
{
    public class Elem  // holds scope information
    {
        public string type { get; set; }
        public string name { get; set; }
        public int begin { get; set; }
        public int end { get; set; }

        public int size { get; set; }

        public int functionComplexity { get; set; }  //for counting the complexity of a given function
        public string fileName { get; set; }        // passing the file name to print file by file data

        public override string ToString()
        {
            StringBuilder temp = new StringBuilder();
            temp.Append("{");
            temp.Append(String.Format("{0,-10}", type)).Append(" : ");
            temp.Append(String.Format("{0,-10}", name)).Append(" : ");
            temp.Append(String.Format("{0,-5}", begin.ToString()));  // line of scope start
            temp.Append(String.Format("{0,-5}", end.ToString()));    // line of scope end
            temp.Append("}");
            return temp.ToString();
        }
    }

    public class ElemRelation  // holds scope information
    {
        public string relationType { get; set; }
        public string fromClass { get; set; }
        public string toClass { get; set; }

        public string fileName { get; set; }

    }


    public class OutputRepository   //holds data to be displayed at parse1
    {
        public static List<Elem> output_ = new List<Elem>();

        public List<Elem> outputStorage
        {
            get { return output_; }
        }
    }

    public class RelationshipRepository     //holds data to be displayed at parse2
    {
        public static List<ElemRelation> relationship_ = new List<ElemRelation>();

        public List<ElemRelation> relationshipStorage
        {
            get { return relationship_; }
        }
    }

    public class Repository         //used to find the types in the file
    {
        ScopeStack<Elem> stack_ = new ScopeStack<Elem>();
        List<Elem> locations_ = new List<Elem>();
        static Repository instance;

        public Repository()
        {
            instance = this;
        }

        public static Repository getInstance()
        {
            return instance;
        }
        // provides all actions access to current semiExp

        public CSsemi.CSemiExp semi
        {
            get;
            set;
        }

        // semi gets line count from toker who counts lines
        // while reading from its source

        public int lineCount  // saved by newline rule's action
        {
            get { return semi.lineCount; }
        }
        public int prevLineCount  // not used in this demo
        {
            get;
            set;
        }
        // enables recursively tracking entry and exit from scopes

        public ScopeStack<Elem> stack  // pushed and popped by scope rule's action
        {
            get { return stack_; }
        }
        // the locations table is the result returned by parser's actions
        // in this demo

        public List<Elem> locations
        {
            get { return locations_; }
        }
    }
    /////////////////////////////////////////////////////////
    // pushes scope info on stack when entering new scope

    public class PushStack : AAction
    {
        Repository repo_;

        public PushStack(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(CSsemi.CSemiExp semi, string file)
        {
            Elem elem = new Elem();
            elem.type = semi[0];  // expects type
            if (elem.type == "scopeDetect")   //for incrementing the complexity of the function
            {
                int i = repo_.locations.Count;
                repo_.locations[i - 1].functionComplexity = repo_.locations[i - 1].functionComplexity + 1;
                return;
            }
            if (elem.type == "function")
            {
                elem.functionComplexity = 1;
            }
            elem.name = semi[1];  // expects name
            elem.begin = repo_.semi.lineCount - 1;
            elem.end = 0;
            elem.fileName = file;
            repo_.stack.push(elem);
            if (elem.type == "control" || elem.name == "anonymous")
                return;
            repo_.locations.Add(elem);
            OutputRepository.output_.Add(elem);

            if (AAction.displaySemi)
            {
                Console.Write("\n  line# {0,-5}", repo_.semi.lineCount - 1);
                Console.Write("entering ");
                string indent = new string(' ', 2 * repo_.stack.count);
                Console.Write("{0}", indent);
                this.display(semi); // defined in abstract action
            }
            if (AAction.displayStack)
                repo_.stack.display();
        }
    }
    /////////////////////////////////////////////////////////
    // pops scope info from stack when leaving scope

    public class PopStack : AAction
    {
        Repository repo_;

        public PopStack(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(CSsemi.CSemiExp semi, string file)
        {
            Elem elem;
            try
            {
                elem = repo_.stack.pop();
                for (int i = 0; i < repo_.locations.Count; ++i)
                {
                    Elem temp = repo_.locations[i];
                    if (elem.type == temp.type)
                    {
                        if (elem.name == temp.name)
                        {
                            if ((repo_.locations[i]).end == 0)
                            {
                                (repo_.locations[i]).end = repo_.semi.lineCount;
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
                Console.Write("popped empty stack on semiExp: ");
                semi.display();
                return;
            }
            CSsemi.CSemiExp local = new CSsemi.CSemiExp();
            local.Add(elem.type).Add(elem.name);
            if (local[0] == "control")
                return;

            if (AAction.displaySemi)
            {
                Console.Write("\n  line# {0,-5}", repo_.semi.lineCount);
                Console.Write("leaving  ");
                string indent = new string(' ', 2 * (repo_.stack.count + 1));
                Console.Write("{0}", indent);
                this.display(local); // defined in abstract action
            }
        }
    }
    ///////////////////////////////////////////////////////////
    // action to print function signatures - not used in demo

    public class PrintFunction : AAction
    {
        Repository repo_;

        public PrintFunction(Repository repo)
        {
            repo_ = repo;
        }
        public override void display(CSsemi.CSemiExp semi)
        {
            Console.Write("\n    line# {0}", repo_.semi.lineCount - 1);
            Console.Write("\n    ");
            for (int i = 0; i < semi.count; ++i)
                if (semi[i] != "\n" && !semi.isComment(semi[i]))
                    Console.Write("{0} ", semi[i]);
        }
        public override void doAction(CSsemi.CSemiExp semi, string file)
        {
            this.display(semi);
        }
    }
    /////////////////////////////////////////////////////////
    // concrete printing action, useful for debugging

    public class Print : AAction
    {
        Repository repo_;

        public Print(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(CSsemi.CSemiExp semi, string file)
        {
            Console.Write("\n  line# {0}", repo_.semi.lineCount - 1);
            this.display(semi);
        }
    }
    /////////////////////////////////////////////////////////
    // rule to detect namespace declarations

    public class DetectNamespace : ARule
    {
        public override bool test(CSsemi.CSemiExp semi, string file)
        {
            int index = semi.Contains("namespace");
            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // create local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index]).Add(semi[index + 1]);
                doActions(local, file);
                return true;
            }
            return false;
        }
    }
    //////////////////////////////////////
    //Detect Enum
    public class DetectEnum : ARule
    {
        public override bool test(CSsemi.CSemiExp semi, string file)
        {
            int index = semi.Contains("enum");

            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // create local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index]).Add(semi[index + 1]);
                doActions(local, file);
                return true;
            }
            return false;
        }
    }

    //////////////////////////////////////
    //Detect Delegates
    public class DetectDelegate : ARule
    {
        public override bool test(CSsemi.CSemiExp semi, string file)
        {
            int index = semi.Contains("delegate");

            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // create local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index]).Add(semi[index + 2]);
                doActions(local, file);
                return true;
            }
            return false;
        }
    }


    /////////////////////////////////////////////////////////
    // rule to dectect class definitions

    public class DetectClass : ARule
    {
        public override bool test(CSsemi.CSemiExp semi, string file)
        {
            int indexCL = semi.Contains("class");
            int indexIF = semi.Contains("interface");
            int indexST = semi.Contains("struct");
            //int indexEN = semi.Contains("enum");

            int index = Math.Max(indexCL, indexIF);
            index = Math.Max(index, indexST);
            // index = Math.Max(index, indexEN);
            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index]).Add(semi[index + 1]);
                doActions(local, file);
                return true;
            }
            return false;
        }
    }
    /////////////////////////////////////////////////////////
    // rule to dectect function definitions

    public class DetectFunction : ARule
    {
        public static bool isSpecialToken(string token)
        {
            string[] SpecialToken = { "if", "for", "foreach", "while", "catch", "using" };
            foreach (string stoken in SpecialToken)
                if (stoken == token)
                    return true;
            return false;
        }
        public override bool test(CSsemi.CSemiExp semi, string file)
        {
            if (semi[semi.count - 1] != "{")
                return false;

            int index = semi.FindFirst("(");
            if (index > 0 && !isSpecialToken(semi[index - 1]))
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                local.Add("function").Add(semi[index - 1]);
                doActions(local, file);
                return true;
            }
            return false;
        }
    }

    /////////////////////////////////////////////////////////
    //function to detect Complexity
    public class DetectComplexity : ARule
    {
        public override bool test(CSsemi.CSemiExp semi, string file)
        {
            string[] scopeDetect = { "if", "for", "else", "switch", "try", "catch", "finally", "while", "foreach" };

            foreach (string s in scopeDetect)
            {
                int index1 = semi.Contains(s);
                if (index1 != -1)
                {
                    CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                    // create local semiExp with tokens for type and name
                    local.displayNewLines = false;
                    local.Add("scopeDetect");
                    doActions(local, file);
                    return false;
                }
            }
            return false;
        }
    }

    /////////////////////////////////////////////////////////
    // detect entering anonymous scope
    // - expects namespace, class, and function scopes
    public class DetectAnonymousScope : ARule
    {
        public override bool test(CSsemi.CSemiExp semi, string file)
        {
            int index = semi.Contains("{");
            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // create local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add("control").Add("anonymous");
                doActions(local, file);
                return true;
            }
            return false;
        }
    }
    /////////////////////////////////////////////////////////
    // detect leaving scope

    public class DetectLeavingScope : ARule
    {
        public override bool test(CSsemi.CSemiExp semi, string file)
        {
            int index = semi.Contains("}");
            if (index != -1)
            {
                doActions(semi, file);
                return true;
            }
            return false;
        }
    }

    /////////////////////////////////////////////////////////
    // does necessary actions if the relationships are found

    public class PushStackRelationship : AAction
    {
        Repository repo_;

        public PushStackRelationship(Repository repo)
        {
            repo_ = repo;
        }

        //----<detects if it is a class and pushes onto the stack to check for last pushed class >-----
        public void doActionClass(CSsemi.CSemiExp semi, string file, ElemRelation elem)
        {
            if (semi[0].Equals("class"))
            {
                Elem elemcl = new Elem();
                elemcl.type = semi[0];  // expects type
                elemcl.name = semi[1];  // expects name
                elemcl.begin = repo_.semi.lineCount - 1;
                elemcl.end = 0;
                elemcl.fileName = file;
                repo_.stack.push(elemcl);
                repo_.locations.Add(elemcl);
            }

        }

        //----<set of actions if there is inheritance between two types>----------------
        public void doActionInheritance(CSsemi.CSemiExp semi, string file, ElemRelation elem)
        {
            if (semi[0].Equals("Inheritance"))
            {
                elem.fromClass = semi[1];
                elem.toClass = semi[2];
                elem.relationType = semi[0];
                elem.fileName = file;
                RelationshipRepository.relationship_.Add(elem);

            }

        }

        //------<set of actions if there is aggregation between two types>---------------
        public void doActionAggregation(CSsemi.CSemiExp semi, string file, ElemRelation elem)
        {
            if (semi[0].Equals("Aggregation"))
            {
                int i = repo_.locations.Count;
                if (!semi[1].Equals(repo_.locations[i - 1].name))
                {

                    elem.fromClass = repo_.locations[i - 1].name;
                    elem.toClass = semi[1];
                    elem.relationType = semi[0];
                    elem.fileName = file;
                    RelationshipRepository.relationship_.Add(elem);

                }
            }

        }

        //------------<set of actions if there is composition between two types>--------------
        public void doActionComposition(CSsemi.CSemiExp semi, string file, ElemRelation elem)
        {
            if (semi[0].Equals("Composition"))
            {
                int i = repo_.locations.Count;
                if (!semi[1].Equals(repo_.locations[i - 1].name))
                {

                    elem.fromClass = repo_.locations[i - 1].name;
                    elem.toClass = semi[1];
                    elem.relationType = semi[0];
                    elem.fileName = file;
                    RelationshipRepository.relationship_.Add(elem);
                }
            }


        }

        //----------<set of actions if there is using relation between two types>---------------
        public void doActionUsing(CSsemi.CSemiExp semi, string file, ElemRelation elem)
        {
            if (semi[0].Equals("Using"))
            {
                int i = repo_.locations.Count;
                if (!semi[1].Equals(repo_.locations[i - 1].name))
                {

                    elem.fromClass = repo_.locations[i - 1].name;
                    elem.toClass = semi[1];
                    elem.relationType = semi[0];
                    elem.fileName = file;
                    RelationshipRepository.relationship_.Add(elem);

                }
            }

        }
        //-----------<doAction after the rules are Detected for elationship Analysis>------------
        public override void doAction(CSsemi.CSemiExp semi, string file)
        {
            ElemRelation elem = new ElemRelation();

            doActionClass(semi, file, elem);
            doActionInheritance(semi, file, elem);
            doActionAggregation(semi, file, elem);
            doActionComposition(semi, file, elem);
            doActionUsing(semi, file, elem);

            if (AAction.displaySemi)
            {
                Console.Write("\n  line# {0,-5}", repo_.semi.lineCount - 1);
                Console.Write("entering ");
                string indent = new string(' ', 2 * repo_.stack.count);
                Console.Write("{0}", indent);
                this.display(semi); // defined in abstract action
            }
            if (AAction.displayStack)
                repo_.stack.display();
        }
    }
    /////////////////////////////////////////////////////////
    // rule to detect Inheritance

    public class DetectInheritance : ARule
    {
        public override bool test(CSsemi.CSemiExp semi, string file)
        {
            try
            {
                List<Elem> output = OutputRepository.output_;

                int index = semi.Contains(":");
                if (index != -1)
                {
                    foreach (Elem e in output)
                    {
                        if (semi[index + 1].Equals(e.name))
                        {
                            CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                            // create local semiExp with tokens for type and name
                            local.displayNewLines = false;
                            local.Add("Inheritance").Add(semi[index - 1]).Add(semi[index + 1]);
                            doActions(local, file);
                            return false;
                        }
                    }

                }
            }
            catch { }
            return false;
        }
    }
    /////////////////////////////////////////////////////////
    // rule to detect Aggregation

    public class DetectAggregation : ARule
    {
        public override bool test(CSsemi.CSemiExp semi, string file)
        {
            try
            {
                List<Elem> output = OutputRepository.output_;

                int index = semi.Contains("new");
                if (index != -1)
                {
                    foreach (Elem e in output)
                    {
                        if (semi[index - 3].Equals(e.name) || semi[index + 1].Equals(e.name))
                        {
                            CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                            // create local semiExp with tokens for type and name
                            local.displayNewLines = false;
                            if (semi[index + 2].Equals("."))
                            {
                                local.Add("Aggregation").Add(semi[index + 3]);

                            }
                            else
                            {
                                local.Add("Aggregation").Add(semi[index + 1]);
                            }
                            doActions(local, file);
                            return true;
                        }
                    }

                }
            }
            catch { }
            return false;
        }
    }

    /////////////////////////////////////////////////////////
    // rule to detect Composition

    public class DetectComposition : ARule
    {
        public override bool test(CSsemi.CSemiExp semi, string file)
        {
            try
            {
                List<Elem> output = OutputRepository.output_;
                foreach (Elem e in output)
                {
                    for (int i = 0; i < semi.count; i++)
                    {
                        if (semi[i].Equals(e.name) && (semi[i + 2].Equals(";")) && (e.type.Equals("class") || e.type.Equals("enum") || e.type.Equals("struct")))
                        {

                            if (semi[i - 1].Equals("(") || semi[i - 1].Equals("new"))
                            {
                                return false;
                            }
                            CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                            // create local semiExp with tokens for type and name
                            local.displayNewLines = false;
                            local.Add("Composition").Add(semi[i]);
                            doActions(local, file);
                            return true;
                        }
                    }
                }

            }
            catch
            {

            }
            return false;
        }
    }

    /////////////////////////////////////////////////////////
    // rule to detect Using

    public class DetectUsing : ARule
    {
        public static bool isSpecialToken(string token)
        {
            string[] SpecialToken = { "if", "for", "foreach", "while", "catch", "using" };
            foreach (string stoken in SpecialToken)
                if (stoken == token)
                    return true;
            return false;
        }
        public override bool test(CSsemi.CSemiExp semi, string file)
        {
            try
            {
                if (semi[semi.count - 1] != "{")
                    return false;

                int index = semi.FindFirst("(");
                if (index > 0 && !isSpecialToken(semi[index - 1]))
                {
                    List<Elem> output = OutputRepository.output_;

                    foreach (Elem e in output)
                    {
                        for (int i = 0; i < semi.count - 1; i++)
                        {
                            if (e.name.Equals(semi[i]) && !e.type.Equals("namespace") && !e.type.Equals("function"))
                            {
                                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                                local.Add("Using").Add(semi[i]);
                                doActions(local, file);
                                return true;
                            }

                        }
                    }

                }
            }
            catch { }
            return false;
        }
    }

    public class BuildCodeAnalyzer
    {
        Repository repo = new Repository();
        public BuildCodeAnalyzer(CSsemi.CSemiExp semi)
        {
            repo.semi = semi;
        }
        public virtual Parser build()
        {
            Parser parser = new Parser();
            // decide what to show
            AAction.displaySemi = false;
            AAction.displayStack = false;  // this is default so redundant
            // action used for namespaces, classes, and functions
            PushStack push = new PushStack(repo);
            // capture namespace info
            DetectNamespace detectNS = new DetectNamespace();
            detectNS.add(push);
            parser.add(detectNS);
            // capture class info
            DetectClass detectCl = new DetectClass();
            detectCl.add(push);
            parser.add(detectCl);
            // capture enum info
            DetectEnum detectEn = new DetectEnum();
            detectEn.add(push);
            parser.add(detectEn);
            // capture Delegate info
            DetectDelegate detectDe = new DetectDelegate();
            detectDe.add(push);
            parser.add(detectDe);
            // capture function info
            DetectFunction detectFN = new DetectFunction();
            detectFN.add(push);
            parser.add(detectFN);
            //Detect Complexity
            DetectComplexity detectComp = new DetectComplexity();
            detectComp.add(push);
            parser.add(detectComp);
            // handle entering anonymous scopes, e.g., if, while, etc.
            DetectAnonymousScope anon = new DetectAnonymousScope();
            anon.add(push);
            parser.add(anon);
            // handle leaving scopes
            DetectLeavingScope leave = new DetectLeavingScope();
            PopStack pop = new PopStack(repo);
            leave.add(pop);
            parser.add(leave);
            // parser configured
            return parser;
        }
    }

    public class BuildCodeAnalyzerRelation
    {
        Repository repo = new Repository();
        public BuildCodeAnalyzerRelation(CSsemi.CSemiExp semi)
        {
            repo.semi = semi;
        }
        public virtual Parser build()
        {
            Parser parser = new Parser();

            // decide what to show
            AAction.displaySemi = false;
            AAction.displayStack = false;  // this is default so redundant

            // action used for inheritance, aggregation, composition and using
            PushStackRelationship push = new PushStackRelationship(repo);

            // capture Inheritance info
            DetectInheritance detectNS = new DetectInheritance();
            detectNS.add(push);
            parser.add(detectNS);

            // capture class info
            DetectClass detectCl = new DetectClass();
            detectCl.add(push);
            parser.add(detectCl);

            // capture Aggr info
            DetectAggregation detectAG = new DetectAggregation();
            detectAG.add(push);
            parser.add(detectAG);

            // capture Composition info
            DetectComposition detectCP = new DetectComposition();
            detectCP.add(push);
            parser.add(detectCP);

            // capture Using info
            DetectUsing detectUs = new DetectUsing();
            detectUs.add(push);
            parser.add(detectUs);

            // parser configured
            return parser;
        }
    }
}


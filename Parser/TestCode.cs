using System.IO;

namespace CodeAnalysis
{
  public class Parser
  {
    enum days{Sat, Sun};
    private List<IRule> Rules;

    public void add(IRule rule)
    {
      if(rule.test(semi)!=0)
       {
       Add(rule);
        }  
      Rules.Add(rule);
    }



    public void parse(CSsemi.CSemiExp semi)
    {

      foreach (IRule rule in Rules)
      {
        if (rule.test(semi))
          break;
      }
      if(rule.test(semi)!=0)
       Add(rule);
    }
  }
}
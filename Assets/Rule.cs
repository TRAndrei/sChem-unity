using System;

public class Rule
{
	public string type;
	public bool hasLink;
	public string firstTypeInitial;
	public string firstTypeFinal;
	public string secondTypeInitial;
	public string secondTypeFinal;

	public Rule(string type, bool hasLink, string firstTypeInitial, string firstTypeFinal, string secondTypeInitial, string secondTypeFinal)
	{
		this.type = type;
		this.hasLink = hasLink;
		this.firstTypeInitial = firstTypeInitial;
		this.firstTypeFinal = firstTypeFinal;
		this.secondTypeInitial = secondTypeInitial;
		this.secondTypeFinal = secondTypeFinal;
	}

	public RuleKey GetRuleKeyInitial()
	{
		return new RuleKey(firstTypeInitial, secondTypeInitial);
	}

	public RuleKey GetRuleKeyFinal()
	{
		return new RuleKey(firstTypeFinal, secondTypeFinal);
	}
}

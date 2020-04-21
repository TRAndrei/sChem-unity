using System;
using System.Collections.Generic;

[Serializable]
public class Level
{
	public int size;
	public List<RandomElements> randomElements;
	public List<Elements> elements;
	public List<Links> links;
	public List<Rule> rules;

	[Serializable]
	public class RandomElements
	{
		public string type;
		public int count;
	}

	[Serializable]
	public class Elements
	{
		public string id;
		public string type;
		public float x;
		public float y;
	}

	[Serializable]
	public class Links
	{
		public string id;
		public string first;
		public string second;
	}
}

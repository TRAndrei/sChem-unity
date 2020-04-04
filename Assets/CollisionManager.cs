using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class CollisionManager
{
    private static readonly CollisionManager instance = new CollisionManager();

    private Dictionary<string, ElementScript> elements = new Dictionary<string, ElementScript>();
    private Dictionary<LinkKey, Link> links = new Dictionary<LinkKey, Link>();
    private Dictionary<RuleKey, Rule> rules = new Dictionary<RuleKey, Rule>();
    private Dictionary<RuleKey, List<LinkKey>> activeRules = new Dictionary<RuleKey, List<LinkKey>>();
    private Dictionary<string, HashSet<LinkKey>> elementLinks = new Dictionary<string, HashSet<LinkKey>>();

    static CollisionManager()
    {
    }

    private CollisionManager()
    {
    }

    public static CollisionManager Instance
    {
        get
        {
            return instance;
        }
    }

    public void UpdateAll()
    {
        foreach (var linkEntry in links)
        {
            linkEntry.Value.Update();
        }
    }

    public void Init()
    {
        rules.Add(new RuleKey("E0", "E0"), new Rule("R1", true, "E0", "E1", "E0", "E1"));
        rules.Add(new RuleKey("E1", "E1"), new Rule("R2", true, "E1", "E2", "E1", "E2"));
        rules.Add(new RuleKey("E1", "E2"), new Rule("R3", false, "E1", "E1", "E2", "E2"));
    }

    public void AddElement(ElementScript elementScript)
    {
        elements.Add(elementScript.name, elementScript);
    }

    public void AddCollision(ElementScript firstElement, ElementScript secondElement)
    {
        // see if we already have a link between these two objects
        LinkKey linkKey = new LinkKey(firstElement.name, secondElement.name);

        if (!links.ContainsKey(linkKey))
        {
            Rule rule;
            if ((rule = applyRule(new RuleKey(firstElement.Type, secondElement.Type), linkKey, firstElement, secondElement)) == null)
            {
                rule = applyRule(new RuleKey(secondElement.Type, firstElement.Type), linkKey, secondElement, firstElement);
            }

            if (rule != null)
            {
                // check for any links that should updated

                if (rule.firstTypeInitial != rule.firstTypeFinal)
                {

                }
            }
        }
    }

    private Rule applyRule(RuleKey ruleKey, LinkKey linkKey, ElementScript firstElement, ElementScript secondElement)
    {
        Rule rule;

        if (rules.TryGetValue(ruleKey, out rule))
        {
            // there is a rule between these two types
            if (rule.hasLink)
            {
                addLink(rule.type, ruleKey, linkKey, firstElement, secondElement);
            }
            else
            {
                // remove any link between these two objects
                removeLink(linkKey, firstElement, secondElement);
            }

            firstElement.Type = rule.firstTypeFinal;
            secondElement.Type = rule.secondTypeFinal;

            //UpdateLinks(new Queue<string> (new string[] {firstElement.name, secondElement.name }));
        }

        return rule;
    }

    private Link addLink(string ruleType, RuleKey ruleKey, LinkKey linkKey, ElementScript firstElement, ElementScript secondElement)
    {
        Link link = new Link(ruleType, firstElement, secondElement);
        links[linkKey] = link;

        List<LinkKey> linksForRule;
        if (!activeRules.TryGetValue(ruleKey, out linksForRule))
        {
            linksForRule = new List<LinkKey>();
        }

        linksForRule.Add(linkKey);

        HashSet<LinkKey> neighborsFirst;
        if (!elementLinks.TryGetValue(firstElement.name, out neighborsFirst))
        {
            neighborsFirst = new HashSet<LinkKey>();
            elementLinks.Add(firstElement.name, neighborsFirst);
        }

        neighborsFirst.Add(linkKey);

        HashSet<LinkKey> neighborsSecond;
        if (!elementLinks.TryGetValue(secondElement.name, out neighborsSecond))
        {
            neighborsSecond = new HashSet<LinkKey>();
            elementLinks.Add(secondElement.name, neighborsSecond);
        }

        neighborsSecond.Add(linkKey);

        return link;
    }

    private void removeLink(LinkKey linkKey, ElementScript firstElement, ElementScript secondElement)
    {
        Link link;

        if (links.TryGetValue(linkKey, out link))
        {
            links.Remove(linkKey);
            link.Remove();
        }

        elementLinks[firstElement.name].Remove(linkKey);
        elementLinks[secondElement.name].Remove(linkKey);
    }

    private void UpdateLinks(Queue<string> elementsToCheck)
    {
        while(elementsToCheck.Count >0)
        {
            string elementName = elementsToCheck.Dequeue();

            HashSet<LinkKey> neighbors;
            if (elementLinks.TryGetValue(elementName, out neighbors))
            {
                // see if there is a rule between the neighbor's type and the new element's type;
                ElementScript element = elements[elementName];

                foreach (LinkKey linkKey in neighbors)
                {
                    string otherElementName = linkKey.GetOther(elementName);
                    ElementScript otherElement = elements[otherElementName];

                    Rule rule;

                    if (rules.TryGetValue(new RuleKey(element.Type, otherElement.Type), out rule))
                    {
                        // there is a rule between these two types
                        if (rule.hasLink)
                        {
                            addLink(rule.type, new RuleKey(element.Type, otherElement.Type), linkKey, element, otherElement);
                        }
                        else
                        {
                            // remove any link between these two objects
                            removeLink(linkKey, element, otherElement);
                        }

                        element.Type = rule.firstTypeFinal;
                        otherElement.Type = rule.secondTypeFinal;
                        elementsToCheck.Enqueue(otherElementName);

                    } else if (rules.TryGetValue(new RuleKey(otherElement.Type, element.Type), out rule))
                    {
                        // there is a rule between these two types
                        if (rule.hasLink)
                        {
                            addLink(rule.type, new RuleKey(otherElement.Type, element.Type), linkKey, otherElement, element);
                        }
                        else
                        {
                            // remove any link between these two objects
                            removeLink(linkKey, element, otherElement);
                        }

                        otherElement.Type = rule.firstTypeFinal;
                        element.Type = rule.secondTypeFinal;
                        elementsToCheck.Enqueue(otherElementName);
                    }
                    else
                    {
                        removeLink(linkKey, element, otherElement);
                    }
                }
            }
        }
    }

}

using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class CollisionManager
{
    private static readonly CollisionManager instance = new CollisionManager();

    private Dictionary<string, ElementScript> elements = new Dictionary<string, ElementScript>();
    private Dictionary<LinkKey, Link> links = new Dictionary<LinkKey, Link>();
    private Dictionary<RuleKey, Rule> rules = new Dictionary<RuleKey, Rule>();
    private Dictionary<string, HashSet<LinkKey>> activeRules = new Dictionary<string, HashSet<LinkKey>>();
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
        
    }

    public void AddRule(Rule rule)
    {
        rules.Add(rule.GetRuleKeyInitial(), rule);
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
                Debug.Log("Adding link for rule " + rule.type + " between " + firstElement.name + " and " + secondElement.name);
                addLink(rule.type, linkKey, firstElement, secondElement);
            }
            else
            {
                // remove any link between these two objects
                Debug.Log("Removing link for rule " + rule.type + " between " + firstElement.name + " and " + secondElement.name);
                removeLink(linkKey, firstElement, secondElement);
            }

            firstElement.Type = rule.firstTypeFinal;
            secondElement.Type = rule.secondTypeFinal;

            UpdateLinks(linkKey);
        }

        return rule;
    }

    private Link addLink(string ruleType, LinkKey linkKey, ElementScript firstElement, ElementScript secondElement)
    {
        Link link;

        // if link already exists update it with the new rule type
        if (links.TryGetValue(linkKey, out link))
        {
            if (link.type != ruleType)
            {
                activeRules[link.type].Remove(linkKey);
                link.type = ruleType;
                activeRules[ruleType].Add(linkKey);
            }

            return link;
        }

        link = new Link(ruleType, firstElement, secondElement);
        links[linkKey] = link;

        HashSet<LinkKey> linksForRule;
        if (!activeRules.TryGetValue(ruleType, out linksForRule))
        {
            linksForRule = new HashSet<LinkKey>();
            activeRules.Add(ruleType, linksForRule);
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

    private void UpdateLinks(LinkKey originalLinkKey)
    {
        Queue<string> elementsToCheck = new Queue<string>();
        HashSet<string> processedElements = new HashSet<string>();
        elementsToCheck.Enqueue(originalLinkKey.first);
        elementsToCheck.Enqueue(originalLinkKey.second);
        processedElements.Add(originalLinkKey.first);
        processedElements.Add(originalLinkKey.second);

        while (elementsToCheck.Count > 0)
        {
            string elementName = elementsToCheck.Dequeue();            

            HashSet<LinkKey> neighborsOriginal;
            if (elementLinks.TryGetValue(elementName, out neighborsOriginal))
            {                
                // see if there is a rule between the neighbor's type and the new element's type;
                ElementScript element = elements[elementName];

                HashSet<LinkKey> neighbors = new HashSet<LinkKey>(neighborsOriginal);

                foreach (LinkKey linkKey in neighbors)
                {                   
                    string otherElementName = linkKey.GetOther(elementName);

                    if (processedElements.Contains(otherElementName))
                    {
                        continue;
                    }

                    ElementScript otherElement = elements[otherElementName];

                    Debug.Log("Processing link between " + elementName + " and " + otherElementName);

                    Rule rule;

                    if (rules.TryGetValue(new RuleKey(element.Type, otherElement.Type), out rule))
                    {
                        // there is a rule between these two types
                        if (rule.hasLink)
                        {
                            addLink(rule.type, linkKey, element, otherElement);
                        }
                        else
                        {
                            // remove any link between these two objects
                            removeLink(linkKey, element, otherElement);
                        }

                        element.Type = rule.firstTypeFinal;
                        otherElement.Type = rule.secondTypeFinal;
                        elementsToCheck.Enqueue(otherElementName);                        

                    }
                    else if (rules.TryGetValue(new RuleKey(otherElement.Type, element.Type), out rule))
                    {
                        // there is a rule between these two types
                        if (rule.hasLink)
                        {
                            addLink(rule.type, linkKey, otherElement, element);
                        }
                        else
                        {
                            // remove any link between these two objects
                            removeLink(linkKey, element, otherElement);
                        }

                        otherElement.Type = rule.firstTypeFinal;
                        element.Type = rule.secondTypeFinal;
                        if (!processedElements.Contains(otherElementName))
                        {
                            elementsToCheck.Enqueue(otherElementName);
                        }
                    }
                    else
                    {
                        removeLink(linkKey, element, otherElement);
                    }
                }
            }

            processedElements.Add(elementName);
        }
    }

}

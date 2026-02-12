using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Runtime.InteropServices;

class Program
{
    static void Main()
    {
        try
        {
            DirectoryEntry RootDse = new DirectoryEntry("LDAP://RootDSE");
            string defaultNamingContext = RootDse.Properties["defaultNamingContext"].Value.ToString();

            DirectoryEntry domainroot = new DirectoryEntry("LDAP://" + defaultNamingContext);
            DirectorySearcher groupsearcher = new DirectorySearcher(domainroot)
            {
                Filter = "(objectClass=group)"
            };

            groupsearcher.PropertiesToLoad.Add("distinguishedName");
            groupsearcher.PropertiesToLoad.Add("name");
            groupsearcher.PropertiesToLoad.Add("member");


            SearchResultCollection groups = groupsearcher.FindAll();

            var groupDictionary = new Dictionary<string, SearchResult>(StringComparer.OrdinalIgnoreCase);
            foreach (SearchResult group in groups)
            {
                string dn = group.Properties["distinguishedName"].Count > 0 ? group.Properties["distinguishedName"][0].ToString() : null;
                if (!string.IsNullOrEmpty(dn))
                {
                    groupDictionary[dn] = group;
                }
            }


            var processGroups = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            Console.WriteLine("Groups with nested group Hierarchy");
            Console.WriteLine();

            foreach (var groupdn in groupDictionary.Keys)
            {
                if (!processGroups.Contains(groupdn))
                    PrintGroupHierarchy(groupdn, groupDictionary, processGroups, 0);
            }


        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    static void PrintGroupHierarchy(string groupdn, Dictionary<string, SearchResult> groupDict, HashSet<string> ProcessGroups, int indenetLevel)
    {
        if (ProcessGroups.Contains(groupdn))
            return;

        ProcessGroups.Add(groupdn);
        if (!groupDict.TryGetValue(groupdn, out SearchResult group))
            return;

        string groupname = group.Properties["name"].Count > 0 ? group.Properties["name"][0].ToString() : "N/A";

        Console.WriteLine(new string(' ', indenetLevel * 4) + " -" + groupname);

        if (group.Properties["member"].Count > 0)
        {
            foreach (object memberDnObject in group.Properties["member"])
            {
                string memberDn = memberDnObject.ToString();
                if (groupDict.ContainsKey(memberDn))
                {
                    PrintGroupHierarchy(memberDn, groupDict, ProcessGroups, indenetLevel + 1);
                }
            }
        }

    }
}
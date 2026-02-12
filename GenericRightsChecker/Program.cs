using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Security.AccessControl;
using System.Security.Principal;

class Program
{
    // The generic rights to look for
    static readonly ActiveDirectoryRights[] GenericRightsToFind = new ActiveDirectoryRights[]
    {
        ActiveDirectoryRights.GenericAll,
        ActiveDirectoryRights.GenericWrite,
        ActiveDirectoryRights.GenericRead
    };

    static void Main()
    {
        try
        {
            DirectoryEntry rootDse = new DirectoryEntry("LDAP://RootDSE");
            string defaultNamingContext = rootDse.Properties["defaultNamingContext"].Value.ToString();
            DirectoryEntry domainRoot = new DirectoryEntry($"LDAP://{defaultNamingContext}");

            DirectorySearcher searcher = new DirectorySearcher(domainRoot)
            {
                Filter = "(objectClass=*)"
            };
            searcher.PageSize = 1000;
            searcher.PropertiesToLoad.Add("distinguishedName");
            searcher.PropertiesToLoad.Add("name");
            searcher.PropertiesToLoad.Add("objectClass");

            Console.WriteLine("Enumerating all AD objects for GenericAll/GenericWrite/GenericRead permissions...");

            SearchResultCollection results = searcher.FindAll();

            foreach (SearchResult result in results)
            {
                DirectoryEntry entry = result.GetDirectoryEntry();
                if (entry == null) continue;

                var matchingRules = GetGenericRightRules(entry);

                if (matchingRules.Count > 0)
                {
                    Console.WriteLine($"\nObject: {entry.Properties["name"].Value} ({entry.Properties["distinguishedName"].Value})");
                    Console.WriteLine($"ObjectClass: {string.Join(", ", entry.Properties["objectClass"].Value as IEnumerable<object> ?? new[] { entry.Properties["objectClass"].Value })}");
                    Console.WriteLine("Matching Permissions:");

                    foreach (var rule in matchingRules)
                    {
                        string principal = new SecurityIdentifier(rule.IdentityReference.Value).Translate(typeof(NTAccount)).ToString();
                        Console.WriteLine($"  Principal: {principal}");
                        Console.WriteLine($"   Access Type: {rule.AccessControlType}");
                        Console.WriteLine($"   Rights: {rule.ActiveDirectoryRights}");
                        Console.WriteLine($"   Inheritance: {rule.InheritanceType}");
                        Console.WriteLine($"   ObjectType: {rule.ObjectType}");
                        Console.WriteLine();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

        Console.WriteLine("Done.");
        Console.ReadLine();
    }

    static List<ActiveDirectoryAccessRule> GetGenericRightRules(DirectoryEntry entry)
    {
        List<ActiveDirectoryAccessRule> matchingRules = new List<ActiveDirectoryAccessRule>();

        try
        {
            ActiveDirectorySecurity security = entry.ObjectSecurity;
            AuthorizationRuleCollection acl = security.GetAccessRules(true, true, typeof(SecurityIdentifier));

            foreach (ActiveDirectoryAccessRule rule in acl)
            {
                foreach (var genericRight in GenericRightsToFind)
                {
                    if ((rule.ActiveDirectoryRights & genericRight) == genericRight
                        && rule.AccessControlType == AccessControlType.Allow)
                    {
                        matchingRules.Add(rule);
                        break;
                    }
                }
            }
        }
        catch
        {
            // Ignore errors reading security for some objects
        }

        return matchingRules;
    }
}

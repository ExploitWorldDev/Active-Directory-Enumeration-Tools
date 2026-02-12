using System;
using System.DirectoryServices;
using System.Security.AccessControl;
using System.Security.Principal;

class Program
{
    static void Main()
    {
        try
        {

            DirectoryEntry rootDse = new DirectoryEntry("LDAP://RootDSE");
            string defaultNamingContext = rootDse.Properties["defaultNamingContext"].Value.ToString();
            Console.WriteLine($"Default Naming Context: {defaultNamingContext}\n");

            var objectFilters = new (string filter, string type)[]
            {
                ("(&(objectCategory=user)(objectClass=user))", "User"),
                ("(&(objectCategory=computer)(objectClass=computer))", "Computer"),
                ("(&(objectCategory=group)(objectClass=group))", "Group")
            };

            Guid addSelfGuid = new Guid("ab721a53-1e2f-11d0-9819-00aa0040529b");

            foreach (var (filter, type) in objectFilters)
            {
                Console.WriteLine($"Enumerating AddSelf ACL for {type}s:");

                DirectoryEntry domainEntry = new DirectoryEntry($"LDAP://{defaultNamingContext}");
                DirectorySearcher searcher = new DirectorySearcher(domainEntry)
                {
                    Filter = filter
                };
                searcher.PropertiesToLoad.Add("distinguishedName");

                foreach (SearchResult result in searcher.FindAll())
                {
                    string dn = result.Properties["distinguishedName"][0].ToString();
                    DirectoryEntry entry = new DirectoryEntry($"LDAP://{dn}");
                    ActiveDirectorySecurity acl = entry.ObjectSecurity;
                    var rules = acl.GetAccessRules(true, true, typeof(SecurityIdentifier));

                    bool foundAddSelf = false;

                    foreach (ActiveDirectoryAccessRule rule in rules)
                    {
                        if (rule.AccessControlType == AccessControlType.Allow &&
                            (rule.ActiveDirectoryRights & ActiveDirectoryRights.WriteProperty) == ActiveDirectoryRights.WriteProperty &&
                            rule.ObjectType == addSelfGuid)
                        {
                            IdentityReference trustee = rule.IdentityReference.Translate(typeof(NTAccount));
                            Console.WriteLine($"{type}: {dn}");
                            Console.WriteLine($"  Trustee with AddSelf writeProperty right: {trustee.Value}");
                            foundAddSelf = true;
                        }
                    }

                    if (foundAddSelf)
                    {
                        Console.WriteLine();
                    }
                }

                Console.WriteLine(new string('-', 60));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
